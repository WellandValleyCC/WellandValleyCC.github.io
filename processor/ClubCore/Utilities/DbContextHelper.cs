using ClubCore.Context;
using Microsoft.EntityFrameworkCore;

namespace ClubCore.Utilities
{
    public static class DbContextHelper
    {
        // Public: writable contexts (apply migrations)
        public static EventDbContext CreateEventContext(string year)
            => CreateWritableContext<EventDbContext>(BuildEventContextOptions(year), opts => new EventDbContext(opts), GetEventDbPath(year));

        public static CompetitorDbContext CreateCompetitorContext(string year)
            => CreateWritableContext<CompetitorDbContext>(BuildCompetitorContextOptions(year), opts => new CompetitorDbContext(opts), GetCompetitorDbPath(year));

        // Public: read-only contexts (validate only, no migrations)
        public static EventDbContext CreateReadonlyEventDbContext(string year)
            => CreateReadonlyContext<EventDbContext>(BuildEventContextOptions(year), opts => new EventDbContext(opts), GetEventDbPath(year));

        public static CompetitorDbContext CreateReadonlyCompetitorDbContext(string year)
            => CreateReadonlyContext<CompetitorDbContext>(BuildCompetitorContextOptions(year), opts => new CompetitorDbContext(opts), GetCompetitorDbPath(year));

        // Private: common builders

        private static string GetEventDbPath(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            return Path.Combine(dataDir, $"club_events_{year}.db");
        }

        private static string GetCompetitorDbPath(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            return Path.Combine(dataDir, $"club_competitors_{year}.db");
        }

        private static DbContextOptions<EventDbContext> BuildEventContextOptions(string year)
        {
            var path = GetEventDbPath(year);
            return new DbContextOptionsBuilder<EventDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;
        }

        private static DbContextOptions<CompetitorDbContext> BuildCompetitorContextOptions(string year)
        {
            var path = GetCompetitorDbPath(year);
            return new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;
        }

        private static TDbContext CreateWritableContext<TDbContext>(
            DbContextOptions<TDbContext> options,
            Func<DbContextOptions<TDbContext>, TDbContext> factory,
            string dbPath)
            where TDbContext : DbContext
        {
            var dbContext = factory(options);

            // Writable: apply migrations
            dbContext.Database.Migrate();

            Console.WriteLine($"[INFO] Migrations applied for: {dbPath}");
            return dbContext;
        }

        private static TDbContext CreateReadonlyContext<TDbContext>(
            DbContextOptions<TDbContext> options,
            Func<DbContextOptions<TDbContext>, TDbContext> factory,
            string dbPath)
            where TDbContext : DbContext
        {
            var dbContext = factory(options);

            // Read-only: validate that schema is already applied (no migrations)
            ValidateSchema(dbContext, dbPath);

            return dbContext;
        }

        private static void ValidateSchema(DbContext dbContext, string dbPath)
        {
            // If any migrations are pending, fail fast — read-only must not mutate the DB
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                throw new InvalidOperationException(
                    $"Pending migrations detected for {dbPath}. Read-only app expects schema to be pre-applied.");
            }

            Console.WriteLine($"[INFO] Schema validated for: {dbPath}");
        }
    }
}