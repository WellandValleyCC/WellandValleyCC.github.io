using ClubProcessor.Interfaces;
using ClubProcessor.Models;

namespace ClubProcessor.Calculators
{
    public abstract class BaseCompetitionScoreCalculator : ICompetitionScoreCalculator, IRideProcessor
    {
        public abstract string CompetitionName { get; }

        protected readonly Func<int, int> pointsForPosition;

        protected BaseCompetitionScoreCalculator(Func<int, int> pointsForPosition)
        {
            this.pointsForPosition = pointsForPosition;
        }

        public int ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition)
        {
            var eligibleRides = rides
                .Where(r => r.Competitor != null && r.EventNumber == eventNumber)
                .Where(IsEligible)
                .OrderBy(GetOrderingTime)
                .ToList();

            ClearIneligibleRideScores(rides, eligibleRides);

            int position = 1;
            int i = 0;

            while (i < eligibleRides.Count)
            {
                var currentTime = GetOrderingTime(eligibleRides[i]);

                var tiedGroup = eligibleRides
                    .Skip(i)
                    .TakeWhile(r => GetOrderingTime(r) == currentTime)
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

        public virtual int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return ApplyScores(eventNumber, eventRides, pointsForPosition);
        }

        protected abstract bool IsEligible(Ride ride);
        protected abstract void AssignPoints(Ride ride, int position, double points);

        /// <summary>
        /// Returns the time value used for ranking. Override in derived classes to apply handicaps or alternate metrics.
        /// </summary>
        protected virtual double GetOrderingTime(Ride r) => r.TotalSeconds;

        protected abstract void ClearPoints(Ride ride);

        protected void ClearIneligibleRideScores(
            IEnumerable<Ride> rides,
            IEnumerable<Ride> eligibleRides)
        {
            var eligibleSet = eligibleRides.ToHashSet();

            foreach (var ride in rides)
            {
                if (!eligibleSet.Contains(ride))
                {
                    ClearPoints(ride);
                }
            }
        }
    }
}
