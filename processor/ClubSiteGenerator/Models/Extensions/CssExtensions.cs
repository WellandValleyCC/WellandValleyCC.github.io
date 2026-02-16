using ClubCore.Models;
using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.Models.Extensions
{
    public static class CssExtensions
    {
        /// <summary>
        /// Returns the medal css class for podium ranks for the event
        /// </summary>
        public static string GetEventEligibleRidersRankClass(this Ride ride)
        {
            var medal = ride.EventEligibleRidersRank switch
            {
                1 => Medal.Gold,
                2 => Medal.Silver,
                3 => Medal.Bronze,
                _ => Medal.None
            };

            return medal.ToCssClass();
        }

        /// <summary>
        /// Returns the medal for podium ranks for the road bike rides for the event
        /// </summary>
        public static string GetEventEligibleRoadBikeRidersRankClass(this Ride ride)
        {
            var medal = ride.EventEligibleRoadBikeRidersRank switch
            {
                1 => Medal.Gold,
                2 => Medal.Silver,
                3 => Medal.Bronze,
                _ => Medal.None
            };

            return medal.ToCssClass();
        }

        public static string GetRREligibleRidersRankClass(this Ride ride)
        {
            var medal = ride.RREligibleRidersRank switch
            {
                1 => Medal.Gold,
                2 => Medal.Silver,
                3 => Medal.Bronze,
                _ => Medal.None
            };

            return medal.ToCssClass();
        }

        public static string GetRREligibleRoadBikeRidersRankClass(this Ride ride)
        {
            var medal = ride.RREligibleRoadBikeRidersRank switch
            {
                1 => Medal.Gold,
                2 => Medal.Silver,
                3 => Medal.Bronze,
                _ => Medal.None
            };

            return medal.ToCssClass();
        }

        /// <summary>
        /// Returns the CSS class name for the given medal.
        /// </summary>
        public static string ToCssClass(this Medal medal)
        {
            return medal == Medal.None
                ? string.Empty
                : medal.ToString().ToLowerInvariant();
        }
    }
}
