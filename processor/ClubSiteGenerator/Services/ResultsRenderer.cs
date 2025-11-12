using ClubSiteGenerator.Models;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public static class ResultsRenderer
    {
        public static string RenderAsHtml(HtmlTable table)
        {
            var sb = new StringBuilder();

            // Document scaffold
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <title>Event Results</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"../assets/csv/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Results table
            sb.AppendLine("<table class=\"results\">");

            sb.AppendLine("<thead><tr>");
            foreach (var h in table.Headers)
                sb.AppendLine($"<th>{WebUtility.HtmlEncode(h)}</th>");
            sb.AppendLine("</tr></thead>");

            sb.AppendLine("<tbody>");
            foreach (var row in table.Rows)
            {
                sb.AppendLine("<tr>");
                foreach (var cell in row)
                    sb.AppendLine($"<td>{WebUtility.HtmlEncode(cell)}</td>");
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody></table>");

            // Close document
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
