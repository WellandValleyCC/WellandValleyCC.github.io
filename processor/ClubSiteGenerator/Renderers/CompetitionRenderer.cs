using ClubCore.Models;
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

            sb.AppendLine("  <nav class=\"competition-nav\" aria-label=\"Competition navigation\">");
            sb.AppendLine($"    <a class=\"prev\" href=\"{resultsSet.PrevLink}\" aria-label=\"Previous\">{resultsSet.PrevLabel}</a>");
            sb.AppendLine("    <a class=\"index\" href=\"../preview.html\" aria-current=\"page\" aria-label=\"Back to index\">Index</a>");
            sb.AppendLine($"    <a class=\"next\" href=\"{resultsSet.NextLink}\" aria-label=\"Next\">{resultsSet.NextLabel}</a>");
            sb.AppendLine("  </nav>");

            sb.AppendLine("  <section class=\"competition-rules\">");
            sb.AppendLine("    <p>");
            sb.AppendLine("      You must compete in at least two TTs which are not standard 10 mile events.");
            sb.AppendLine("      When you have met this requirement, your score will be based on the best two");
            sb.AppendLine("      scores from these events, plus the scores from your best 9 other events");
            sb.AppendLine("      (any distance).");
            sb.AppendLine("    </p>");
            sb.AppendLine("  </section>");

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
                sb.AppendLine("<th class=\"invisible-cell\"></th>");

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
                sb.AppendLine("<th class=\"invisible-cell\"></th>");

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

            foreach (var cell in BuildCells(result).Select((value, index) => RenderCell(value, index, result.Competitor)))
                sb.AppendLine(cell);

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private IEnumerable<string> BuildCells(CompetitorResult result)
        {
            yield return result.Competitor.FullName;
            yield return result.FullCompetition.RankDisplay;
            yield return result.EventsCompleted.ToString();
            yield return result.TenMileCompetition.PointsDisplay;
            yield return result.FullCompetition.PointsDisplay;

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

        private string RenderCell(string value, int index, Competitor competitor)
        {
            var encodedValue = WebUtility.HtmlEncode(value);

            // Column indices (zero-based)
            const int firstFixedIndexEnd = 2;   // 0..2 => fixed columns (no class)
            const int best8Index = 3;           // column 4
            const int scoring11Index = 4;       // column 5
            const int firstEventIndex = 5;      // column 6 onwards

            // Fixed columns: 0..2
            if (index <= firstFixedIndexEnd)
            {
                return $"<td>{encodedValue}</td>";
            }

            if (index == best8Index)
            {
                var podiumClass = GetPodiumClassForBest8(competitor);
                var best8CssClass = string.IsNullOrEmpty(podiumClass)
                    ? "best-8"
                    : $"best-8 {podiumClass}";

                return $"<td class=\"{best8CssClass}\">{encodedValue}</td>";
            }

            if (index == scoring11Index)
            {
                var podiumClass = GetPodiumClassForScoring11(competitor);
                var scoring11CssClass = string.IsNullOrEmpty(podiumClass)
                    ? "scoring-11"
                    : $"scoring-11 {podiumClass}";

                return $"<td class=\"{scoring11CssClass}\">{encodedValue}</td>";
            }

            // Event columns: index 5+
            // Map to calendar by subtracting the number of non-event columns (5).
            var calendarIndex = index - firstEventIndex;
            var ev = calendar[calendarIndex];
            var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";

            return $"<td class=\"{cssClass}\">{encodedValue}</td>";
        }

        private string? GetPodiumClassForScoring11(Competitor competitor)
        {
            var result = resultsSet.ScoredRides.FirstOrDefault(r => r.Competitor == competitor);
            return result?.FullCompetition.Rank switch
            {
                1 => "gold",
                2 => "silver",
                3 => "bronze",
                _ => string.Empty
            };
        }

        private string? GetPodiumClassForBest8(Competitor competitor)
        {
            var result = resultsSet.ScoredRides.FirstOrDefault(r => r.Competitor == competitor);
            return result?.TenMileCompetition.Rank switch
            {
                1 => "gold",
                2 => "silver",
                3 => "bronze",
                _ => string.Empty
            };
        }
    }
}
