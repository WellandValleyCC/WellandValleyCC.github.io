using ClubCore.Models;
using ClubCore.Models.Enums;

namespace EventProcessor.Tests.Helpers
{
    public static class RideFactory
    {
        /// <summary>
        /// Create a Ride consistent with EventImporter.ParseRide output.
        /// Matches the properties the importer sets from CSV:
        /// EventNumber, Name, ClubNumber (nullable), ActualTime, TotalSeconds, IsRoadBike, Eligibility.
        /// </summary>
        public static Ride Create(
            int eventNumber,
            string? numberOrName,                // either club number string "9999" or a name "Johnny Doe"
            string? name,
            double totalSeconds,
            bool isRoadBike = false,
            RideStatus eligibility = RideStatus.Valid,
            string? actualTime = null,
            double? avgSpeed = null,
            int? eventRank = null)
        {
            bool isClubMember = int.TryParse(numberOrName?.Trim(), out int clubNumber);

            return new Ride
            {
                EventNumber = eventNumber,
                Name = name ?? (isClubMember ? string.Empty : numberOrName ?? string.Empty),
                ClubNumber = isClubMember ? clubNumber : null,
                TotalSeconds = totalSeconds,
                IsRoadBike = isRoadBike,
                Status = eligibility,
                AvgSpeed = avgSpeed,
                EventRank = eventRank
            };
        }

        /// <summary>
        /// Convenience overload for a club-member rider identified by club number.
        /// </summary>
        public static Ride CreateClubMemberRide(
            int eventNumber,
            int clubNumber,
            string? name,
            double totalSeconds,
            bool isRoadBike = false,
            RideStatus eligibility = RideStatus.Valid,
            string? actualTime = null,
            double? avgSpeed = null,
            int? eventRank = null)
        {
            return Create(
                eventNumber,
                clubNumber.ToString(),
                name,
                totalSeconds,
                isRoadBike,
                eligibility,
                actualTime,
                avgSpeed,
                eventRank);
        }

        /// <summary>
        /// Convenience overload for a guest rider (no ClubNumber).
        /// </summary>
        public static Ride CreateGuestRide(
            int eventNumber,
            string guestName,
            double totalSeconds,
            bool isRoadBike = false,
            RideStatus eligibility = RideStatus.Valid,
            string? actualTime = null,
            double? avgSpeed = null,
            int? eventRank = null)
        {
            return Create(
                eventNumber,
                guestName,
                guestName,
                totalSeconds,
                isRoadBike,
                eligibility,
                actualTime,
                avgSpeed,
                eventRank);
        }
    }
}
