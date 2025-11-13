using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.ResultsGenerator
{
    public class EventResults : BaseResults
    {
        public readonly CalendarEvent CalendarEvent;

        public EventResults(int eventNumber, List<CalendarEvent> eventsCalendar, List<Ride> rides)
            : base(rides)
        {
            CalendarEvent = eventsCalendar.Single(e => e.EventNumber == eventNumber);
        }

        public override string DisplayName => CalendarEvent.EventName;
        public override string FileName => $"event-{CalendarEvent.EventNumber:D2}";
        public override string SubFolderName => "events";

        public int EventNumber => CalendarEvent.EventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(CalendarEvent.EventDate);
        
        public override IEnumerable<Ride> EventRides()
            => AllRides.Where(r => r.EventNumber == CalendarEvent.EventNumber);
    }
}
