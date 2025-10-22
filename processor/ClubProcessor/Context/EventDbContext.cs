using ClubProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Context
{
    public class EventDbContext : DbContext
    {
        public DbSet<Ride> Rides { get; set; }

        public EventDbContext(DbContextOptions<EventDbContext> options)
            : base(options)
        {
        }

        // Optional fallback for CLI runs without DI
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var basePath = AppContext.BaseDirectory;
                var dbPath = Path.Combine(basePath, "data", "club_events_fallback.db");
                Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ride>()
                .HasKey(r => r.Id);

            // Optional: configure enums as strings
            modelBuilder.Entity<Ride>()
                .Property(r => r.Eligibility)
                .HasConversion<string>();

            // Optional: add indexes or constraints here
        }
    }
}
