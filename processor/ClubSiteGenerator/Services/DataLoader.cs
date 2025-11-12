using ClubCore.Context;
using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public static class DataLoader
    {
        public static List<Ride> LoadHydratedRides(CompetitorDbContext competitorDb, EventDbContext eventDb)
        {
            // Materialise all three sets
            var rides = eventDb.Rides.ToList();
            var competitors = competitorDb.Competitors.ToList();
            var calendarEvents = eventDb.CalendarEvents.ToList();

            // Hydrate rides with linked competitor + calendar event
            foreach (var ride in rides)
            {
                ride.Competitor = competitors.FirstOrDefault(c => c.ClubNumber == ride.ClubNumber);
                ride.CalendarEvent = calendarEvents.FirstOrDefault(e => e.EventNumber == ride.EventNumber);
            }

            return rides;
        }

        internal static List<CalendarEvent> LoadCalendar(EventDbContext eventDb)
        {
            var calendarEvents = eventDb.CalendarEvents.ToList();

            return calendarEvents;
        }
    }
}
