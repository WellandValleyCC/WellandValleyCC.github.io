using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Services
{
    public class CompetitionRuleHalfPlusOneCapped : ICompetitionRule
    {
        private readonly int cap;
        public CompetitionRuleHalfPlusOneCapped(int cap) => this.cap = cap;

        public int GetLimit(int totalEvents) => Math.Min(cap, (totalEvents / 2) + 1);

        /// <summary>
        /// Defines the minimum number of non‑ten‑mile events required for scoring.
        /// </summary>  
        /// <remarks>
        /// If this rule is used for a ten‑mile competition, this prooperty will be ignored.
        /// </remarks>
        public int RequiredNonTens => 2;
    }
}
