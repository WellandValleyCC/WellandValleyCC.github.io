using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Services
{
    public class HalfPlusOneCappedRule : ICompetitionRule
    {
        private readonly int cap;
        public HalfPlusOneCappedRule(int cap) => this.cap = cap;

        public int GetLimit(int totalEvents)
        {
            var halfPlusOne = (totalEvents / 2) + 1;
            return Math.Min(halfPlusOne, cap);
        }
    }
}
