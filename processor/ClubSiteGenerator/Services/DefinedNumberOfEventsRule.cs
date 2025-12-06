using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Services
{
    public class DefinedNumberOfEventsRule : ICompetitionRule
    {
        private readonly int fixedNumber;
        public DefinedNumberOfEventsRule(int fixedNumber) => this.fixedNumber = fixedNumber;

        public int GetLimit(int totalEvents) => Math.Min(fixedNumber, totalEvents);
    }
}
