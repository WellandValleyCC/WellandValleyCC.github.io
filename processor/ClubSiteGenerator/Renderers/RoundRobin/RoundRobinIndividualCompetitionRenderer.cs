using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinIndividualCompetitionRenderer
        : RoundRobinCompetitionPageRenderer<RoundRobinRiderResult>
    {
        private readonly int IndividualCompetitionEventLimit;

        public RoundRobinIndividualCompetitionRenderer(
            string indexFileName,
            RoundRobinCompetitionResultsSet<RoundRobinRiderResult> resultsSet)
                 : base(indexFileName, resultsSet)
        {
            IndividualCompetitionEventLimit = resultsSet.CompetitionRules?.RoundRobin.Count ?? 0;
        }

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        protected override string GetPageTitle() =>
            CleanTitle(ResultsSet.DisplayName);

        // ------------------------------------------------------------
        //  HEADER (COMPETITION-SPECIFIC)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var title = CleanTitle(ResultsSet.DisplayName);

            var prevLinkHtml = string.IsNullOrEmpty(ResultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{ResultsSet.PrevLink}"" aria-label=""Previous"">{ResultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(ResultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{ResultsSet.NextLink}"" aria-label=""Next"">{ResultsSet.NextLabel}</a>";

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""event-header-core"">
        <h1>{title}</h1>
      </div>
    </div>

    {RenderNavigationPills()}
  </div>
</header>";
        }

        // ------------------------------------------------------------
        //  LEGEND (INDIVIDUAL COMPETITIONS ALWAYS SHOW IT)
        // ------------------------------------------------------------

        // N/A - use default legend

        // ------------------------------------------------------------
        //  MAIN CONTENT (INDIVIDUAL COMPETITION TABLE)
        // ------------------------------------------------------------

        protected override string RenderMainContent()
        {
            var sb = new StringBuilder();

            sb.AppendLine(RenderCompetitionRules());
            sb.AppendLine("<table class=\"results\">");
            sb.AppendLine(RenderHeaderRow());
            sb.AppendLine(RenderBody());
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  TABLE STRUCTURE
        // ------------------------------------------------------------

        private string RenderHeaderRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<thead><tr>");

            sb.AppendLine("<th>Name</th>");
            sb.AppendLine("<th>Club</th>");
            sb.AppendLine("<th>Rank</th>");
            sb.AppendLine("<th>Events</th>");
            sb.AppendLine($"<th>Best {IndividualCompetitionEventLimit}</th>");

            // One column per event in the season
            foreach (var evt in ResultsSet.Calendar)
                sb.AppendLine($"<th>{evt.RoundRobinEventNumber}</th>");

            sb.AppendLine("</tr></thead>");
            return sb.ToString();
        }

        private string RenderBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tbody>");

            foreach (var rider in ResultsSet.Results)
                sb.AppendLine(RenderRow(rider));

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(RoundRobinRiderResult rider)
        {
            var sb = new StringBuilder();
            var cssClass = string.IsNullOrWhiteSpace(rider.Rider.RoundRobinClub)
                ? "guest-non-club-member"
                : "competition-eligible";

            sb.AppendLine($"<tr class=\"{cssClass}\">");

            foreach (var cell in BuildCells(rider))
                sb.AppendLine($"<td>{WebUtility.HtmlEncode(cell)}</td>");

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  CELL BUILDING
        // ------------------------------------------------------------

        private IEnumerable<string> BuildCells(RoundRobinRiderResult riderResult)
        {
            yield return riderResult.Rider.Name;
            yield return riderResult.Rider.RoundRobinClub?? "";
            yield return riderResult.Rank?.ToString() ?? "";
            yield return riderResult.EventsCompleted.ToString();
            // Use a safe, non-null display value for the total score.
            // CompetitionScore exposes PointsDisplay; if Total is null, return empty string.
            yield return riderResult.Total?.PointsDisplay ?? "";

            // Per-event points (or blank)
            //foreach (var evt in ResultsSet.Calendar)
            //    yield return riderResult.EventPoints.TryGetValue(evt.EventNumber, out var pts)
            //        ? pts.ToString()
            //        : "";
        }
    }
}