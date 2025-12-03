using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class CompetitionResultsSet : ResultsSet
    {

        /// <summary>
        /// All scored rides contributing to this competition.
        /// </summary>
        public IReadOnlyList<CompetitorResult> ScoredRides { get; }

        protected CompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar)        
        {
            ScoredRides = scoredRides.ToList().AsReadOnly();
        }

        public abstract string CompetitionType { get; }

        public abstract string EligibilityStatement { get; }
    }
}
