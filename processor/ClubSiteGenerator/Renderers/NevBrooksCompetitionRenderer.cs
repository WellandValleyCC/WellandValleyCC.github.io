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
            yield return result.TenMileCompetition.RankDisplay;

            // Events completed split
            yield return result.EventsCompletedTens.ToString();

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
                    // Did not ride
                    display = "-";
                    ride = null;
                }
                else
                {
                    display = status switch
                    {
                        RideStatus.Valid => points.HasValue
                            ? Math.Round(points.Value, MidpointRounding.AwayFromZero).ToString()
                            : "H",   // Handicap-establishing ride

                        RideStatus.DNS => "DNS",
                        RideStatus.DNF => "DNF",
                        RideStatus.DQ => "DQ",

                        _ => "-"
                    };

                    // For DNS/DNF/DQ, suppress ride object
                    if (status != RideStatus.Valid)
                        ride = null;
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

            var ride = cell.Ride;

            // No ride at all → simple "-"
            if (ride is null)
                return $"<td class=\"{cssClass}\">-</td>";

            bool isHandicapEstablishing = !ride.NevBrooksPoints.HasValue;

            string display = isHandicapEstablishing
                ? "H"
                : ride.NevBrooksPoints.Value.ToString("0");

            // Ride time in mm:ss
            var rideTime = ride.Time.HasValue
                ? $"{(int)ride.Time.Value.TotalMinutes}:{ride.Time.Value.Seconds:D2}"
                : "-";

            // Generated handicap
            var generated = ride.NevBrooksSecondsGenerated?.ToString("0") ?? "-";

            // Applied handicap (only for scoring rides)
            var applied = ride.NevBrooksSecondsApplied?.ToString("0") ?? "-";

            // Adjusted time (only for scoring rides)
            var adjustedTime = ride.NevBrooksSecondsAdjustedTime.HasValue
                ? FormatSecondsAsTime(ride.NevBrooksSecondsAdjustedTime.Value)
                : "-";

            // Build details panel
            var details = new StringBuilder();

            details.AppendLine($"<div>Ride: {rideTime}</div>");
            details.AppendLine("<div style=\"margin-top:0.25rem;\">Handicap</div>");
            details.AppendLine($"<div>&nbsp;&nbsp;Generated: {generated}s</div>");

            if (!isHandicapEstablishing)
            {
                details.AppendLine($"<div>&nbsp;&nbsp;Applied: {applied}s</div>");
                details.AppendLine($"<div style=\"margin-top:0.25rem;\">Adjusted: {adjustedTime}</div>");
            }

            return $@"
<td class=""{cssClass}"">
  <div class=""nb-cell collapsed"" onclick=""this.classList.toggle('expanded')"">
    <span class=""nb-points"">{WebUtility.HtmlEncode(display)}</span>
    <div class=""nb-details"">
      {details}
    </div>
  </div>
</td>";
        }


        string FormatSecondsAsTime(double seconds)
        {
            var ts = TimeSpan.FromSeconds(seconds);
            return $"{(int)ts.TotalMinutes}:{ts.Seconds:D2}";
        }
    }
}