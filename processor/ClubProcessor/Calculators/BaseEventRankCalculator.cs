using ClubProcessor.Interfaces;
using ClubProcessor.Models;

namespace ClubProcessor.Calculators
{
    public abstract class BaseEventRankCalculator : IEventRankCalculator, IRideProcessor
    {
        public int AssignRanks(int eventNumber, List<Ride> rides)
        {
            var eligibleRides = rides
                .Where(IsEligible)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            ClearIneligibleRideRanks(rides, eligibleRides);

            int lastRank = 0;
            double? lastTime = null;

            for (int i = 0; i < eligibleRides.Count; i++)
            {
                var current = eligibleRides[i];
                var time = current.TotalSeconds;

                int rank = (i == 0 || !Nullable.Equals(time, lastTime)) ? i + 1 : lastRank;

                AssignRank(current, rank);

                lastRank = rank;
                lastTime = time;
            }

            return eligibleRides.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides) => AssignRanks(eventNumber, eventRides);

        protected abstract bool IsEligible(Ride ride);
        protected abstract void AssignRank(Ride ride, int rank);

        protected abstract void ClearRank(Ride ride);

        protected void ClearIneligibleRideRanks(
            IEnumerable<Ride> rides,
            IEnumerable<Ride> eligibleRides)
        {
            var eligibleSet = eligibleRides.ToHashSet();

            foreach (var ride in rides)
            {
                if (!eligibleSet.Contains(ride))
                {
                    ClearRank(ride);
                }
            }
        }
    }
}
