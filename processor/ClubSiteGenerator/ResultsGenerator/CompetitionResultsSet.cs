using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> Calendar;

        protected CompetitionResultsSet(IEnumerable<CompetitorResult> scoredRides, IEnumerable<CalendarEvent> calendar)
        {
            Calendar = calendar;
        }

        public abstract AgeGroup? AgeGroupFilter { get; }
        public abstract string CompetitionCode { get; }
    }

}
