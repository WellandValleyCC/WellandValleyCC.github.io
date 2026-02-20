using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public abstract class RoundRobinPageRenderer
    {
        protected readonly string IndexFileName;
        protected readonly RoundRobinResultsSet ResultsSet;

        protected RoundRobinPageRenderer(string indexFileName, RoundRobinResultsSet resultsSet)
        {
            IndexFileName = indexFileName;
            ResultsSet = resultsSet;
        }

        // ------------------------------------------------------------
        //  PUBLIC ENTRY POINT
        // ------------------------------------------------------------

        public string Render()
        {
            var timestamp = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm 'UTC'");

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>{GetPageTitle()}</title>
    <link rel=""stylesheet"" href=""../assets/{ResultsSet.CssFile}"">
</head>
<body class=""rr event-page"">

{RenderHeader()}

{RenderLegendIfNeeded()}

<main>
  {RenderMainContent()}
</main>

<footer><p class=""generated"">Generated {timestamp}</p></footer>

</body>
</html>";
        }

        // ------------------------------------------------------------
        //  ABSTRACT / VIRTUAL HOOKS
        // ------------------------------------------------------------

        /// <summary>
        /// The page title for the HTML <title> tag.
        /// Event pages will use the event title.
        /// Competition pages will use "Open Competition", etc.
        /// </summary>
        protected abstract string GetPageTitle();

        /// <summary>
        /// The full header block (banner + nav).
        /// Event pages have a rich header.
        /// Competition pages have a simpler header.
        /// </summary>
        protected abstract string RenderHeader();

        /// <summary>
        /// Legend is optional. Event pages and individual competitions use it.
        /// Team competition does not.
        /// </summary>
        protected virtual string RenderLegendIfNeeded() => string.Empty;

        /// <summary>
        /// The main content of the page (table, placeholder, etc.)
        /// </summary>
        protected abstract string RenderMainContent();

        // ------------------------------------------------------------
        //  SHARED HELPERS
        // ------------------------------------------------------------

        protected string RenderNavigationPills()
        {
            var prevLinkHtml = string.IsNullOrEmpty(ResultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{ResultsSet.PrevLink}"" aria-label=""Previous"">{ResultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(ResultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{ResultsSet.NextLink}"" aria-label=""Next"">{ResultsSet.NextLabel}</a>";

            return $@"
<nav class=""event-nav"" aria-label=""Page navigation"">
  {prevLinkHtml}
  <a class=""index"" href=""../{IndexFileName}"">Index</a>
  {nextLinkHtml}
</nav>";
        }

        protected string RenderCompetitionRules()
        {
            // Collect only non‑empty statements
            var parts = new[]
            {
        ResultsSet.EligibilityStatement,
        ResultsSet.ScoringStatement,
        ResultsSet.AdditionalComments
    }
            .Where(s => !string.IsNullOrWhiteSpace(s));

            // Join with <br/> and wrap in the standard block
            var joined = string.Join("<br/>", parts);

            return $@"
<div class=""rules-and-legend"">
  <section class=""competition-rules"">
    <p>{joined}</p>
  </section>
</div>";
        }

        protected static string FormatHosts(string rawHosts)
        {
            if (string.IsNullOrWhiteSpace(rawHosts))
                return string.Empty;

            var parts = rawHosts
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToList();

            if (parts.Count == 0)
                return string.Empty;

            if (parts.Count == 1)
                return parts[0];

            if (parts.Count == 2)
                return $"{parts[0]} & {parts[1]}";

            return string.Join(", ", parts.Take(parts.Count - 1))
                   + " & " + parts.Last();
        }

        protected static string StripClubSuffix(string name, string? roundRobinClub)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(roundRobinClub))
                return name;

            var pattern = @"\s*\(" + Regex.Escape(roundRobinClub.Trim()) + @"\)\s*$";

            return Regex.Replace(name, pattern, "", RegexOptions.IgnoreCase).TrimEnd();
        }

        /// <summary>
        /// Removes "Round Robin" from the title.
        /// </summary>
        /// <remarks>
        /// Used on Round Robin Event pages since all events are part of the Round Robin, so it's redundant 
        /// to include it in the title.
        /// </remarks>
        protected static string CleanTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return title;

            var cleaned = Regex.Replace(
                title,
                @"\s*round\s*robin\s*",
                "",
                RegexOptions.IgnoreCase
            );

            return cleaned.Trim();
        }

        /// <summary>
        /// Removes "Round Robin" and also "10 mile TT" from the title.
        /// </summary>
        /// <remarks>
        /// Used in competition tables to keep the title short while removing extraneous details.
        /// Lubenham Round Robin 10mile TT -> Lubenham
        /// </remarks>
        protected static string VeryCleanTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return title;

            var cleaned = CleanTitle(title);

            cleaned = Regex.Replace(
                cleaned,
                @"\s*10\s*mile\s*TT\s*",
                "",
                RegexOptions.IgnoreCase
            );

            return cleaned.Trim();
        }
    }
}