using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.ResultsGenerator;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public class SiteIndexRenderer
    {
        private readonly IEnumerable<EventResultsSet> events;
        private readonly IEnumerable<CompetitionResultsSet> competitions;
        private readonly string outputDir;

        public SiteIndexRenderer(IEnumerable<EventResultsSet> events,
                                 IEnumerable<CompetitionResultsSet> competitions,
                                 string outputDir)
        {
            this.events = events;
            this.competitions = competitions;
            this.outputDir = outputDir;
        }

        public void RenderIndex()
        {
            Directory.CreateDirectory(outputDir);

            // Order events by EventNumber
            var orderedEvents = events.OrderBy(ev => ev.EventNumber).Cast<IResultsSet>().ToList();

            // Order competitions by fixed sequence
            var orderedCompetitions = competitions
                .OrderBy(c => Array.IndexOf(CompetitionOrder, c.CompetitionType))
                .Cast<IResultsSet>()
                .ToList();

            // Unified sequence
            var allResults = orderedEvents.Concat(orderedCompetitions).ToList();

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

            foreach (var rs in allResults)
            {
                sb.AppendLine($"  <li><a href=\"{rs.SubFolderName}/{rs.FileName}.html\">{rs.DisplayName}</a></li>");
            }

            sb.AppendLine("</ul>");

            sb.AppendLine("</body></html>");

            var path = Path.Combine(outputDir, "preview.html");
            File.WriteAllText(path, sb.ToString());
        }

        public static readonly string[] CompetitionOrder =
        {
            "Seniors", "Veterans", "Women", "Juniors", "Juveniles",
            "Road Bike Men", "Road Bike Women",
            "Premier", "League1", "League2", "League3", "League4",
            "NevBrooks"
        };
    }

}
