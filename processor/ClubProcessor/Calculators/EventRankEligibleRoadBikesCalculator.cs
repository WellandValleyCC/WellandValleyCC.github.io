using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class EventRankEligibleRoadBikesCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride r) =>
            r.IsRoadBike &&
            r.Competitor?.IsEligible() == true &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignRank(Ride ride, int rank) => ride.EventEligibleRoadBikeRidersRank = rank;
    }
}
