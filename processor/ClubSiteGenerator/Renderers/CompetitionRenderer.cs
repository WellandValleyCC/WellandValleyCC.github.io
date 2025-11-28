using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers
{
    public class CompetitionRenderer : HtmlRendererBase
    {
        private readonly CompetitionResultsSet resultsSet;
        private readonly IReadOnlyList<CalendarEvent> calendar;
        private readonly string competitionTitle;

        internal readonly List<string> fixedColumnTitles;
        internal readonly List<string> columnTitles;

        public CompetitionRenderer(CompetitionResultsSet resultsSet, IEnumerable<CalendarEvent> calendar)
        {
            this.resultsSet = resultsSet;
            this.calendar = calendar.OrderBy(ev => ev.EventNumber).ToList();
            this.competitionTitle = resultsSet.DisplayName;

            fixedColumnTitles = new List<string>
            {
                "Name",
                "Current rank",
                "Events completed",
                "10-mile TTs Best 8",
                "Scoring 11"
            };

            // columnTitles = fixed + event names
            columnTitles = fixedColumnTitles.Concat(calendar.Select(ev => ev.EventName)).ToList();
        }

        protected override string TitleElement()
            => $"<title>{WebUtility.HtmlEncode(competitionTitle)}</title>";

        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"  <h1>{WebUtility.HtmlEncode(competitionTitle)}</h1>");
            sb.AppendLine($"  <p class=\"competition-code\">Code: {resultsSet.CompetitionType}</p>");
            return sb.ToString();
        }

        protected override string LegendHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<div class=\"legend\">");
            sb.AppendLine("  <span class=\"ten-mile-event\">10‑mile events</span>");
            sb.AppendLine("  <span class=\"non-ten-mile-event\">Other events</span>");
            sb.AppendLine("</div>");
            return sb.ToString();
        }

        protected override string ResultsTableHtml()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table class=\"results\">");
            sb.AppendLine(RenderHeader());
            sb.AppendLine(RenderBody());
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private string RenderHeader()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<thead>");
            sb.AppendLine(RenderEventNumberRow());
            sb.AppendLine(RenderEventDateRow());
            sb.AppendLine(RenderTitleRow());
            sb.AppendLine("</thead>");
            return sb.ToString();
        }

        private string RenderEventNumberRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var _ in fixedColumnTitles)
                sb.AppendLine("<th class=\"fixed-column-title\"></th>");

            foreach (var ev in calendar.OrderBy(e => e.EventNumber))
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                sb.AppendLine($"<th class=\"event-number {cssClass}\">{ev.EventNumber}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderEventDateRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var _ in fixedColumnTitles)
                sb.AppendLine("<th class=\"fixed-column-title\"></th>");

            foreach (var ev in calendar.OrderBy(e => e.EventNumber))
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                sb.AppendLine($"<th class=\"event-date {cssClass}\">{ev.EventDate:ddd dd/MM/yy}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderTitleRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var title in fixedColumnTitles)
                sb.AppendLine($"<th class=\"fixed-column-title\">{WebUtility.HtmlEncode(title)}</th>");

            foreach (var ev in calendar.OrderBy(e => e.EventNumber))
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                sb.AppendLine($"<th class=\"event-title {cssClass}\">{WebUtility.HtmlEncode(ev.EventName)}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }


        private string RenderBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tbody>");

            foreach (var result in resultsSet.ScoredRides)
                sb.AppendLine(RenderRow(result));

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(CompetitorResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var cell in BuildCells(result).Select((value, index) => RenderCell(value, index)))
                sb.AppendLine(cell);

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private IEnumerable<string> BuildCells(CompetitorResult result)
        {
            yield return result.Competitor.FullName;
            yield return result.RankDisplay;
            yield return result.EventsCompleted.ToString();
            yield return result.Best8TenMileDisplay;
            yield return result.Scoring11Display;

            // per-event columns
            foreach (var ev in calendar.OrderBy(e => e.EventNumber))
            {
                var hasStatus = result.EventStatuses.TryGetValue(ev.EventNumber, out var status);
                var hasPoints = result.EventPoints.TryGetValue(ev.EventNumber, out var points);

                string display;

                if (!hasStatus)
                {
                    display = "-";
                }
                else
                {
                    display = status switch
                    {
                        RideStatus.Valid => points.HasValue
                            ? Math.Round(points.Value, MidpointRounding.AwayFromZero).ToString()
                            : string.Empty,
                        _ => status.GetDisplayName() ?? string.Empty
                    };
                }

                yield return display;
            }
        }

        private string RenderCell(string value, int index)
        {
            var encodedValue = WebUtility.HtmlEncode(value);

            // Fixed columns (0..fixedColumnTitles.Count-1): no class
            if (index < fixedColumnTitles.Count)
            {
                return $"<td>{encodedValue}</td>";
            }

            // Event columns: look up corresponding CalendarEvent
            var ev = calendar[index - fixedColumnTitles.Count];
            var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";

            return $"<td class=\"{cssClass}\">{encodedValue}</td>";
        }
    }
}
