using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubProcessor.Calculators
{
    public class EventRankRoadBikesCalculator : BaseEventRankCalculator
    {
        protected override bool IsEligible(Ride ride) => 
            ride.Status == RideStatus.Valid && 
            ride.IsRoadBike;
        protected override void AssignRank(Ride ride, int rank) => ride.EventRoadBikeRank = rank;
    }
}