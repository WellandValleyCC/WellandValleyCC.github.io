using ClubCore.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    // -------------------- Competition Results --------------------

    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> CalendarEvents;

        protected CompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides)
        {
            CalendarEvents = events;
        }

        public abstract AgeGroup? AgeGroupFilter { get; }
        public abstract string CompetitionCode { get; }
    }

}
