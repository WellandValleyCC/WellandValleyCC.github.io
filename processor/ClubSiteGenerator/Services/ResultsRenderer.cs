using ClubSiteGenerator.Models;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public static class ResultsRenderer
    {
        public static string RenderAsHtml(HtmlTable table, string eventTitle)
        {
            var sb = new StringBuilder();

            // Document scaffold
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine($"  <title>{WebUtility.HtmlEncode(eventTitle)}</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"../assets/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header + nav
            sb.AppendLine("<header>");
            sb.AppendLine($"  <h1>{WebUtility.HtmlEncode(eventTitle)}</h1>");
            sb.AppendLine("  <nav><a href=\"../preview.html\">Index</a></nav>");
            sb.AppendLine("</header>");

            // Results table
            sb.AppendLine("<table class=\"results\">");

            sb.AppendLine("<thead><tr>");
            foreach (var h in table.Headers)
                sb.AppendLine($"<th>{WebUtility.HtmlEncode(h)}</th>");
            sb.AppendLine("</tr></thead>");

            sb.AppendLine("<tbody>");
            foreach (var row in table.Rows)
            {
                string cssClass = GetRowClass(row);

                sb.AppendLine($"<tr class=\"{cssClass}\">");
                foreach (var cell in row.Cells)
                    sb.AppendLine($"<td>{WebUtility.HtmlEncode(cell)}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Close document
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        private static string GetRowClass(HtmlRow row)
        {
            return row.Ride.Competitor?.ClaimStatus switch
            {
                ClubCore.Models.Enums.ClaimStatus.Honorary => "claim-first",
                ClubCore.Models.Enums.ClaimStatus.FirstClaim => "claim-first",
                ClubCore.Models.Enums.ClaimStatus.SecondClaim => "claim-second",
                null => "guest",
                _ => string.Empty,
            };
        }
    }
}
