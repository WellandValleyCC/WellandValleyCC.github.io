using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Utilities;
using System.Text.Json;


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
                resultsSets.Add(EventResultsSet.CreateFrom(calendar, rides, ev.EventNumber));

            var dataDir = FolderLocator.GetDataDirectory();
            var rulesPath = Path.Combine(dataDir, "CompetitionRules.json");

            using var doc = JsonDocument.Parse(File.ReadAllText(rulesPath));
            var rulesProvider = new CompetitionRulesProvider(doc.RootElement);

            resultsSets.Add(SeniorsCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(VeteransCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(WomenCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(JuniorsCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(RoadBikeMenCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
            resultsSets.Add(RoadBikeWomenCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));


            // League competitions
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.Premier, rides, calendar, rulesProvider));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League1, rides, calendar, rulesProvider));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League2, rides, calendar, rulesProvider));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League3, rides, calendar, rulesProvider));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League4, rides, calendar, rulesProvider));

            // Nev Brooks
            resultsSets.Add(NevBrooksCompetitionResultsSet.CreateFrom(rides, calendar, rulesProvider));
        }

        public void GenerateAll()
        {
            StylesWriter.EnsureStylesheet(OutputLocator.GetOutputDirectory());

            // Build unified ordering and assign Prev/Next
            var orderedEvents = resultsSets.OfType<EventResultsSet>()
                .OrderBy(ev => ev.EventNumber)
                .Cast<IResultsSet>()
                .ToList();

            var orderedCompetitions = resultsSets.OfType<CompetitionResultsSet>()
                .OrderBy(comp => SiteIndexRenderer.CompetitionOrder
                    .ToList()
                    .IndexOf(comp.CompetitionType))
                .Cast<IResultsSet>()
                .ToList();

            var allResults = orderedEvents.Concat(orderedCompetitions).ToList();

            for (int i = 0; i < allResults.Count; i++)
            {
                var current = allResults[i];
                var prev = allResults[(i - 1 + allResults.Count) % allResults.Count];
                var next = allResults[(i + 1) % allResults.Count];

                current.PrevLink = $"../{prev.SubFolderName}/{prev.FileName}.html";
                current.NextLink = $"../{next.SubFolderName}/{next.FileName}.html";

                current.PrevLabel = prev.GenericName;
                current.NextLabel = next.GenericName;
            }

            foreach (var resultsSet in resultsSets.OfType<EventResultsSet>())
            {
                var renderer = new EventRenderer(resultsSet);
                Console.WriteLine($"Generating results for event: {resultsSet.FileName}");
                var html = renderer.Render();
                var outputDir = OutputLocator.GetOutputDirectory();
                var folderPath = Path.Combine(outputDir, resultsSet.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{resultsSet.FileName}.html"), html);
            }

            foreach (var resultsSet in resultsSets.OfType<CompetitionResultsSet>())
            {
                var renderer = CompetitionRendererFactory.Create(resultsSet, calendar);

                Console.WriteLine($"Generating results for competition: {resultsSet.FileName}");
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

            var competitionResults = resultsSets
                .OfType<CompetitionResultsSet>()
                .OrderBy(comp => SiteIndexRenderer.CompetitionOrder
                    .ToList()
                    .IndexOf(comp.CompetitionType))
                .ToList();

            var outputDir = OutputLocator.GetOutputDirectory();
            var indexRenderer = new SiteIndexRenderer(eventResults, competitionResults, outputDir);
            indexRenderer.RenderIndex();
        }
    }
}
