using ClubProcessor.Interfaces;
using ClubProcessor.Models;

namespace ClubProcessor.Calculators
{
    internal abstract class BaseCompetitionScoreCalculator : ICompetitionScoreCalculator, IRideProcessor
    {
        public abstract string CompetitionName { get; }

        private readonly Func<int, int> pointsForPosition;
        protected BaseCompetitionScoreCalculator(Func<int, int> pointsForPosition)
        {
            this.pointsForPosition = pointsForPosition;
        }

        public int ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligibleRides = rides
                .Where(r => r.Competitor != null && r.EventNumber == eventNumber)
                .Where(IsEligible)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            int position = 1;
            int i = 0;

            while (i < eligibleRides.Count)
            {
                var currentTime = eligibleRides[i].TotalSeconds;

                var tiedGroup = eligibleRides
                    .Skip(i)
                    .TakeWhile(r => r.TotalSeconds == currentTime)
                    .ToList();

                int tieCount = tiedGroup.Count;
                int totalPoints = Enumerable.Range(position, tieCount)
                    .Select(pointsForPosition)
                    .Sum();

                double sharedPoints = (double)totalPoints / tieCount;

                foreach (var ride in tiedGroup)
                {
                    AssignPoints(ride, position, sharedPoints);
                }

                position += tieCount;
                i += tieCount;
            }

            return eligibleRides.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return ApplyScores(eventNumber, eventRides, pointsForPosition);
        }

        protected abstract bool IsEligible(Ride ride);
        protected abstract void AssignPoints(Ride ride, int position, double points);
    }
}
