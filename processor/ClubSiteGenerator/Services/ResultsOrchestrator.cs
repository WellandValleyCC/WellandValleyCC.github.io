using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;

namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<BaseResults> resultsGenerators = new();

        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<CalendarEvent> eventsCalendar;

        public ResultsOrchestrator(IEnumerable<Ride> rides,
                                   IEnumerable<CalendarEvent> eventCalendar)
        {
            this.rides = rides;
            //this.competitors = competitors;
            this.eventsCalendar = eventCalendar;

            InitializeGenerators();
        }

        private void InitializeGenerators()
        {
            // Discover all event numbers dynamically
            var eventNumbers = eventsCalendar.Select(e => e.EventNumber); 
            
            foreach (var e in eventNumbers) 
                resultsGenerators.Add(new EventResults(e, eventsCalendar, rides));
            
            // Later: competitions auto‑discovered via reflection
        }

        public void GenerateAll()
        {
            StylesWriter.EnsureStylesheet(OutputLocator.GetOutputDirectory());

            var totalEvents = resultsGenerators.OfType<EventResults>().Count();

            foreach (var generator in resultsGenerators.OfType<EventResults>())
            {
                var table = generator.CreateTable();
                var html = ResultsRenderer.RenderAsHtml(
                    table,
                    generator.DisplayName,
                    generator.EventNumber,
                    totalEvents,
                    generator.EventDate,
                    generator.CalendarEvent.Miles);

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
