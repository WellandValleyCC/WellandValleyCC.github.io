using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class AllRiderEventRankCalculator : IEventRankCalculator, IRideProcessor
    {
        public int AssignRanks(int eventNumber, List<Ride> rides)
        {
            var eligible = rides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            int lastRank = 0;
            double? lastTime = null;

            for (int i = 0; i < eligible.Count; i++)
            {
                var current = eligible[i];
                var time = current.TotalSeconds;

                int rank;
                if (i == 0)
                {
                    rank = 1;
                }
                else if (Nullable.Equals(time, lastTime))
                {
                    rank = lastRank;
                }
                else
                {
                    rank = i + 1;
                }

                current.EventRank = rank;
                lastRank = rank;
                lastTime = time;
            }

            return eligible.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return AssignRanks(eventNumber, eventRides);
        }
    }
}
