using ClubCore.Models;
using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public abstract class RoundRobinCompetitionResultsSet<T> : ResultsSet
    {
        public IReadOnlyList<T> Results { get; }

        protected RoundRobinCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<T> results)
            : base(calendar)
        {
            Results = results.ToList().AsReadOnly();
        }

        public abstract RoundRobinCompetitionType CompetitionType { get; }

        public abstract string EligibilityStatement { get; }
    }
}