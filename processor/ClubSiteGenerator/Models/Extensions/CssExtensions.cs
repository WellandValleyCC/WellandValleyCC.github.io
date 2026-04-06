using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.Models.Extensions
{
    public static class CssExtensions
    {
        /// <summary>
        /// Returns the medal CSS class for podium ranks for the event.
        /// </summary>
        public static string GetEventEligibleRidersRankClass(this Ride ride)
            => MedalClass(ride.EventEligibleRidersRank, ride);

        /// <summary>
        /// Returns the medal CSS class for podium ranks for the road bike category.
        /// </summary>
        public static string GetEventEligibleRoadBikeRidersRankClass(this Ride ride)
            => MedalClass(ride.EventEligibleRoadBikeRidersRank, ride);

        /// <summary>
        /// Returns the medal CSS class for podium ranks for the round-robin event.
        /// </summary>
        public static string GetRREligibleRidersRankClass(this Ride ride)
            => MedalClass(ride.RREligibleRidersRank, ride);

        /// <summary>
        /// Returns the medal CSS class for podium ranks for the round-robin road bike category.
        /// </summary>
        public static string GetRREligibleRoadBikeRidersRankClass(this Ride ride)
            => MedalClass(ride.RREligibleRoadBikeRidersRank, ride);

        /// <summary>
        /// Converts a medal enum to its CSS class name.
        /// </summary>
        public static string ToCssClass(this Medal medal)
        {
            return medal == Medal.None
                ? string.Empty
                : medal.ToString().ToLowerInvariant();
        }

        /// <summary>
        /// Shared helper that determines the correct medal class for a given rank.
        /// </summary>
        private static string MedalClass(int? rank, Ride ride)
        {
            var medal =
                !ride.HasResult()
                    ? Medal.None
                    : rank switch
                    {
                        1 => Medal.Gold,
                        2 => Medal.Silver,
                        3 => Medal.Bronze,
                        _ => Medal.None
                    };

            return medal.ToCssClass();
        }

        /// <summary>
        /// Determines whether the ride has a real result (i.e., the rider has ridden).
        /// </summary>
        private static bool HasResult(this Ride ride)
        {
            return ride.Status == RideStatus.Valid &&
                   ride.TotalSeconds > 0;
        }
    }
}
