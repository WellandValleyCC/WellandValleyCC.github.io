using ClubCore.Utilities;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinTeamCompetitionRenderer
        : RoundRobinCompetitionPageRenderer<RoundRobinTeamResult>
    {
        public RoundRobinTeamCompetitionRenderer(
            string indexFileName,
            RoundRobinCompetitionResultsSet<RoundRobinTeamResult> resultsSet)
            : base(indexFileName, resultsSet)
        {
        }

        protected override string UiHint =>
            "Click an event score to see the contributing rides.";

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        private string PageTitle => ResultsSet.DisplayName;

        protected override string GetPageTitle() => PageTitle;

        // ------------------------------------------------------------
        //  HEADER (TEAM COMPETITION)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var title = PageTitle;

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
        //  LEGEND (TEAM COMPETITION DOES NOT USE IT)
        // ------------------------------------------------------------

        protected override string RenderLegendIfNeeded() => string.Empty;

        // ------------------------------------------------------------
        //  MAIN CONTENT
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
        //  TABLE HEADER
        // ------------------------------------------------------------

        private string RenderHeaderRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<thead>");
            sb.AppendLine(RenderEventNumberRow());
            sb.AppendLine(RenderEventTitleRow());
            sb.AppendLine(RenderEventDateRow());
            sb.AppendLine("</thead>");
            return sb.ToString();
        }

        private string RenderEventNumberRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            sb.AppendLine("<th rowspan=\"3\" class=\"fixed-column-title\">Team</th>");
            sb.AppendLine("<th rowspan=\"3\" class=\"fixed-column-title\">Current rank</th>");
            sb.AppendLine("<th rowspan=\"3\" class=\"fixed-column-title\">Total</th>");

            foreach (var ev in ResultsSet.Calendar)
                sb.AppendLine($"<th class=\"event-number\">{ev.RoundRobinEventNumber}</th>");

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderEventTitleRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var ev in ResultsSet.Calendar)
            {
                var cssClass = "event-title";
                if (ev.IsCancelled)
                    cssClass += " cancelled-event";

                sb.AppendLine(
                    $"<th class=\"{cssClass}\" data-col-index=\"{ev.RoundRobinEventNumber}\">" +
                    $"{WebUtility.HtmlEncode($"{VeryCleanTitle(ev.EventName)} ({ev.RoundRobinClub})")}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderEventDateRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var ev in ResultsSet.Calendar)
            {
                var dateString = ev.EventDate.ToString("ddd dd/MM/yy");
                sb.AppendLine($"<th class=\"event-date\">{dateString}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  TABLE BODY
        // ------------------------------------------------------------

        private string RenderBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tbody>");

            foreach (var team in ResultsSet.Results)
                sb.AppendLine(RenderRow(team));

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(RoundRobinTeamResult team)
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"<tr class=""competition-eligible"">");

            foreach (var cell in BuildCells(team))
            {
                if (cell is RawHtml raw)
                    sb.AppendLine($"<td>{raw.Value}</td>");
                else
                    sb.AppendLine($"<td>{WebUtility.HtmlEncode(cell?.ToString() ?? "")}</td>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  CELL BUILDING
        // ------------------------------------------------------------

        private IEnumerable<object> BuildCells(RoundRobinTeamResult team)
        {
            yield return team.ClubShortName;
            yield return team.Rank?.ToString() ?? "";
            yield return team.Total?.PointsDisplay ?? "";

            foreach (var evt in ResultsSet.Calendar)
            {
                team.EventPoints.TryGetValue(evt.RoundRobinEventNumber, out var pts);

                // Merge Open + Women contributors
                var open = team.ContributingOpenRidesByEvent
                    .TryGetValue(evt.RoundRobinEventNumber, out var o)
                        ? o
                        : Array.Empty<RoundRobinRiderScore>();

                var women = team.ContributingWomenRidesByEvent
                    .TryGetValue(evt.RoundRobinEventNumber, out var w)
                        ? w
                        : Array.Empty<RoundRobinRiderScore>();

                var fullListHtml = BuildFullContributorListHtml(open, women);

                bool isScoring = pts.HasValue;

                if (isScoring)
                {
                    yield return new RawHtml($@"
<div class=""team-event-cell collapsed"" onclick=""this.classList.toggle('expanded')"">
  <span class=""rr-scoring"">{pts}</span>
  <div class=""rr-contributors-full"">
    {fullListHtml}
  </div>
</div>");
                }
                else
                {
                    yield return "";
                }
            }
        }

        // ------------------------------------------------------------
        //  CONTRIBUTOR FORMATTING
        // ------------------------------------------------------------

        private static string BuildFullContributorListHtml(
            IReadOnlyList<RoundRobinRiderScore> open,
            IReadOnlyList<RoundRobinRiderScore> women)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"<div class=""rr-open-group"">");
            foreach (var r in open)
                sb.AppendLine($"<div>{r.Points} {WebUtility.HtmlEncode(r.Rider.Name)}</div>");
            sb.AppendLine("</div>");

            if (women.Any())
            {
                sb.AppendLine(@"<div class=""rr-separator""></div>");

                sb.AppendLine(@"<div class=""rr-women-group"">");
                foreach (var r in women)
                    sb.AppendLine($"<div>{r.Points} {WebUtility.HtmlEncode(r.Rider.Name)}</div>");
                sb.AppendLine("</div>");
            }

            return sb.ToString();
        }
    }
}