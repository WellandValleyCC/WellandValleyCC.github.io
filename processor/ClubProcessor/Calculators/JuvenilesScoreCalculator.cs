using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System.Reflection.Metadata.Ecma335;

namespace ClubProcessor.Calculators
{
    public class JuvenilesScoreCalculator : ICompetitionScoreCalculator, IRideProcessor
    {
        public string CompetitionName => "Juveniles";
        
        private readonly Func<int, int> pointsForPosition;
        public JuvenilesScoreCalculator(Func<int, int> pointsForPosition)
        {
            this.pointsForPosition = pointsForPosition;
        }

        public int ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligibleRides = rides
                .Where(r => r.Competitor != null && r.EventNumber == eventNumber &&
                            r.Competitor.ClaimStatus is ClaimStatus.FirstClaim or ClaimStatus.Honorary &&
                            r.Competitor.IsJuvenile &&
                            r.Eligibility == RideEligibility.Valid)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            var position = 1;
            int i = 0;

            while (i < eligibleRides.Count)
            {
                var currentTime = eligibleRides[i].TotalSeconds;

                // Find all riders tied at this time
                var tiedGroup = eligibleRides
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
                    ride.JuvenilesPosition = position;
                    ride.JuvenilesPoints = sharedPoints;
                }

                // Advance position and index by the size of the tie group
                position += tieCount;
                i += tieCount;
            }

            return eligibleRides.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return ApplyScores(eventNumber, eventRides, pointsForPosition);
        }
    }
}
