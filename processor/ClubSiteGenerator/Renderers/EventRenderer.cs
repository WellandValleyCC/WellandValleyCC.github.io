using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers
{
    public class EventRenderer : HtmlRendererBase
    {
        private readonly HtmlTable table;
        private readonly string eventTitle;
        private readonly int eventNumber;
        private readonly int totalEvents;
        private readonly DateOnly eventDate; 
        private readonly double eventMiles;

        public EventRenderer(HtmlTable table,
            string eventTitle,
            int eventNumber,
            int totalEvents,
            DateOnly eventDate,
            double eventMiles)
        {
            this.table = table;
            this.eventTitle = eventTitle;
            this.eventNumber = eventNumber;
            this.totalEvents = totalEvents;
            this.eventDate = eventDate;
            this.eventMiles = eventMiles;
        }

        protected override string TitleElement() => $"<title>Event {eventNumber}: {WebUtility.HtmlEncode(eventTitle)}</title>";

        protected override string HeaderHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"  <h1><span class=\"event-number\">Event {eventNumber}:</span> {WebUtility.HtmlEncode(eventTitle)}</h1>");
            sb.AppendLine($"  <p class=\"event-date\">{eventDate:dddd, dd MMMM yyyy}</p>");
            sb.AppendLine($"  <p class=\"event-distance\">Distance: {eventMiles:0.##} miles</p>");

            int prev = eventNumber == 1 ? totalEvents : eventNumber - 1;
            int next = eventNumber == totalEvents ? 1 : eventNumber + 1;

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
            sb.AppendLine("  <span class=\"competition-eligible\">Competition eligible club member</span>");
            sb.AppendLine("  <span class=\"guest-second-claim\">2nd claim</span>");
            sb.AppendLine("  <span class=\"guest-non-club-member\">Guest</span>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        protected override string ResultsTableHtml()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<table class=\"results\">");

            sb.AppendLine("<thead><tr>");
            foreach (var h in table.Headers)
                sb.AppendLine($"<th>{WebUtility.HtmlEncode(h)}</th>");
            sb.AppendLine("</tr></thead>");

            sb.AppendLine("<tbody>");

            foreach (var row in table.Rows)
            {
                string cssClass = GetRowClass(row.Ride);

                sb.AppendLine($"<tr class=\"{cssClass}\">");

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    var cellValue = WebUtility.HtmlEncode(row.Cells[i]);
                    var tdClass = string.Empty;

                    if (i == 1)
                        tdClass = GetPodiumClass(row.Ride.EventEligibleRidersRank, row.Ride);

                    if (i == 2)
                        tdClass = GetPodiumClass(row.Ride.EventEligibleRoadBikeRidersRank, row.Ride);

                    sb.AppendLine(string.IsNullOrEmpty(tdClass)
                        ? $"<td>{cellValue}</td>"
                        : $"<td class=\"{tdClass}\">{cellValue}</td>");
                }

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table>");

            return sb.ToString();
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
