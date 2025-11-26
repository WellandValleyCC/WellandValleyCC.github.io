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
        

        /// <param name="rides">These rides have been hydrated - i.e. Competitors (where applicable) attached and CalendarEvent attached.</param>
        /// <param name="competitors"></param>
        /// <param name="calendar"></param>
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

            foreach (var resultsSet in resultsSets.OfType<EventResultsSet>())
            {
                var renderer = new EventRenderer(resultsSet, totalEvents);
                var html = renderer.Render();
                var outputDir = OutputLocator.GetOutputDirectory();
                var folderPath = Path.Combine(outputDir, resultsSet.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{resultsSet.FileName}.html"), html);
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
