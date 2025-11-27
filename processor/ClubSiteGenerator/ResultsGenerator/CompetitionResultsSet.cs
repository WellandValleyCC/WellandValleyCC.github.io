using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> Calendar;

        /// <summary>
        /// All scored rides contributing to this competition.
        /// </summary>
        public IReadOnlyList<CompetitorResult> ScoredRides { get; }

        protected CompetitionResultsSet(IEnumerable<CompetitorResult> scoredRides, IEnumerable<CalendarEvent> calendar)
        {
            Calendar = calendar;
            ScoredRides = scoredRides.ToList().AsReadOnly();
        }

        public abstract AgeGroup? AgeGroupFilter { get; }
        public abstract string CompetitionCode { get; }
    }

}
