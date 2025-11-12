using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class AllRiderEventRankCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride ride) => ride.Eligibility == RideEligibility.Valid;
        protected override void AssignRank(Ride ride, int rank) => ride.EventRank = rank;
    }
}
