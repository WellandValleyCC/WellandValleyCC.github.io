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
    public class ClubResultsOrchestrator
    {
        private readonly List<ResultsSet> resultsSets = new();

        private readonly string outputDir;
        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<Competitor> competitors;
        private readonly IEnumerable<CalendarEvent> calendar;

        private readonly ICompetitionRulesProvider rulesProvider;
        private readonly ICompetitionRules rules;

        private readonly int competitionYear;

        public ClubResultsOrchestrator(
            string outputDir,
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar)
        {
            this.outputDir = outputDir;
            this.rides = rides;
            this.competitors = competitors;
            this.calendar = calendar;

            // Determine competition year from first event
            competitionYear = calendar.First().EventDate.Year;

            var folderLocator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            var configDir = folderLocator.GetConfigDirectory();
            var configFilePath = Path.Combine(configDir, "competition-rules.json");
            rulesProvider = new CompetitionRulesProvider(configFilePath);

            // Resolve rules for this season
            rules = rulesProvider.GetRules(competitionYear, calendar);
        }

        public void GenerateAll(string indexFileName)
        {
            PrepareAssets();
            InitializeResultsSets();
            WirePrevNextLinks();
            GeneratePages(indexFileName);
            GenerateIndex(indexFileName);
        }

        private void PrepareAssets()
        {
            StylesWriter.EnsureStylesheet(outputDir);
        }

        private void InitializeResultsSets()
        {
            // Always add event results
            foreach (var ev in calendar)
                resultsSets.Add(EventResultsSet.CreateFrom(calendar, rides, ev.EventNumber));

            var championshipRides = GetChampionshipRides(rides, calendar);
            var championshipCalendar = GetChampionshipCalendar(calendar);

            // Bail out if no championship events
            if (!championshipCalendar.Any())
                return;

            // Core championship competitions
            resultsSets.Add(SeniorsCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(VeteransCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(WomenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(JuniorsCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(JuvenilesCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(RoadBikeMenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            resultsSets.Add(RoadBikeWomenCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));

            // League competitions only if someone has a league assigned
            if (championshipRides.Any(r => r.Competitor != null && r.Competitor.League != League.Undefined))
            {
                resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.Premier, championshipRides, championshipCalendar, rules));
                resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League1, championshipRides, championshipCalendar, rules));
                resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League2, championshipRides, championshipCalendar, rules));
                resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League3, championshipRides, championshipCalendar, rules));
                resultsSets.Add(LeagueCompetitionResultsSet.CreateFrom(League.League4, championshipRides, championshipCalendar, rules));
            }

            // Nev Brooks only after the second 10‑mile TT with rides
            var tenMileEventsWithRides = championshipCalendar
                .Where(ev => ev.IsEvening10)
                .Select(ev => ev.EventNumber)
                .Distinct()
                .Where(evNum => championshipRides.Any(r => r.EventNumber == evNum))
                .ToList();

            if (tenMileEventsWithRides.Count >= 2)
            {
                resultsSets.Add(NevBrooksCompetitionResultsSet.CreateFrom(championshipRides, championshipCalendar, rules));
            }
        }

        private void WirePrevNextLinks()
        {
            var orderedEvents = resultsSets
                .OfType<EventResultsSet>()
                .OrderBy(ev => ev.EventNumber)
                .Cast<IResultsSet>()
                .ToList();

            var orderedCompetitions = resultsSets
                .OfType<CompetitionResultsSet>()
                .OrderBy(comp => SiteIndexRenderer.CompetitionOrder
                    .ToList()
                    .IndexOf(comp.CompetitionType))
                .Cast<IResultsSet>()
                .ToList();

            var allResults = orderedEvents.Concat(orderedCompetitions).ToList();

            // If 0 or 1 results, explicitly clear Prev/Next
            if (allResults.Count <= 1)
            {
                if (allResults.Count == 1)
                {
                    var single = allResults[0];
                    single.PrevLink = null;
                    single.NextLink = null;
                    single.PrevLabel = null;
                    single.NextLabel = null;
                }

                return;
            }

            // Wire circular Prev/Next for 2+ results
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
        }

        private void GeneratePages(string indexFileName)
        {
            foreach (var resultsSet in resultsSets.OfType<EventResultsSet>())
            {
                var renderer = new EventRenderer(indexFileName, resultsSet);
                Console.WriteLine($"Generating results for event: {resultsSet.FileName}");
                var html = renderer.Render();

                var folderPath = Path.Combine(outputDir, resultsSet.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{resultsSet.FileName}.html"), html);
            }

            foreach (var resultsSet in resultsSets.OfType<CompetitionResultsSet>())
            {
                var renderer = CompetitionRendererFactory.Create(indexFileName, resultsSet, calendar, rules);

                Console.WriteLine($"Generating results for competition: {resultsSet.FileName}");
                var html = renderer.Render();

                var folderPath = Path.Combine(outputDir, resultsSet.SubFolderName);
                Directory.CreateDirectory(folderPath);
                File.WriteAllText(Path.Combine(folderPath, $"{resultsSet.FileName}.html"), html);
            }
        }

        public void GenerateIndex(string indexFileName)
        {
            var eventResults = resultsSets
                .OfType<EventResultsSet>()
                .OrderBy(ev => ev.EventDate)
                .ToList();

            var competitionResults = resultsSets
                .OfType<CompetitionResultsSet>()
                .OrderBy(comp => SiteIndexRenderer.CompetitionOrder
                    .ToList()
                    .IndexOf(comp.CompetitionType))
                .ToList();

            var indexRenderer = new SiteIndexRenderer(eventResults, competitionResults, outputDir);
            indexRenderer.RenderIndex(indexFileName);
            indexRenderer.RenderRedirectIndex(indexFileName);
        }

        private static IEnumerable<Ride> GetChampionshipRides(
            IEnumerable<Ride> rides,
            IEnumerable<CalendarEvent> calendar)
        {
            var championshipEventNumbers = new HashSet<int>(
                calendar.Where(ev => ev.IsClubChampionship)
                        .Select(ev => ev.EventNumber));

            return rides.Where(r => championshipEventNumbers.Contains(r.EventNumber));
        }

        private static IEnumerable<CalendarEvent> GetChampionshipCalendar(
            IEnumerable<CalendarEvent> fullCalendar)
        {
            return fullCalendar.Where(ev => ev.IsClubChampionship);
        }
    }
}