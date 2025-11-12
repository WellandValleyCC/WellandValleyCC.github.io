using ClubProcessor.Interfaces;
using ClubCore.Models;

namespace ClubProcessor.Calculators
{
    public abstract class BaseEventRankCalculator : IEventRankCalculator, IRideProcessor
    {
        public int AssignRanks(int eventNumber, List<Ride> rides)
        {
            var eligible = rides
                .Where(IsEligible)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            int lastRank = 0;
            double? lastTime = null;

            for (int i = 0; i < eligible.Count; i++)
            {
                var current = eligible[i];
                var time = current.TotalSeconds;

                int rank = (i == 0 || !Nullable.Equals(time, lastTime)) ? i + 1 : lastRank;

                AssignRank(current, rank);

                lastRank = rank;
                lastTime = time;
            }

            return eligible.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides) => AssignRanks(eventNumber, eventRides);

        protected abstract bool IsEligible(Ride ride);
        protected abstract void AssignRank(Ride ride, int rank);
    }
}
