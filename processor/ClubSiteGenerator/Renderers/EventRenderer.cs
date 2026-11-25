using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Models.Extensions;
using ClubSiteGenerator.ResultsGenerator;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers
{
    public class EventRenderer : HtmlRendererBase
    {
        private readonly EventResultsSet resultsSet;
        private readonly string eventTitle;
        private readonly int eventNumber;
        private readonly DateOnly eventDate; 
        private readonly double eventMiles;
        private readonly bool isCancelled;
        private readonly bool isPostponed;
        private readonly bool isStandAloneEvent;

        protected override string PageTypeClass => "event-page";

        public EventRenderer(string indexFileName, EventResultsSet resultsSet)
            : base(indexFileName)
        {
            this.resultsSet = resultsSet;

            this.eventTitle = resultsSet.DisplayName;
            this.eventNumber = resultsSet.EventNumber;
            this.eventDate = resultsSet.EventDate;
            this.eventMiles = resultsSet.CalendarEvent.Miles;
            this.isCancelled = resultsSet.CalendarEvent.IsCancelled;
            this.isPostponed = resultsSet.CalendarEvent.IsPostponed;
            this.isStandAloneEvent = !resultsSet.CalendarEvent.IsClubChampionship;
        }

        internal readonly List<string> columnTitles = new()
        {
            "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
        };

        protected override string TitleElement() => isStandAloneEvent
            ? $"<title>{eventDate:dd/MM/yyyy}: {WebUtility.HtmlEncode(eventTitle)}</title>"
            : $"<title>Event {eventNumber}: {WebUtility.HtmlEncode(eventTitle)}</title>";

        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<div class=\"header-and-legend\">");

            // Banner area: event header + nav
            sb.AppendLine("<div class=\"wvcc-banner-header\">");

            var headerClasses = "event-header-core";

            if (isStandAloneEvent)
                headerClasses += " stand-alone-event";

            if (isCancelled)
                headerClasses += " cancelled-event";

            if (isPostponed)
                headerClasses += " postponed-event";

            sb.AppendLine($"  <div class=\"{headerClasses}\">");

            if (isStandAloneEvent)
            {
                sb.AppendLine($"    <h1>{WebUtility.HtmlEncode(eventTitle)}</h1>");
            }
            else
            {
                sb.AppendLine(
                    $"    <h1><span class=\"event-number\">Event {eventNumber}:</span> {WebUtility.HtmlEncode(eventTitle)}</h1>");
            }

            sb.AppendLine($"    <p class=\"event-date\">{eventDate:dddd, dd MMMM yyyy}</p>");
            sb.AppendLine($"    <p class=\"event-distance\">Distance: {eventMiles:0.##} miles</p>");
            sb.AppendLine("  </div>");

            sb.AppendLine(RenderNav(resultsSet));

            sb.AppendLine("</div>"); // end banner

            // Legend outside banner
            sb.AppendLine("  <div class=\"legend\">");
            sb.AppendLine("    <span class=\"competition-eligible\">Full member</span>");
            sb.AppendLine("    <span class=\"guest-second-claim\">2nd claim</span>");
            sb.AppendLine("    <span class=\"guest-non-club-member\">Guest</span>");
            sb.AppendLine("  </div>");

            sb.AppendLine("</div>"); // end header-and-legend

            return sb.ToString();
        }


        private string RenderNav(EventResultsSet resultsSet)
        {
            var sb = new StringBuilder();
            sb.AppendLine("  <nav class=\"event-nav\" aria-label=\"Event navigation\">");

            if (!string.IsNullOrWhiteSpace(resultsSet.PrevLink))
            {
                sb.AppendLine($"    <a class=\"prev\" href=\"{resultsSet.PrevLink}\" aria-label=\"Previous\">{resultsSet.PrevLabel}</a>");
            }

            sb.AppendLine($"    <a class=\"index\" href=\"../{IndexFileName}\" aria-current=\"page\" aria-label=\"Back to index\">Index</a>");

            if (!string.IsNullOrWhiteSpace(resultsSet.NextLink))
            {
                sb.AppendLine($"    <a class=\"next\" href=\"{resultsSet.NextLink}\" aria-label=\"Next\">{resultsSet.NextLabel}</a>");
            }

            sb.AppendLine("  </nav>");
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
            sb.AppendLine("<thead><tr>");

            foreach (var title in columnTitles)
                sb.AppendLine($"<th>{WebUtility.HtmlEncode(title)}</th>");

            sb.AppendLine("</tr></thead>");
            return sb.ToString();
        }

        private string RenderBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tbody>");

            bool anyFractional = resultsSet.Rides.Any(r =>
            {
                double ts = r.TotalSeconds;
                return ts % 1 != 0;   // true if fractional
            });

            int startNumber = 1;

            foreach (var ride in resultsSet.Rides)
            {
                sb.AppendLine(RenderRow(ride, startNumber, anyFractional));
                startNumber++;
            }

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(
            Ride ride,
            int startNumber,
            bool showFractionalSeconds)
        {
            var sb = new StringBuilder();
            var cssClass = GetRowClass(ride);

            sb.AppendLine($"<tr class=\"{cssClass}\">");

            sb.AppendLine(RenderCell(
                BuildNameCell(ride, startNumber),
                0,
                ride));

            sb.AppendLine(RenderCell(
                BuildPositionCell(ride),
                1,
                ride));

            sb.AppendLine(RenderCell(
                BuildRoadBikeCell(ride),
                2,
                ride));

            sb.AppendLine(RenderTimeCell(
                ride,
                showFractionalSeconds));

            sb.AppendLine(RenderAverageSpeedCell(
                ride));

            sb.AppendLine("</tr>");

            return sb.ToString();
        }

        private string BuildNameCell(
            Ride ride,
            int startNumber)
        {
            string riderName = ride.Name ?? "Unknown";

            return ride.Status == RideStatus.Ready
                ? $"{startNumber}. {riderName}"
                : riderName;
        }

        private string BuildPositionCell(Ride ride)
        {
            return ride.Status == RideStatus.Valid
                ? ride.EventRank?.ToString() ?? ""
                : "";
        }

        private string BuildRoadBikeCell(Ride ride)
        {
            return ride.Status == RideStatus.Valid
                ? ride.EventRoadBikeRank?.ToString() ?? ""
                : ride.IsRoadBike ? "R" : "";
        }

        private string RenderAverageSpeedCell(Ride ride)
        {
            string content = ride.Status switch
            {
                RideStatus.DNF => "",
                RideStatus.DNS => "",
                RideStatus.DQ => "",

                RideStatus.Ready => "",

                RideStatus.Valid => RenderValidAverageSpeedCellContent(ride),

                _ => ""
            };

            return $"<td>{content}</td>";
        }

        private string RenderValidAverageSpeedCellContent(Ride ride)
        {
            if (ride.CalendarEvent?.IsTwoLegEvent != true)
            {
                return ride.AvgSpeed?.ToString("0.00") ?? "";
            }

            string leg1Speed = ride.Leg1AvgSpeed?.ToString("0.00") ?? "";
            string leg2Speed = ride.Leg2AvgSpeed?.ToString("0.00") ?? "";
            string totalSpeed = ride.AvgSpeed?.ToString("0.00") ?? "";

            string leg1Name = WebUtility.HtmlEncode(
                ride.CalendarEvent?.Leg1Name ?? string.Empty);

            string leg2Name = WebUtility.HtmlEncode(
                ride.CalendarEvent?.Leg2Name ?? string.Empty);

            return $"""
                <div class="two-leg-breakdown">
                    <span class="leg1-value" data-leg-name="{leg1Name}">L1: {leg1Speed}</span>
                    <span class="leg2-value" data-leg-name="{leg2Name}">L2: {leg2Speed}</span>
                    <span class="total-value">{totalSpeed}</span>
                </div>
                """;
        }

        private string RenderTimeCell(
            Ride ride,
            bool showFractionalSeconds)
        {
            string content = ride.Status switch
            {
                RideStatus.DNF => "DNF",
                RideStatus.DNS => "DNS",
                RideStatus.DQ => "DQ",

                RideStatus.Ready => "",

                RideStatus.Valid => RenderValidTimeCellContent(
                    ride,
                    showFractionalSeconds),

                _ => ""
            };

            return $"<td>{content}</td>";
        }

        private string RenderValidTimeCellContent(
            Ride ride,
            bool showFractionalSeconds)
        {
            if (ride.CalendarEvent?.IsTwoLegEvent != true)
            {
                return FormatTime(
                    ride.TotalSeconds,
                    showFractionalSeconds);
            }

            string leg1 = FormatTime(
                ride.Leg1TotalSeconds,
                showFractionalSeconds);

            string leg2 = FormatTime(
                ride.Leg2TotalSeconds,
                showFractionalSeconds);

            string total = FormatTime(
                ride.TotalSeconds,
                showFractionalSeconds);

            string leg1Name = WebUtility.HtmlEncode(
                ride.CalendarEvent?.Leg1Name ?? string.Empty);

            string leg2Name = WebUtility.HtmlEncode(
                ride.CalendarEvent?.Leg2Name ?? string.Empty);

            return $"""
                <div class="two-leg-breakdown">
                    <span class="leg1-value" data-leg-name="{leg1Name}">L1: {leg1}</span>
                    <span class="leg2-value" data-leg-name="{leg2Name}">L2: {leg2}</span>
                    <span class="total-value">{total}</span>
                </div>
                """;
        }

        private static string FormatTime(double totalSeconds, bool showFractionalSeconds)
        {
            var ts = TimeSpan.FromSeconds(totalSeconds);

            if (!showFractionalSeconds)
            {
                // Standard hh:mm:ss
                return ts.ToString(@"hh\:mm\:ss");
            }

            // hh:mm:ss.ff (always two decimals)
            double secondsWithFraction = ts.Seconds + ts.Milliseconds / 1000.0;
            string sec = secondsWithFraction.ToString("00.00");

            return $"{ts.Hours:00}:{ts.Minutes:00}:{sec}";
        }

        private string RenderCell(string value, int index, Ride ride)
        {
            var encoded = WebUtility.HtmlEncode(value);
            var tdClass = index switch
            {
                1 => ride.GetEventEligibleRidersRankClass(),
                2 => ride.GetEventEligibleRoadBikeRidersRankClass(),
                _ => string.Empty
            };

            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{encoded}</td>"
                : $"<td class=\"{tdClass}\">{encoded}</td>";
        }

        public static string GetRowClass(Ride ride) => ride switch
        {
            { EventEligibleRidersRank: not null } => "competition-eligible",
            { ClubNumber: null } => "guest-non-club-member",
            { Competitor.ClaimStatus: ClaimStatus.SecondClaim } => "guest-second-claim",
            _ => "competition-eligible"
        };
    }
}
