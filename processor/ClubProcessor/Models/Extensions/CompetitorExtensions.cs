using ClubProcessor.Models.Enums;
namespace ClubProcessor.Models.Extensions
{
    public static class CompetitorExtensions
    {
        /// <summary>
        /// NOTE: This override list reflects committee policy as of 2025 season.
        /// These competitors are SecondClaim but included in club competitions.
        /// Update this list if committee policy changes.
        /// </summary>
        private static readonly HashSet<int> OverrideEligibleClubNumbers = new()
        {
            1152, // Ruby
            1144  // Jamie
        };

        public static bool IsEligible(this Competitor competitor)
        {
            if (competitor == null) return false;

            if (competitor.ClaimStatus != ClaimStatus.SecondClaim)
                return true;

            return OverrideEligibleClubNumbers.Contains(competitor.ClubNumber);
         }

        public static void LogOverrideEligibleCompetitors(IEnumerable<Competitor> competitors)
        {
            var overrides = competitors
                .Where(c => OverrideEligibleClubNumbers.Contains(c.ClubNumber))
                .ToList();

            if (overrides.Count == 0)
            {
                Console.WriteLine("[INFO] No override-eligible competitors configured.");
                return;
            }

            Console.WriteLine("[INFO] Override-eligible competitors (SecondClaim but included):");
            foreach (var c in overrides)
            {
                Console.WriteLine($"    ClubNumber={c.ClubNumber}, Name={c.FullName}, ClaimStatus={c.ClaimStatus}");
            }
        }
    }
}
