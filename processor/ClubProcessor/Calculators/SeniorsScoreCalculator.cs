using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubProcessor.Calculators
{
    /// <summary>
    /// Scores all FirstClaim and Honorary members based on raw ride time.
    /// </summary>
    public class SeniorsScoreCalculator : ICompetitionScoreCalculator
    {
        public string CompetitionName => "Seniors";

        public void ApplyScores(List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligible = rides
                .Where(r => r.Competitor != null &&
                            r.Competitor.ClaimStatus is ClaimStatus.FirstClaim or ClaimStatus.Honorary)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            for (int i = 0; i < eligible.Count; i++)
            {
                eligible[i].SeniorsPoints = pointsForPosition(i + 1);
            }
        }
    }
}
