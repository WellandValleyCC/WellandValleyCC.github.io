using ClubProcessor.Models;

namespace EventProcessor.Tests.Helpers
{
    public static class CompetitorFinders
    {
        // Throws if not found (use when a missing competitor is a test failure)
        public static Competitor GetByClubNumber(this IEnumerable<Competitor> competitors, int clubNumber)
        {
            if (competitors == null) throw new ArgumentNullException(nameof(competitors));
            return competitors.First(c => c.ClubNumber == clubNumber);
        }

        // Safe try-find pattern (preferred in fixtures where absence is expected)
        public static bool TryGetByClubNumber(this IEnumerable<Competitor> competitors, int clubNumber, out Competitor? competitor)
        {
            if (competitors == null) throw new ArgumentNullException(nameof(competitors));
            competitor = competitors.FirstOrDefault(c => c.ClubNumber == clubNumber);
            return competitor != null;
        }

        // Optionally: return null when not found (concise alternative)
        public static Competitor? FindByClubNumberOrDefault(this IEnumerable<Competitor> competitors, int clubNumber)
        {
            if (competitors == null) throw new ArgumentNullException(nameof(competitors));
            return competitors.FirstOrDefault(c => c.ClubNumber == clubNumber);
        }
    }
}
