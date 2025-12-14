using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Utilities;


namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<ResultsSet> resultsSets = new();

        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<Competitor> competitors;   
        private readonly IEnumerable<CalendarEvent> calendar;

        private readonly ICompetitionRulesProvider rulesProvider;
        private readonly ICompetitionRules rules;

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

            // Determine competition year from first event
            var competitionYear = calendar.First().EventDate.Year;

            // Construct provider once
            var configDir = FolderLocator.GetConfigDirectory();
            var configFilePath = Path.Combine(configDir, "competition-rules.json");
            rulesProvider = new CompetitionRulesProvider(configFilePath);

            // Resolve rules for this season
            rules = rulesProvider.GetRules(competitionYear, calendar);

            InitializeResultsSets();
        }

        private void InitializeResultsSets()
        {
            foreach (var ev in calendar)
                resultsSets.Add(EventResultsSet.CreateFrom(calendar, rides, ev.EventNumber));

            var championshipRides = GetChampionshipRides(rides, calendar);
            var championshipCalendar = GetChampionshipCalendar(calendar);

            resultsSets.Add(SeniorsCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(VeteransCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(WomenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(JuniorsCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(JuvenilesCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(RoadBikeMenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(RoadBikeWomenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));

            // League competitions
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.Premier, championshipRides, championshipCalendar, rules));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League1, championshipRides, championshipCalendar, rules));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League2, championshipRides, championshipCalendar, rules));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League3, championshipRides, championshipCalendar, rules));
            resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League4, championshipRides, championshipCalendar, rules));

            // Nev Brooks
            resultsSets.Add(NevBrooksCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
        }

        private static IEnumerable<Ride> GetChampionshipRides(
            IEnumerable<Ride> rides,
            IEnumerable<CalendarEvent> calendar)
        {
            if (rides == null) throw new ArgumentNullException(nameof(rides));
            if (calendar == null) throw new ArgumentNullException(nameof(calendar));

            // Build a fast lookup of championship event numbers
            var championshipEventNumbers = new HashSet<int>(
                calendar.Where(ev => ev.IsClubChampionship)
                        .Select(ev => ev.EventNumber));

            // Only keep rides whose event is marked as championship
            return rides.Where(r => championshipEventNumbers.Contains(r.EventNumber));
        }

        private static IEnumerable<CalendarEvent> GetChampionshipCalendar(
            IEnumerable<CalendarEvent> fullCalendar)
        {
            if (fullCalendar == null) throw new ArgumentNullException(nameof(fullCalendar));

            return fullCalendar.Where(ev => ev.IsClubChampionship);
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

                current.PrevLabel = prev.LinkText;
                current.NextLabel = next.LinkText;
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
                var renderer = CompetitionRendererFactory.Create(resultsSet, calendar, rules);

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
