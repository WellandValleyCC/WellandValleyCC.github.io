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

        public void ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligible = rides
                .Where(r => r.Competitor != null && r.EventNumber == eventNumber &&
                            r.Competitor.ClaimStatus is ClaimStatus.FirstClaim or ClaimStatus.Honorary)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            var position = 1;
            int i = 0;

            while (i < eligible.Count)
            {
                var currentTime = eligible[i].TotalSeconds;

                // Find all riders tied at this time
                var tiedGroup = eligible
                    .Skip(i)
                    .TakeWhile(r => r.TotalSeconds == currentTime)
                    .ToList();

                int tieCount = tiedGroup.Count;

                // Calculate total points for the positions consumed by this tie
                int totalPoints = Enumerable.Range(position, tieCount)
                    .Select(pointsForPosition)
                    .Sum();

                //int sharedPoints = (totalPoints + tieCount - 1) / tieCount; // round fractions up
                //int sharedPoints = (int)Math.Round((double)totalPoints / tieCount, MidpointRounding.AwayFromZero);
                double sharedPoints = (double)totalPoints / tieCount;

                foreach (var ride in tiedGroup)
                {
                    ride.SeniorsPosition = position;
                    ride.SeniorsPoints = sharedPoints;
                }

                // Advance position and index by the size of the tie group
                position += tieCount;
                i += tieCount;
            }
        }
    }
}
