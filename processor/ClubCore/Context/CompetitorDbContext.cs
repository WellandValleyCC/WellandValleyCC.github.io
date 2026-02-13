using ClubCore.Models;
using ClubCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace ClubCore.Context
{
    public class CompetitorDbContext : DbContext
    {
        public DbSet<Competitor> Competitors { get; set; }

        public CompetitorDbContext(DbContextOptions<CompetitorDbContext> options)
            : base(options)
        {
        }

        // Optional fallback for CLI runs without DI
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = DbPathResolver.ResolveCompetitorDbPath("2025"); // or inject year if dynamic
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}

