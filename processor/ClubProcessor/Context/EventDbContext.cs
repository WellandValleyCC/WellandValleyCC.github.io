using ClubProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Context
{
    public class EventDbContext : DbContext
    {
        public DbSet<Ride> Rides { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<PointsAllocation> PointsAllocations { get; set; }

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
            // Ride configuration
            modelBuilder.Entity<Ride>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Ride>()
                .Property(r => r.Eligibility)
                .HasConversion<string>();

            // CalendarEvent configuration
            modelBuilder.Entity<CalendarEvent>(entity =>
            {
                entity.ToTable("CalendarEvents");

                entity.HasKey(e => e.EventID);

                entity.Property(e => e.EventDate).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Miles).HasColumnType("REAL");
                entity.Property(e => e.Location).HasMaxLength(100);
                entity.Property(e => e.SheetName).HasMaxLength(20);
            });
        }

        public int GetPointsForPosition(int position)
        {
            // Clamp position to 1–100
            var clamped = Math.Clamp(position, 1, 100);

            return PointsAllocations
                .AsNoTracking()
                .Where(p => p.Position == clamped)
                .Select(p => p.Points)
                .FirstOrDefault();
        }
    }
}
