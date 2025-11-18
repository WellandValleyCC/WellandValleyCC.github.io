using ClubCore.Models;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;

namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<ResultsSet> resultsGenerators = new();

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
                resultsGenerators.Add(new EventResultsSet(e, eventsCalendar, rides));
            
            // Later: competitions auto‑discovered via reflection
        }

        public void GenerateAll()
        {
            StylesWriter.EnsureStylesheet(OutputLocator.GetOutputDirectory());

            var totalEvents = resultsGenerators.OfType<EventResultsSet>().Count();

            foreach (var generator in resultsGenerators.OfType<EventResultsSet>())
            {
                var table = generator.CreateTable();
                var renderer = new EventRenderer(
                    table,
                    generator.DisplayName,
                    generator.EventNumber,
                    totalEvents,
                    generator.EventDate,
                    generator.CalendarEvent.Miles);
                var html = renderer.Render();
                var outputDir = OutputLocator.GetOutputDirectory();
                var folderPath = Path.Combine(outputDir, generator.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{generator.FileName}.html"), html);
            }
        }

        public void GenerateIndex()
        {
            var eventResults = resultsGenerators
                .OfType<EventResultsSet>()   // filters only EventResults
                .OrderBy(ev => ev.EventDate) // optional: sort by date
                .ToList();

            var outputDir = OutputLocator.GetOutputDirectory();
            var indexRenderer = new SiteIndexRenderer(eventResults, outputDir);
            indexRenderer.RenderIndex();
        }
    }
}
