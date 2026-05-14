using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
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
            : base(indexFileName, resultsSet, rules)
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
                ("Rank", Array.Empty<string>()),
                ("Events completed", Array.Empty<string>()),
                (Rules.TenMileShortTitle, Array.Empty<string>())
            };

        protected override string TitleElement()
            => $"<title>{WebUtility.HtmlEncode(competitionTitle)}</title>";

        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();

            // Banner area: title + nav
            sb.AppendLine("<div class=\"wvcc-banner-header\">");

            sb.AppendLine($"  <h1>{WebUtility.HtmlEncode(competitionTitle)}</h1>");

            sb.AppendLine("  <nav class=\"competition-nav\" aria-label=\"Competition navigation\">");
            sb.AppendLine($"    <a class=\"prev\" href=\"{resultsSet.PrevLink}\" aria-label=\"Previous\">{resultsSet.PrevLabel}</a>");
            sb.AppendLine($"    <a class=\"index\" href=\"../{IndexFileName}\" aria-current=\"page\" aria-label=\"Back to index\">Index</a>");
            sb.AppendLine($"    <a class=\"next\" href=\"{resultsSet.NextLink}\" aria-label=\"Next\">{resultsSet.NextLabel}</a>");
            sb.AppendLine("  </nav>");

            sb.AppendLine("</div>"); // end banner

            // Rules + legend (outside banner)
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

        protected override IEnumerable<object> BuildCells(CompetitorResult result)
        {
            // Name
            yield return result.Competitor.FullName;

            // Rank - Nev Brooks handicapped tens
            yield return result.TenMileCompetition.RankDisplay; // Tens

            // Events completed split
            yield return result.EventsCompletedTens.ToString();   // Tens

            // Best n Tens, total points
            yield return result.TenMileCompetition.PointsDisplay;

            // Per-event columns
            foreach (var ev in calendar.OrderBy(e => e.EventNumber))
            {
                result.EventStatuses.TryGetValue(ev.EventNumber, out var status);
                result.EventPoints.TryGetValue(ev.EventNumber, out var points);

                // Find the ride (may be null)
                var ride = result.Rides.FirstOrDefault(r => r.EventNumber == ev.EventNumber);

                string display;

                if (!result.EventStatuses.ContainsKey(ev.EventNumber))
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

                yield return new NevBrooksCell
                {
                    Display = display,
                    Ride = ride
                };
            }
        }

        protected override string RenderCell(object cellValue, int index, Competitor competitor)
        {
            // Fixed columns → base behaviour
            if (index < FirstEventIndex)
                return base.RenderCell(cellValue, index, competitor);

            var ev = calendar[index - FirstEventIndex];

            // Nev Brooks special case
            if (cellValue is NevBrooksCell nb)
                return RenderNevBrooksEventCell(nb, ev);

            // Fallback to base behaviour
            return base.RenderCell(cellValue, index, competitor);
        }

        private string RenderNevBrooksEventCell(NevBrooksCell cell, CalendarEvent ev)
        {
            const string cssClass = "ten-mile-event";

            // Simple cell for "-", DNS, DNF, DQ, or no ride object
            if (cell.Ride is null ||
                string.IsNullOrEmpty(cell.Display) ||
                cell.Display == "-")
            {
                return $"<td class=\"{cssClass}\">{WebUtility.HtmlEncode(cell.Display)}</td>";
            }

            var ride = cell.Ride;

            // Raw seconds values
            var totalSeconds = ride.TotalSeconds.ToString("0");
            var generated = ride.NevBrooksSecondsGenerated?.ToString("0") ?? "0";
            var applied = ride.NevBrooksSecondsApplied?.ToString("0") ?? "0";
            var adjusted = ride.NevBrooksSecondsAdjustedTime?.ToString("0") ?? "-";

            var rideTime = ride.Time.HasValue
    ? $"{ride.Time.Value.Minutes}:{ride.Time.Value.Seconds:D2}"
    : "-";

            var adjustedTime = ride.NevBrooksSecondsAdjustedTime.HasValue
                ? FormatSecondsAsTime(ride.NevBrooksSecondsAdjustedTime.Value)
                : "-";

            return $@"
<td class=""{cssClass}"">
  <div class=""nb-cell collapsed"" onclick=""this.classList.toggle('expanded')"">
    <span class=""nb-points"">{WebUtility.HtmlEncode(cell.Display)}</span>

    <div class=""nb-details"">
      <div>Ride: {rideTime}</div>

      <div style=""margin-top:0.25rem;"">Handicap</div>
      <div>&nbsp;&nbsp;Generated: {generated}s</div>
      <div>&nbsp;&nbsp;Applied: {applied}s</div>

      <div style=""margin-top:0.25rem;"">Adjusted: {adjustedTime}</div>
    </div>
  </div>
</td>";

        }

        string FormatSecondsAsTime(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
        }

        protected override string RenderEventCell(string encodedValue, CalendarEvent ev)
        {
            // All Nev Brooks events are ten-mile events
            const string cssClass = "ten-mile-event";

            // If no score, keep the simple cell
            if (string.IsNullOrEmpty(encodedValue) || encodedValue == "-")
                return $"<td class=\"{cssClass}\">{encodedValue}</td>";

            // Build the expandable details panel
            var detailsHtml = BuildNevBrooksDetailsHtml(ev);

            return $@"
<td class=""{cssClass}"">
  <div class=""nb-cell collapsed"" onclick=""this.classList.toggle('expanded')"">
    <span class=""nb-points"">{encodedValue}</span>
    <div class=""nb-details"">
      {detailsHtml}
    </div>
  </div>
</td>";
        }

        private string BuildNevBrooksDetailsHtml(CalendarEvent ev)
        {
            // You will replace this with real data from your result model
            var sb = new StringBuilder();

            sb.AppendLine("<div class=\"nb-detail-line\">Handicap used: TODO</div>");
            sb.AppendLine("<div class=\"nb-detail-line\">Generated seconds: TODO</div>");
            sb.AppendLine("<div class=\"nb-detail-line\">Applied seconds: TODO</div>");
            sb.AppendLine("<div class=\"nb-detail-line\">Adjusted time: TODO</div>");

            return sb.ToString();
        }

    }
}

