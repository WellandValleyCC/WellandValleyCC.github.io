using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class AllRiderEventRankCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride ride) => ride.Eligibility == RideEligibility.Valid;
        protected override void AssignRank(Ride ride, int rank) => ride.EventRank = rank;
        protected override void ClearRank(Ride ride) => ride.EventRank = null;
    }
}
