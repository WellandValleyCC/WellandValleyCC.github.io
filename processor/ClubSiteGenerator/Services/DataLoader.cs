using ClubCore.Context;
using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public static class DataLoader
    {
        public static IReadOnlyList<CalendarEvent> LoadCalendar(EventDbContext eventDb) =>
            eventDb.CalendarEvents.ToList();

        public static IReadOnlyList<Competitor> LoadCompetitors(CompetitorDbContext competitorDb) =>
            competitorDb.Competitors.ToList();

        public static IReadOnlyList<Ride> LoadRides(EventDbContext eventDb) =>
            eventDb.Rides.ToList();

        public static void AttachReferencesToRides(
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar)
        {
            // Index calendar events by EventNumber (unique by your contract)
            var eventsByNumber = calendar.ToDictionary(e => e.EventNumber);

            // Group competitors by ClubNumber, sort each group by CreatedUtc ascending
            var competitorsByClub = competitors
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(c => DateTime.SpecifyKind(c.CreatedUtc, DateTimeKind.Utc)).ToList()
                );

            foreach (var ride in rides)
            {
                // Attach calendar event
                if (eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                {
                    ride.CalendarEvent = ev;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Ride for event number {ride.EventNumber} has no matching CalendarEvent");
                }

                // Attach competitor snapshot if ClubNumber present
                if (ride.ClubNumber.HasValue)
                {
                    if (!competitorsByClub.TryGetValue(ride.ClubNumber.Value, out var snapshots))
                    {
                        throw new InvalidOperationException(
                            $"Ride '{ride.Name}' (Event {ride.EventNumber}, Club {ride.ClubNumber}) has no matching Competitor");
                    }

                    var eventDateUtc = DateTime.SpecifyKind(ev.EventDate, DateTimeKind.Utc);

                    // Find latest snapshot with CreatedUtc <= event date
                    Competitor? chosen = null;
                    for (int i = snapshots.Count - 1; i >= 0; i--)
                    {
                        var candidate = DateTime.SpecifyKind(snapshots[i].CreatedUtc, DateTimeKind.Utc);
                        if (candidate <= eventDateUtc)
                        {
                            chosen = snapshots[i];
                            break;
                        }
                    }

                    if (chosen == null)
                    {
                        // No snapshot before event date
                        throw new InvalidOperationException(
                            $"Ride '{ride.Name}' (Event {ride.EventNumber}, Club {ride.ClubNumber}) has no valid Competitor snapshot before {eventDateUtc:u}");
                    }

                    ride.Competitor = chosen;
                }
            }
        }
    }
}
