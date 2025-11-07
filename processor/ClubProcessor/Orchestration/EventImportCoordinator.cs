using ClubProcessor.Configuration;
using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Orchestration
{
    public class EventImportCoordinator
    {
        public EventImportCoordinator()
        {
        }

        public void Run(string inputPath, string year)
        {
            Console.WriteLine($"[INFO] Starting events ingestion for: {inputPath}");

            using var eventContext = CreateEventContext(year);
            using var competitorContext = CreateCompetitorContext(year);

            Migrate(eventContext, $"club_events_{year}.db");
            Migrate(competitorContext, $"club_competitors_{year}.db");

            ImportCalendar(eventContext, inputPath, year);
            ImportEvents(eventContext, competitorContext, inputPath);

            // Load all rides, competitors and the event calendar for scoring
            var rides = eventContext.Rides.ToList();
            var competitors = competitorContext.Competitors.ToList();
            var calendar = eventContext.CalendarEvents.ToList();
            var pointsForPosition = CompetitionConfig.LoadPointsForPosition(eventContext);
            var competitionYear = int.Parse(year);

            var processors = RideProcessingCoordinatorFactory.DiscoverAll(pointsForPosition, competitionYear);
            var scorer = new RideProcessingCoordinator(processors, pointsForPosition);

            // Apply scoring and ranking
            scorer.ProcessAll(rides, competitors, calendar);

            // Save changes
            eventContext.SaveChanges();
        }

        private EventDbContext CreateEventContext(string year)
        {
            var path = Path.Combine("data", $"club_events_{year}.db");
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;
            return new EventDbContext(options);
        }

        private CompetitorDbContext CreateCompetitorContext(string year)
        {
            var path = Path.Combine("data", $"club_competitors_{year}.db");
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;
            return new CompetitorDbContext(options);
        }

        private void Migrate(DbContext context, string dbName)
        {
            context.Database.Migrate();
            Console.WriteLine($"[INFO] Migration complete for: data/{dbName}");
        }

        private void ImportCalendar(EventDbContext context, string inputPath, string year)
        {
            var calendarCsvPath = Path.Combine(inputPath, $"Calendar_{year}.csv");
            if (File.Exists(calendarCsvPath))
            {
                Console.WriteLine($"[INFO] Importing calendar from: {calendarCsvPath}");
                var importer = new CalendarImporter(context);
                importer.ImportFromCsv(calendarCsvPath);
                Console.WriteLine("[OK] Calendar import complete");
            }
            else
            {
                Console.WriteLine($"[WARN] Calendar CSV not found: {calendarCsvPath}");
            }
        }

        private void ImportEvents(EventDbContext eventContext, CompetitorDbContext competitorContext, string inputPath)
        {
            var processor = new EventsImporter(eventContext, competitorContext);
            processor.ImportFromFolder(inputPath);
        }
    }
}

