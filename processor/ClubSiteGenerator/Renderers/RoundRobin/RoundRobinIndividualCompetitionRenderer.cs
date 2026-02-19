using ClubCore.Utilities;
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

        private string PageTitle => ResultsSet.DisplayName;

        protected override string GetPageTitle() => PageTitle;

        // ------------------------------------------------------------
        //  HEADER (COMPETITION-SPECIFIC)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var title = PageTitle;

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
            sb.AppendLine("<th>Current rank</th>");
            sb.AppendLine("<th>Events completed</th>");
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
            {
                if (cell is RawHtml raw)
                {
                    sb.AppendLine($"<td>{raw.Value}</td>");
                }
                else
                {
                    sb.AppendLine($"<td>{WebUtility.HtmlEncode(cell?.ToString() ?? "")}</td>");
                }
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  CELL BUILDING
        // ------------------------------------------------------------

        private IEnumerable<object> BuildCells(RoundRobinRiderResult riderResult)
        {
            yield return riderResult.Rider.Name;
            yield return riderResult.Rider.RoundRobinClub ?? "";
            yield return riderResult.Rank?.ToString() ?? "";
            yield return riderResult.EventsCompleted.ToString();
            yield return riderResult.Total?.PointsDisplay ?? "";

            // Per‑event points with scoring highlight
            foreach (var evt in ResultsSet.Calendar)
            {
                riderResult.EventPoints.TryGetValue(evt.RoundRobinEventNumber, out var pts);

                bool isScoring = IsScoringRide(riderResult, evt.RoundRobinEventNumber);

                if (isScoring)
                    yield return new RawHtml($"<span class=\"rr-scoring\">{pts}</span>");
                else
                    yield return pts?.ToString() ?? "";
            }
        }


        private bool IsScoringRide(RoundRobinRiderResult rider, int eventNumber)
        {
            return rider.Total.Rides
                .Any(r => r.CalendarEvent!.RoundRobinEventNumber == eventNumber);
        }
    }
}