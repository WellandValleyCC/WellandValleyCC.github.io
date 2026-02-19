using ClubCore.Models;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public abstract class RoundRobinResultsSet : ResultsSet
    {
        protected RoundRobinResultsSet(IEnumerable<CalendarEvent> calendar)
            : base(calendar)
        {
        }

        public string CssFile { get; set; } = "";

        public ICompetitionRules? CompetitionRules { get; set; }
    }
}