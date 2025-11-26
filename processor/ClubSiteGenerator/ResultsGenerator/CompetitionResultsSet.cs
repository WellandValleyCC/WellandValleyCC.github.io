using ClubCore.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    // -------------------- Competition Results --------------------

    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> Calendar;

        protected CompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> calendar)
            : base(rides)
        {
            Calendar = calendar;
        }

        public abstract AgeGroup? AgeGroupFilter { get; }
        public abstract string CompetitionCode { get; }
    }

}
