using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;

namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<ResultsSet> resultsSets = new();

        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<Competitor> competitors;   
        private readonly IEnumerable<CalendarEvent> calendar;

        public ResultsOrchestrator(
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar)
        {
            this.rides = rides;
            this.competitors = competitors;
            this.calendar = calendar;

            InitializeResultsSets();
        }



        private void InitializeResultsSets()
        {
            foreach (var ev in calendar)
                resultsSets.Add(EventResultsSet.CreateFrom(ev, rides));

            // Later: competitions auto‑discovered via reflection
        }

        public void GenerateAll()
        {
            StylesWriter.EnsureStylesheet(OutputLocator.GetOutputDirectory());

            var totalEvents = resultsSets.OfType<EventResultsSet>().Count();

            foreach (var generator in resultsSets.OfType<EventResultsSet>())
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
            var eventResults = resultsSets
                .OfType<EventResultsSet>()   // filters only EventResults
                .OrderBy(ev => ev.EventDate) // optional: sort by date
                .ToList();

            var outputDir = OutputLocator.GetOutputDirectory();
            var indexRenderer = new SiteIndexRenderer(eventResults, outputDir);
            indexRenderer.RenderIndex();
        }
    }
}
