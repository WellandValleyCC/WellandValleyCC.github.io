using ClubCore.Models;
using ClubCore.Models.Enums;
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
        private readonly int numberOfEvents;
        private readonly DateOnly eventDate; 
        private readonly double eventMiles;

        public EventRenderer(EventResultsSet resultsSet, int numberOfEvents)
        {
            this.resultsSet = resultsSet;

            this.eventTitle = resultsSet.DisplayName;
            this.eventNumber = resultsSet.EventNumber;
            this.numberOfEvents = numberOfEvents;
            this.eventDate = resultsSet.EventDate;
            this.eventMiles = resultsSet.CalendarEvent.Miles;
        }

        internal readonly List<string> columnTitles = new()
        {
            "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
        };

        protected override string TitleElement() => $"<title>Event {eventNumber}: {WebUtility.HtmlEncode(eventTitle)}</title>";

        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"  <h1><span class=\"event-number\">Event {eventNumber}:</span> {WebUtility.HtmlEncode(eventTitle)}</h1>");
            sb.AppendLine($"  <p class=\"event-date\">{eventDate:dddd, dd MMMM yyyy}</p>");
            sb.AppendLine($"  <p class=\"event-distance\">Distance: {eventMiles:0.##} miles</p>");

            int prev = eventNumber == 1 ? numberOfEvents : eventNumber - 1;
            int next = eventNumber == numberOfEvents ? 1 : eventNumber + 1;

            sb.AppendLine("  <nav class=\"event-nav\" aria-label=\"Event navigation\">");
            sb.AppendLine($"    <a class=\"prev\" href=\"event-{prev:D2}.html\" aria-label=\"Previous event\">Previous</a>");
            sb.AppendLine("    <a class=\"index\" href=\"../preview.html\" aria-current=\"page\" aria-label=\"Back to index\">Index</a>");
            sb.AppendLine($"    <a class=\"next\" href=\"event-{next:D2}.html\" aria-label=\"Next event\">Next</a>");
            sb.AppendLine("  </nav>");

            return sb.ToString();
        }

        protected override string LegendHtml()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"legend\">");
            sb.AppendLine("  <span class=\"competition-eligible\">Full member</span>");
            sb.AppendLine("  <span class=\"guest-second-claim\">2nd claim</span>");
            sb.AppendLine("  <span class=\"guest-non-club-member\">Guest</span>");
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

            foreach (var ride in resultsSet.Rides)
                sb.AppendLine(RenderRow(ride));

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(Ride ride)
        {
            var sb = new StringBuilder();
            var cssClass = GetRowClass(ride);

            sb.AppendLine($"<tr class=\"{cssClass}\">");

            foreach (var cell in BuildCells(ride).Select((value, index) => RenderCell(value, index, ride)))
                sb.AppendLine(cell);

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private IEnumerable<string> BuildCells(Ride ride)
        {
            var timeCell = ride.Status switch
            {
                RideStatus.DNF => "DNF",
                RideStatus.DNS => "DNS",
                RideStatus.DQ => "DQ",
                _ => TimeSpan.FromSeconds(ride.TotalSeconds).ToString(@"hh\:mm\:ss")
            };

            yield return ride.Name ?? "Unknown";
            yield return ride.EventRank?.ToString() ?? "";
            yield return ride.EventRoadBikeRank?.ToString() ?? "";
            yield return timeCell;
            yield return ride.AvgSpeed?.ToString("0.00") ?? string.Empty;
        }

        private string RenderCell(string value, int index, Ride ride)
        {
            var encoded = WebUtility.HtmlEncode(value);
            var tdClass = index switch
            {
                1 => GetPodiumClass(ride.EventEligibleRidersRank, ride),
                2 => GetPodiumClass(ride.EventEligibleRoadBikeRidersRank, ride),
                _ => string.Empty
            };

            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{encoded}</td>"
                : $"<td class=\"{tdClass}\">{encoded}</td>";
        }

        public static string GetPodiumClass(int? rank, Ride ride)
        {
            return rank switch
            {
                1 => "position-1",
                2 => "position-2",
                3 => "position-3",
                _ => string.Empty
            };
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
