using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubProcessor.Context
{
    public class EventDbContextFactory : IDesignTimeDbContextFactory<EventDbContext>
    {
        public EventDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EventDbContext>();
            optionsBuilder.UseSqlite("Data Source=../../data/club_events_2026.db");

            return new EventDbContext(optionsBuilder.Options);
        }
    }
}
