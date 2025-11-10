using ClubProcessor.Models;

namespace EventProcessor.Tests.Helpers
{
    public static class TestCalendarEvents
    {
        public static CalendarEvent Create(int eventNumber, DateTime eventDateUtc, double miles)
            => new CalendarEvent
            {
                EventNumber = eventNumber,
                EventDate = DateTime.SpecifyKind(eventDateUtc, DateTimeKind.Utc),
                Miles = miles,
            };

        public static IReadOnlyList<CalendarEvent> CreateSequence(
            IEnumerable<int> eventNumbers,
            DateTime firstEventDateUtc,
            TimeSpan interval,
            IReadOnlyDictionary<int, double>? milesByEvent = null)
        {
            var numbers = eventNumbers.Distinct().OrderBy(n => n).ToArray();
            var events = new List<CalendarEvent>(numbers.Length);

            for (int i = 0; i < numbers.Length; i++)
            {
                var dt = DateTime.SpecifyKind(firstEventDateUtc.AddTicks(interval.Ticks * i), DateTimeKind.Utc);
                var miles = milesByEvent != null && milesByEvent.TryGetValue(numbers[i], out var m)
                    ? m
                    : 10.0; // default to 10 miles
                events.Add(Create(numbers[i], dt, miles));
            }

            return events;
        }

        public static IReadOnlyDictionary<int, CalendarEvent> CreateLookupForRides(
            IEnumerable<Ride> rides,
            DateTime firstEventDateUtc,
            TimeSpan? interval = null,
            IReadOnlyDictionary<int, double>? milesByEvent = null)
        {
            interval ??= TimeSpan.FromDays(30);
            var eventNumbers = rides.Select(r => r.EventNumber).Distinct().OrderBy(n => n);
            var events = CreateSequence(eventNumbers, firstEventDateUtc, interval.Value, milesByEvent);
            return events.ToDictionary(e => e.EventNumber, e => e);
        }

        public static DateTime GetEventDateUtcOrThrow(
            IReadOnlyDictionary<int, CalendarEvent> eventsByNumber,
            Ride ride)
        {
            if (!eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                throw new InvalidOperationException($"Test setup missing CalendarEvent for EventNumber {ride.EventNumber}");
            return DateTime.SpecifyKind(ev.EventDate, DateTimeKind.Utc);
        }
    }
}
