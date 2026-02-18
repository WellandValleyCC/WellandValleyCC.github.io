using ClubCore.Models;
using ClubCore.Utilities;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Services.Hydration;

namespace ClubSiteGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ClubSiteGenerator starting…");

            var yearIndex = Array.IndexOf(args, "--year");
            if (yearIndex == -1 || yearIndex + 1 >= args.Length)
            {
                Console.Error.WriteLine("[ERROR] Missing --year argument (e.g. --year 2025)");
                Environment.Exit(1);
            }

            var year = args[yearIndex + 1];
            Console.WriteLine($"[INFO] Processing year: {year}");

            var indexFilename = (year == "2025")
                ? $"preview{year}.html"
                : $"index{year}.html";

            var outputRoot = OutputLocator.GetOutputDirectory();
            Console.WriteLine($"Writing site to: {outputRoot}");

            using var competitorDb = DbContextHelper.CreateReadonlyCompetitorDbContext(year);
            using var eventDb = DbContextHelper.CreateReadonlyEventDbContext(year);

            var eventCalendar = DataLoader.LoadCalendar(eventDb);
            var allRides = DataLoader.LoadRides(eventDb);
            var allCompetitors = DataLoader.LoadCompetitors(competitorDb);
            var rrRiders = DataLoader.LoadRoundRobinRiders(eventDb);

            RideHydrator.AttachCalendarEvents(allRides, eventCalendar);
            RideHydrator.AttachCompetitors(allRides, allCompetitors, eventCalendar);
            RideHydrator.AttachRoundRobinRiders(allRides, rrRiders);
            RideHydrator.AttachSyntheticWvccRoundRobinRiders(allRides, allCompetitors);

            // Club site
            GenerateClubSite(
                Path.Combine(outputRoot, "SiteOutput"),
                allRides,
                allCompetitors,
                eventCalendar,
                indexFilename
            );

            // Round Robin site
            var rrEventCalendar = rrCalendarFromFullCalendar(eventCalendar);

            var roundRobinClubs = DataLoader.LoadRoundRobinClubs(eventDb);

            var activeClubs = roundRobinClubs
                .Where(c => c.FromYear <= int.Parse(year))
                .ToList();

            GenerateRoundRobinSite(
                Path.Combine(outputRoot, "RoundRobinSiteOutput"),
                allRides,
                allCompetitors,
                rrEventCalendar,
                activeClubs,
                indexFilename
            );
        }

        static void GenerateClubSite(
            string outputDir,
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar,
            string indexFileName)
        {
            Directory.CreateDirectory(outputDir);

            var orchestrator = new ClubResultsOrchestrator(
                outputDir,
                rides,
                competitors,
                calendar);

            orchestrator.GenerateAll(indexFileName);
        }

        static List<CalendarEvent> rrCalendarFromFullCalendar(List<CalendarEvent> fullCalendar)
        {
            var rrEventCalendar = fullCalendar
                .Where(e => e.IsRoundRobinEvent)
                .OrderBy(e => e.EventNumber)
                .ToList();

            for (int i = 0; i < rrEventCalendar.Count; i++)
                rrEventCalendar[i].RoundRobinEventNumber = i + 1;

            return rrEventCalendar;
        }

        static void GenerateRoundRobinSite(
            string outputDir,
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> rrEventCalendar,
            IEnumerable<RoundRobinClub> clubs,
            string indexFileName)
        {
            Directory.CreateDirectory(outputDir);

            var rrOrchestrator = new RoundRobinResultsOrchestrator(
                outputDir,
                rides,
                competitors,
                rrEventCalendar,
                clubs);

            rrOrchestrator.GenerateAll(indexFileName);
        }
    }
}
