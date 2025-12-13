using ClubCore.Models;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;

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

        public IEnumerable<CalendarEvent> ClubChampionshipCalendar =>
            Calendar.Where(ev => ev.IsClubChampionship);

        public abstract CompetitionType CompetitionType { get; }

        public abstract string EligibilityStatement { get; }
    }
}
