using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Services
{
    public class CompetitionRuleDefinedNumberOfEvents : ICompetitionRule
    {
        private readonly int fixedNumber;
        public CompetitionRuleDefinedNumberOfEvents(int fixedNumber) => this.fixedNumber = fixedNumber;

        public int GetLimit(int totalTenEventsInCalendar)
        {
            // Ensure the fixed number does not exceed the available events
            return Math.Min(fixedNumber, totalTenEventsInCalendar);
        }

        /// <summary>
        /// Defines the minimum number of non‑ten‑mile events required for scoring.
        /// </summary>  
        /// <remarks>
        /// If this rule is used for a ten‑mile competition, this prooperty will be ignored.
        /// </remarks>
        public int RequiredNonTens => 2;
    }
}
