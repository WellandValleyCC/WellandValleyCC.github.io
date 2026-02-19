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

        public virtual string EligibilityStatement => string.Empty;
        public virtual string ScoringStatement => string.Empty;
        public virtual string AdditionalComments => string.Empty;

    }
}