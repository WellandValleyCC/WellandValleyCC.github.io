using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public abstract class RoundRobinPageRenderer
    {
        protected readonly string IndexFileName;
        protected readonly RoundRobinEventResultsSet ResultsSet;

        protected readonly string EventTitle;
        protected readonly int EventNumber;
        protected readonly DateOnly EventDate;
        protected readonly double EventMiles;
        protected readonly bool IsCancelled;
        protected readonly string EventDistanceText;

        protected RoundRobinPageRenderer(string indexFileName, RoundRobinEventResultsSet resultsSet)
        {
            IndexFileName = indexFileName;
            ResultsSet = resultsSet;

            EventTitle = CleanTitle(resultsSet.DisplayName);
            EventNumber = resultsSet.EventNumber;
            EventDate = resultsSet.EventDate;
            EventMiles = resultsSet.CalendarEvent.Miles;
            IsCancelled = resultsSet.CalendarEvent.IsCancelled;

            EventDistanceText = $"{resultsSet.CalendarEvent.Miles:0.#} miles";
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
    <title>{EventTitle}</title>
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
        //  HEADER
        // ------------------------------------------------------------

        protected string RenderHeader()
        {
            var prevLinkHtml = string.IsNullOrEmpty(ResultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{ResultsSet.PrevLink}"" aria-label=""Previous"">{ResultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(ResultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{ResultsSet.NextLink}"" aria-label=""Next"">{ResultsSet.NextLabel}</a>";

            var hostClubName = FormatHosts(ResultsSet.CalendarEvent.RoundRobinClub);
            var rrHeaderDate = EventDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

            var headerClasses = IsCancelled
                ? "event-header-core cancelled-event"
                : "event-header-core";

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""{headerClasses}"">
        <h1>
          <span class=""event-number"">Event {EventNumber}:</span>
          {EventTitle}
        </h1>
        <p class=""event-host"">Hosted by {hostClubName}</p>
        <p class=""event-date"">{rrHeaderDate}</p>
        <p class=""event-distance"">Distance: {EventDistanceText}</p>
      </div>
    </div>

    <nav class=""event-nav"" aria-label=""Event navigation"">
      {prevLinkHtml}
      <a class=""index"" href=""../{IndexFileName}"" aria-label=""Back to index"">Index</a>
      {nextLinkHtml}
    </nav>
  <div>
</header>";
        }

        // ------------------------------------------------------------
        //  LEGEND HOOK (OPTIONAL)
        // ------------------------------------------------------------

        protected virtual string RenderLegendIfNeeded() => string.Empty;

        // ------------------------------------------------------------
        //  ABSTRACT MAIN CONTENT
        // ------------------------------------------------------------

        protected abstract string RenderMainContent();

        // ------------------------------------------------------------
        //  SHARED HELPERS
        // ------------------------------------------------------------

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
    }
}