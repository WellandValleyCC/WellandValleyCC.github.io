using ClubCore.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubCore.Context
{
    public class CompetitorDbContextFactory : IDesignTimeDbContextFactory<CompetitorDbContext>
    {
        public CompetitorDbContext CreateDbContext(string[] args)
        {
            var year = args.Length > 0 ? args[0] : "2025";
            var dbPath = DbPathResolver.GetCompetitorDbPath(year);

            Console.WriteLine($"[INFO] Creating CompetitorDbContext for year {year} → {dbPath}");

            var optionsBuilder = new DbContextOptionsBuilder<CompetitorDbContext>();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");

            return new CompetitorDbContext(optionsBuilder.Options);
        }
    }
}
