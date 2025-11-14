using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class EventRankEligibleRidersCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride r) =>
            r.Competitor?.IsEligible() == true &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignRank(Ride ride, int rank) => ride.EventEligibleRidersRank = rank;
    }
}
