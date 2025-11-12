using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;

namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<BaseResults> resultsGenerators = new();

        public ResultsOrchestrator(List<CalendarEvent> eventCalendar, List<Ride> rides)
        {
            // Discover all event numbers dynamically
            var eventNumbers = eventCalendar.Select(e => e.EventNumber);
            foreach (var e in eventNumbers)
                resultsGenerators.Add(new EventResults(e, eventCalendar, rides));

            // Later: competitions auto‑discovered via reflection

        }

        public void GenerateAll()
        {
            StylesWriter.EnsureStylesheet(OutputLocator.GetOutputDirectory());

            foreach (var generator in resultsGenerators)
            {
                var table = generator.CreateTable();
                var html = ResultsRenderer.RenderAsHtml(table);
                var outputDir = OutputLocator.GetOutputDirectory();
                var folderPath = Path.Combine(outputDir, generator.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{generator.FileName}.html"), html);
            }
        }

        public void GenerateIndex()
        {
            var eventResults = resultsGenerators
                .OfType<EventResults>()   // filters only EventResults
                .OrderBy(ev => ev.EventDate) // optional: sort by date
                .ToList();

            var outputDir = OutputLocator.GetOutputDirectory();
            var indexRenderer = new SiteIndexRenderer(eventResults, outputDir);
            indexRenderer.RenderIndex();
        }
    }
}
