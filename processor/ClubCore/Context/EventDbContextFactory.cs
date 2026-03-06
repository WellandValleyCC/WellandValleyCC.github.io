using ClubCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubCore.Context
{
    public class EventDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
    {
        public EventDbContext CreateDbContext(string[] args)
        {
            var year = args.Length > 0 ? args[0] : "2025";
            var dbPath = DbPathResolver.ResolveEventDbPath(year);

            Console.WriteLine($"[INFO] Creating EventDbContext for year {year} → {dbPath}");

            var optionsBuilder = new DbContextOptionsBuilder<EventDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new EventDbContext(optionsBuilder.Options);
        }
    }

}
