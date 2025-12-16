using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Utilities;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers
{
    internal class NevBrooksCompetitionRenderer : CompetitionRenderer
    {
        private readonly CompetitionResultsSet resultsSet;
        private readonly IReadOnlyList<CalendarEvent> calendar;
        private readonly string competitionTitle;

        protected override string PageTypeClass => "competition-page";

        public NevBrooksCompetitionRenderer(
            string indexFileName, CompetitionResultsSet resultsSet, ICompetitionRules rules)
            :base(indexFileName, resultsSet, rules)
        {
            this.resultsSet = resultsSet;
            this.calendar = resultsSet.ClubChampionshipCalendar.OrderBy(ev => ev.EventNumber).ToList();
            this.competitionTitle = resultsSet.DisplayName;
        }

        protected override IReadOnlyList<(string GroupTitle, IReadOnlyList<string> SubTitles)> GroupedFixedColumns =>
            new (string, IReadOnlyList<string>)[]
            {
                // no sub-columns in the Nev Brooks competition
                ("Name", Array.Empty<string>()),
                ("Current rank", Array.Empty<string>()),
                ("Events completed", Array.Empty<string>()),
                (Rules.TenMileShortTitle, Array.Empty<string>())
            };

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

            sb.AppendLine("<div class=\"rules-and-legend\">");

            sb.AppendLine("  <section class=\"competition-rules\">");
            sb.AppendLine("    <p>");
            sb.AppendLine($"      {WebUtility.HtmlEncode(resultsSet.EligibilityStatement)}");
            sb.AppendLine("      This is a handicapped competition using your times from previous ten-mile events to establish your individual handicap");
            sb.AppendLine("      <br/>");
            sb.AppendLine($"      {Rules.RuleTextTensCompetition}");
            sb.AppendLine("    </p>");
            sb.AppendLine("  </section>");

            sb.AppendLine("  <div class=\"legend\">");
            sb.AppendLine("    <span class=\"ten-mile-event\">10‑mile events</span>");
            sb.AppendLine("  </div>");

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        protected override IEnumerable<string> BuildCells(CompetitorResult result)
        {
            // Name
            yield return result.Competitor.FullName;

            // Current rank - Nev Brooks handicapped tens
            yield return result.TenMileCompetition.RankDisplay; // Tens

            // Events completed split
            yield return result.EventsCompletedTens.ToString();   // Tens
            
            // Best n Tens, total points
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

        protected override string RenderCell(string value, int index, Competitor competitor)
        {
            var encodedValue = WebUtility.HtmlEncode(value);

            // fixed column indices for Nev Brooks
            const int nameIndex = 0;
            const int rankTensIndex = 1;
            const int numEventsTensIndex = 2;
            const int pointsTensCompetitionIndex = 3;

            if (index < FirstEventIndex)
            {
                return index switch
                {
                    nameIndex => RenderNameCell(competitor, encodedValue),
                    rankTensIndex => RenderRankTensCell(encodedValue),
                    numEventsTensIndex => RenderEventsTensCell(encodedValue),
                    pointsTensCompetitionIndex => RenderTensCompetitionPointsCell(encodedValue, competitor),
                    _ => throw new InvalidOperationException($"Unexpected fixed column index {index}")
                };
            }

            // event columns: offset by FirstEventIndex
            return RenderEventCell(encodedValue, calendar[index - FirstEventIndex]);
        }
    }
}

