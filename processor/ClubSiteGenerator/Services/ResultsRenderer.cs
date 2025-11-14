using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public static class ResultsRenderer
    {
        public static string RenderAsHtml(
            HtmlTable table,
            string eventTitle,
            int eventNumber,
            int totalEvents,
            DateOnly eventDate,
            double eventMiles)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine($"  <title>Event {eventNumber}: {WebUtility.HtmlEncode(eventTitle)}</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"../assets/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header with title, date, and navigation
            sb.AppendLine("<header>");
            sb.AppendLine($"  <h1><span class=\"event-number\">Event {eventNumber}:</span> {WebUtility.HtmlEncode(eventTitle)}</h1>");
            sb.AppendLine($"  <p class=\"event-date\">{eventDate:dddd, dd MMMM yyyy}</p>");
            sb.AppendLine($"  <p class=\"event-distance\">Distance: {eventMiles:0.##} miles</p>");

            int prev = eventNumber == 1 ? totalEvents : eventNumber - 1;
            int next = eventNumber == totalEvents ? 1 : eventNumber + 1;

            sb.AppendLine("  <nav class=\"event-nav\">");
            sb.AppendLine($"    <a class=\"prev\" href=\"event-{prev:D2}.html\" aria-label=\"Previous event\">Previous</a>");
            sb.AppendLine("    <a class=\"index\" href=\"../preview.html\" aria-label=\"Back to index\">Index</a>");
            sb.AppendLine($"    <a class=\"next\" href=\"event-{next:D2}.html\" aria-label=\"Next event\">Next</a>");

            sb.AppendLine("  </nav>");

            sb.AppendLine("</header>");

            sb.AppendLine("<div class=\"legend\">");
            sb.AppendLine("  <span class=\"competition-eligible\">Competition eligible club member</span>");
            sb.AppendLine("  <span class=\"guest-second-claim\">2nd claim</span>");
            sb.AppendLine("  <span class=\"guest-non-club-member\">Guest</span>");
            sb.AppendLine("</div>");

            // Results table
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

                    if (!string.IsNullOrEmpty(tdClass))
                        sb.AppendLine($"<td class=\"{tdClass}\">{cellValue}</td>");
                    else
                        sb.AppendLine($"<td>{cellValue}</td>");
                }

                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Close document
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

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
