using ClubCore.Context;
using Microsoft.EntityFrameworkCore;

namespace ClubCore.Utilities
{
    public static class DbContextHelper
    {
        public static EventDbContext CreateEventContext(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            var path = Path.Combine(dataDir, $"club_events_{year}.db");

            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

            var dbContext = new EventDbContext(options);

            // Standard context: apply migrations if needed
            dbContext.Database.Migrate();

            return dbContext;
        }

        public static CompetitorDbContext CreateCompetitorContext(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            var path = Path.Combine(dataDir, $"club_competitors_{year}.db");

            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

            var dbContext = new CompetitorDbContext(options);

            // Standard context: apply migrations if needed
            dbContext.Database.Migrate();

            return dbContext;
        }

        public static EventDbContext CreateReadonlyEventDbContext(string year)
        {
            var dbContext = CreateEventContext(year);
            ValidateSchema(dbContext, $"club_events_{year}.db");
            return dbContext;
        }

        public static CompetitorDbContext CreateReadonlyCompetitorDbContext(string year)
        {
            var dbContext = CreateCompetitorContext(year);
            ValidateSchema(dbContext, $"club_competitors_{year}.db");
            return dbContext;
        }

        private static void ValidateSchema(DbContext dbContext, string path)
        {
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                throw new InvalidOperationException(
                    $"Pending migrations detected for {path}. Read-only app expects schema to be pre-applied.");
            }

            Console.WriteLine($"[INFO] Schema validated for: {path}");
        }
    }
}