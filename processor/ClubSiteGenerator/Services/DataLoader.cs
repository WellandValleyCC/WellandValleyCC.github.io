using ClubCore.Context;
using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public static class DataLoader
    {
        public static List<CalendarEvent> LoadCalendar(EventDbContext eventDb) =>
            eventDb.CalendarEvents.ToList();

        public static IReadOnlyList<Competitor> LoadCompetitors(CompetitorDbContext competitorDb) =>
            competitorDb.Competitors.ToList();

        public static IReadOnlyList<Ride> LoadRides(EventDbContext eventDb) =>
            eventDb.Rides.ToList();

        public static IReadOnlyList<RoundRobinRider> LoadRoundRobinRiders(EventDbContext eventDb) =>
            eventDb.RoundRobinRiders.ToList();
    }
}
