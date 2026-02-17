using ClubCore.Models;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public abstract class RoundRobinCompetitionResultsSet : ResultsSet
    {
        public IReadOnlyList<CompetitorResult> ScoredRides { get; }

        protected RoundRobinCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> scoredRides)
            : base(calendar)
        {
            ScoredRides = scoredRides.ToList().AsReadOnly();
        }

        public abstract RoundRobinCompetitionType CompetitionType { get; }

        public abstract string EligibilityStatement { get; }
    }
}
