using ClubProcessor.Configuration;
using ClubCore.Context;
using ClubCore.Models.Extensions;
using ClubProcessor.Services;
using ClubCore.Utilities;
using ClubCore.Models;

namespace ClubProcessor.Orchestration
{
    public class EventImportCoordinator
    {
        public void Run(string inputPath, string year)
        {
            Console.WriteLine($"[INFO] Starting events ingestion for: {inputPath}");

            using var eventContext = DbContextHelper.CreateEventContext(year);
            using var competitorContext = DbContextHelper.CreateCompetitorContext(year);

            var calendar = ImportCalendar(eventContext, inputPath, year);
            if (calendar == null) return;

            var roundRobinRiders = ImportRoundRobinRiders(eventContext, inputPath, year);
            if (roundRobinRiders == null) return;

            var competitors = ImportLeagues(competitorContext, inputPath, year);
            if (competitors == null) return;

            var rides = ImportEvents(eventContext, competitorContext, inputPath, calendar);
            if (rides == null) return;

            var pointsForPosition = CompetitionConfig.LoadPointsForPosition(eventContext);
            var competitionYear = int.Parse(year);

            var processors = RideProcessingCoordinatorFactory.DiscoverAll(pointsForPosition, competitionYear);
            var scorer = new RideProcessingCoordinator(processors, pointsForPosition);

            CompetitorExtensions.LogOverrideEligibleCompetitors(competitors);

            // Apply scoring and ranking
            scorer.ProcessAll(rides, competitors, calendar, roundRobinRiders);

            // Save changes
            eventContext.SaveChanges();
        }

        private IEnumerable<CalendarEvent>? ImportCalendar(EventDbContext context, string inputPath, string year)
        {
            var calendarCsvPath = Path.Combine(inputPath, $"Calendar_{year}.csv");
            if (File.Exists(calendarCsvPath))
            {
                Console.WriteLine($"[INFO] Importing calendar from: {calendarCsvPath}");
                var importer = new CalendarImporter(context);
                importer.ImportFromCsv(calendarCsvPath);
                Console.WriteLine("[OK] Calendar import complete");

                return context.CalendarEvents.ToList();
            }

            Console.WriteLine($"[ERROR] Calendar CSV not found: {calendarCsvPath}");
            return default;
        }

        private IEnumerable<RoundRobinRider>? ImportRoundRobinRiders(EventDbContext context, string inputPath, string year)
        {
            var csvPath = Path.Combine(inputPath, $"RoundRobinRiders_{year}.csv");
            if (File.Exists(csvPath))
            {
                Console.WriteLine($"[INFO] Importing RoundRobinRiders from: {csvPath}");
                var importer = new RoundRobinRiderImporter(context);
                importer.Import(csvPath);
                Console.WriteLine("[OK] RoundRobinRiders import complete");

                return context.RoundRobinRiders.ToList();
            }

            Console.WriteLine($"[ERROR] RoundRobinRiders CSV not found: {csvPath}");
            return default;
        }

        private IEnumerable<Competitor>? ImportLeagues(CompetitorDbContext competitorContext, string inputPath, string year)
        {
            var leaguesCsvPath = Path.Combine(inputPath, $"Leagues_{year}.csv");
            if (File.Exists(leaguesCsvPath))
            {
                Console.WriteLine($"[INFO] Importing leagues from: {leaguesCsvPath}");
                var importer = new LeagueMembershipImporter(competitorContext, DateTime.UtcNow);

                // Run import and capture counts
                var (updatedCount, clearedCount) = importer.Import(leaguesCsvPath);

                Console.WriteLine($"[OK] League import complete: {updatedCount} updated, {clearedCount} cleared");
                
                return competitorContext.Competitors.ToList();
            }

            Console.WriteLine($"[WARN] Leagues CSV not found: {leaguesCsvPath}");
            return null;
        }

        private IEnumerable<Ride>? ImportEvents(
            EventDbContext eventContext, 
            CompetitorDbContext competitorContext, 
            string inputPath,
            IEnumerable<CalendarEvent> calendar)
        {
            var processor = new EventsImporter(eventContext, competitorContext, calendar);
            if (processor.ImportFromFolder(inputPath))
                return eventContext.Rides.ToList();

            return null;
        }
    }
}

