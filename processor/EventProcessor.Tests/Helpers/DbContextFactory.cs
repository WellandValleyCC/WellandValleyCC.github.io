using ClubProcessor.Context;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor.Tests.Helpers
{
    public static class DbContextFactory
    {
        public static EventDbContext CreateEventContext()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(databaseName: "EventDb_" + Guid.NewGuid())
                .Options;

            return new EventDbContext(options);
        }

        public static CompetitorDbContext CreateCompetitorContext()
        {
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(databaseName: "CompetitorDb_" + Guid.NewGuid())
                .Options;

            return new CompetitorDbContext(options);
        }
    }
}

