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

        internal readonly List<string> columnTitles;

        protected override string PageTypeClass => "competition-page";

        public CompetitionRenderer(CompetitionResultsSet resultsSet, IEnumerable<CalendarEvent> calendar)
        {
            this.resultsSet = resultsSet;
            this.calendar = calendar.OrderBy(ev => ev.EventNumber).ToList();
            this.competitionTitle = resultsSet.DisplayName;

            // columnTitles = fixed + event names
            columnTitles = groupedFixedColumns
                .SelectMany(group =>
                    group.SubTitles.Count == 0
                        ? new[] { group.GroupTitle }
                        : group.SubTitles.Select(sub => $"{group.GroupTitle}: {sub}")
                )
                .Concat(this.calendar.Select(ev => ev.EventName))
                .ToList();
        }

        internal readonly List<(string GroupTitle, List<string> SubTitles)> groupedFixedColumns = new()
        {
            ("Name", new List<string>()), // no sub-columns
            ("Current rank", new List<string> { "Competition", "Tens" }),
            ("Events completed", new List<string> { "Tens", "Non-tens" }),
            ("Scoring 11", new List<string>()),
            ("10-mile TTs Best 8", new List<string>())
        };

        protected override string TitleElement()
            => $"<title>{WebUtility.HtmlEncode(competitionTitle)}</title>";


        protected virtual string ChampionshipEligibilityStatement => string.Empty;
        
        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"  <h1>{WebUtility.HtmlEncode(competitionTitle)}</h1>");

            sb.AppendLine("  <nav class=\"competition-nav\" aria-label=\"Competition navigation\">");
            sb.AppendLine($"    <a class=\"prev\" href=\"{resultsSet.PrevLink}\" aria-label=\"Previous\">{resultsSet.PrevLabel}</a>");
            sb.AppendLine("    <a class=\"index\" href=\"../preview.html\" aria-current=\"page\" aria-label=\"Back to index\">Index</a>");
            sb.AppendLine($"    <a class=\"next\" href=\"{resultsSet.NextLink}\" aria-label=\"Next\">{resultsSet.NextLabel}</a>");
            sb.AppendLine("  </nav>");

            sb.AppendLine("<div class=\"rules-and-legend\">");

            sb.AppendLine("  <section class=\"competition-rules\">");
            sb.AppendLine("    <p>");
            sb.AppendLine($"      {WebUtility.HtmlEncode(resultsSet.EligibilityStatement)}");
            sb.AppendLine("      To qualify, you must ride at least two non-ten TTs – e.g. 9.5 mile hard-ride, 25 mile TT, etc.");
            sb.AppendLine("      <br/>");
            sb.AppendLine("      Your championship score is the total of the points from your two highest scoring non-ten events, plus your best 9 other events of any distance.");
            sb.AppendLine("    </p>");
            sb.AppendLine("  </section>");

            sb.AppendLine("  <div class=\"legend\">");
            sb.AppendLine("    <span class=\"ten-mile-event\">10‑mile events</span>");
            sb.AppendLine("    <span class=\"non-ten-mile-event\">Other events</span>");
            sb.AppendLine("  </div>");

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
            sb.AppendLine(RenderEventNumberRow()); // Row 1
            sb.AppendLine(RenderEventTitleRow());  // Row 2
            sb.AppendLine(RenderEventDateRow());   // Row 3
            sb.AppendLine("</thead>");
            return sb.ToString();
        }

        private string RenderEventNumberRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            foreach (var (group, subs) in groupedFixedColumns)
            {
                if (subs.Count == 0)
                    sb.AppendLine($"<th rowspan=\"3\" class=\"fixed-column-title\">{WebUtility.HtmlEncode(group)}</th>");
                else
                    sb.AppendLine($"<th rowspan=\"2\" colspan=\"{subs.Count}\" class=\"fixed-column-title\">{WebUtility.HtmlEncode(group)}</th>");
            }

            foreach (var ev in calendar)
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                sb.AppendLine($"<th class=\"event-number {cssClass}\">{ev.EventNumber}</th>");
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderEventTitleRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            // Absolute start index for events in the body: Name(0), Rank(1,2), Events(3,4), Best8(5), Scoring11(6) => firstEventIndex = 7
            var colIndex = 7;
            foreach (var ev in calendar)
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                sb.AppendLine($"<th class=\"event-title {cssClass}\" data-col-index=\"{colIndex}\">{WebUtility.HtmlEncode(ev.EventName)}</th>");
                colIndex++;
            }

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private string RenderEventDateRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tr>");

            // Emit leaf sub-headers with absolute indices
            // Name doesn’t need a sub-header; it spans 3 rows.
            // Current rank: Competitin(1), Tens(2)
            sb.AppendLine("<th data-col-index=\"1\">Competition</th>");
            sb.AppendLine("<th data-col-index=\"2\">Tens</th>");

            // Events completed: Tens(3), Other(4)
            sb.AppendLine("<th data-col-index=\"3\">Tens</th>");
            sb.AppendLine("<th data-col-index=\"4\">Non-tens</th>");

            // Scoring 11 (5) and Best 8 (6) have no sub-header cells; they’re already spanning all rows in row 1.

            foreach (var ev in calendar)
            {
                var cssClass = ev.IsEvening10 ? "ten-mile-event" : "non-ten-mile-event";
                // event date cells don’t need data-col-index; event titles already carry it
                sb.AppendLine($"<th class=\"event-date {cssClass}\">{ev.EventDate:ddd dd/MM/yy}</th>");
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
            // Name
            yield return result.Competitor.FullName;

            // Current rank split
            yield return result.FullCompetition.RankDisplay;   // Full
            yield return result.TenMileCompetition.RankDisplay; // Tens

            // Events completed split
            yield return result.EventsCompletedTens.ToString();   // Tens
            yield return result.EventsCompletedOther.ToString();  // Other

            // Scoring 11
            yield return result.FullCompetition.PointsDisplay;
            
            // Best 8
            yield return result.TenMileCompetition.PointsDisplay;

            // Per-event columns
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

        protected virtual string RenderCell(string value, int index, Competitor competitor)
        {
            var encodedValue = WebUtility.HtmlEncode(value);

            const int nameIndex = 0;
            const int rankFullIndex = 1;
            const int rankTensIndex = 2;
            const int eventsTensIndex = 3;
            const int eventsOtherIndex = 4;
            const int scoring11Index = 5;
            const int best8Index = 6;
            const int firstEventIndex = 7;

            return index switch
            {
                nameIndex => RenderNameCell(competitor, encodedValue),
                rankFullIndex => RenderRankFullCell(encodedValue),
                rankTensIndex => RenderRankTensCell(encodedValue),
                eventsTensIndex => RenderEventsTensCell(encodedValue),
                eventsOtherIndex => RenderEventsOtherCell(encodedValue),
                scoring11Index => RenderScoring11Cell(encodedValue, competitor),
                best8Index => RenderBest8Cell(encodedValue, competitor),
                _ => RenderEventCell(encodedValue, calendar[index - firstEventIndex])
            };
        }

        protected virtual string RenderNameCell(Competitor competitor, string encodedValue)
            => $"<td class=\"competitor-name\">{encodedValue}</td>";

        protected virtual string RenderRankFullCell(string encodedValue)
            => $"<td class=\"rank-full\">{encodedValue}</td>";

        protected virtual string RenderRankTensCell(string encodedValue)
            => $"<td class=\"rank-tens\">{encodedValue}</td>";

        protected virtual string RenderEventsTensCell(string encodedValue)
            => $"<td class=\"events-tens\">{encodedValue}</td>";

        protected virtual string RenderEventsOtherCell(string encodedValue)
            => $"<td class=\"events-other\">{encodedValue}</td>";

        protected virtual string RenderScoring11Cell(string encodedValue, Competitor competitor)
        {
            var podiumClass = GetPodiumClassForScoring11(competitor);
            var scoring11CssClass = string.IsNullOrEmpty(podiumClass)
                ? "scoring-11"
                : $"scoring-11 {podiumClass}";
            return $"<td class=\"{scoring11CssClass}\">{encodedValue}</td>";
        }

        protected virtual string RenderBest8Cell(string encodedValue, Competitor competitor)
        {
            var podiumClass = GetPodiumClassForBest8(competitor);
            var best8CssClass = string.IsNullOrEmpty(podiumClass)
                ? "best-8"
                : $"best-8 {podiumClass}";
            return $"<td class=\"{best8CssClass}\">{encodedValue}</td>";
        }

        protected virtual string RenderEventCell(string encodedValue, CalendarEvent ev)
        {
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
