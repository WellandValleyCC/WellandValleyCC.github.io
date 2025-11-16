using ClubSiteGenerator.ResultsGenerator;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public class SiteIndexRenderer
    {
        private readonly IEnumerable<EventResultsSet> events;
        private readonly string outputDir;

        public SiteIndexRenderer(IEnumerable<EventResultsSet> events, string outputDir)
        {
            this.events = events;
            this.outputDir = outputDir;
        }

        public void RenderIndex()
        {
            // Ensure output directory exists
            Directory.CreateDirectory(outputDir);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <title>Season Index</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"assets/csv/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Season Overview</h1>");
            sb.AppendLine("<ul>");

            foreach (var ev in events)
            {
                var fileName = $"event-{ev.EventNumber:D2}.html"; // two‑digit formatting
                sb.AppendLine($"  <li><a href=\"events/{fileName}\">TT{ev.EventNumber:D2} – {ev.EventDate:yyyy-MM-dd}</a></li>");
            }

            sb.AppendLine("</ul>");
            sb.AppendLine("</body></html>");

            var path = Path.Combine(outputDir, "preview.html");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
