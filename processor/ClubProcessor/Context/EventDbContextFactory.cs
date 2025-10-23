using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubProcessor.Context
{
    public class EventDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
    {
        public EventDbContext CreateDbContext(string[] args)
        {
            // Default to 2025 if no year is passed
            var year = args.Length > 0 ? args[0] : "2025";
            var dbPath = Path.Combine("..", "..", "data", $"club_events_{year}.db");

            var optionsBuilder = new DbContextOptionsBuilder<EventDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new EventDbContext(optionsBuilder.Options);
        }
    }

}
