using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubProcessor.Context
{
    public class CompetitorDbContextFactory : IDesignTimeDbContextFactory<CompetitorDbContext>
    {
        public CompetitorDbContext CreateDbContext(string[] args)
        {
            // Default to 2025 if no year is passed
            var year = args.Length > 0 ? args[0] : "2025";
            var dbPath = Path.Combine("..", "..", "data", $"club_competitors_{year}.db");

            var optionsBuilder = new DbContextOptionsBuilder<CompetitorDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new CompetitorDbContext(optionsBuilder.Options);
        }
    }

}
