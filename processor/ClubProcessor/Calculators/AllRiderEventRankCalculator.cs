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

            for (int i = 0; i < eligible.Count; i++)
            {
                eligible[i].EventRank = i + 1;
            }

            return eligible.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return AssignRanks(eventNumber, eventRides);
        }
    }
}
