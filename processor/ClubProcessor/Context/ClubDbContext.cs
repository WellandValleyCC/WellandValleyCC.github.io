using Microsoft.EntityFrameworkCore;
using ClubProcessor.Models;

namespace ClubProcessor.Context
{
    public class ClubDbContext : DbContext
    {
        public DbSet<Competitor> Competitors { get; set; }

        public ClubDbContext(DbContextOptions<ClubDbContext> options)
            : base(options)
        {
        }

        // Optional fallback for CLI runs without DI
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var basePath = AppContext.BaseDirectory;
                var dbPath = Path.Combine(basePath, "data", "results.db");
                var directoryPath = Path.GetDirectoryName(dbPath);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}

