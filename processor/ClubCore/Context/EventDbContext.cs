using ClubCore.Models;
using ClubCore.Utilities;
using Microsoft.EntityFrameworkCore;

namespace ClubCore.Context
{
    public class EventDbContext : DbContext
    {
        public DbSet<Ride> Rides { get; set; } = default!;
        public DbSet<CalendarEvent> CalendarEvents { get; set; } = default!;
        public DbSet<PointsAllocation> PointsAllocations { get; set; } = default!;
        public DbSet<RoundRobinClub> RoundRobinClubs { get; set; } = default!;
        public DbSet<RoundRobinRider> RoundRobinRiders { get; set; } = default!;

        public EventDbContext(DbContextOptions<EventDbContext> options)
            : base(options)
        {
        }

        // Optional fallback for CLI runs without DI
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                if (!optionsBuilder.IsConfigured)
                {
                    var dbPath = DbPathResolver.ResolveEventDbPath("2025"); // or inject year if dynamic
                    Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
                    optionsBuilder.UseSqlite($"Data Source={dbPath}");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ride configuration
            modelBuilder.Entity<Ride>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Ride>()
                .Property(r => r.Status)
                .HasConversion<string>();

            // CalendarEvent configuration
            modelBuilder.Entity<CalendarEvent>(entity =>
            {
                entity.ToTable("CalendarEvents");

                entity.HasKey(e => e.Id); // EF-managed primary key

                entity.HasIndex(e => e.EventNumber).IsUnique();

                entity.Property(e => e.EventNumber).IsRequired();
                entity.Property(e => e.EventDate).IsRequired();
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EventName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Miles).HasColumnType("REAL");
                entity.Property(e => e.Location).HasMaxLength(100);
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
