using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Services
{
    public class HalfPlusOneMixedCappedRule : IMixedCompetitionRule
    {
        private readonly int cap;
        public int RequiredNonTens { get; }

        public HalfPlusOneMixedCappedRule(int cap, int requiredNonTens)
        {
            this.cap = cap;
            RequiredNonTens = requiredNonTens;
        }

        public int GetLimit(int totalEvents)
        {
            var halfPlusOne = (totalEvents / 2) + 1;
            return Math.Min(halfPlusOne, cap);
        }
    }
}
