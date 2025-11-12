using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class RoadBikeEventRankCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride ride) => ride.Eligibility == RideEligibility.Valid && ride.IsRoadBike;
        protected override void AssignRank(Ride ride, int rank) => ride.EventRoadBikeRank = rank;
    }
}