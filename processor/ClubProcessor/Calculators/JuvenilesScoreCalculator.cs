using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class JuvenilesScoreCalculator : ICompetitionScoreCalculator
    {
        public string CompetitionName => "Juveniles";

        public void ApplyScores(List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligible = rides
                .Where(r => r.Competitor != null &&
                            r.Competitor.ClaimStatus is ClaimStatus.FirstClaim or ClaimStatus.Honorary)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            for (int i = 0; i < eligible.Count; i++)
            {
                eligible[i].JuvenilesPoints = pointsForPosition(i + 1);
            }
        }
    }
}
