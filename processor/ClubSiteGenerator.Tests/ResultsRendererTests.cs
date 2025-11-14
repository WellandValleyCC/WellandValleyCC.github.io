using AutoFixture;
using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Services;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class ResultsRendererTests
    {
        private readonly Fixture _fixture = new();

        [Theory]
        [InlineData(1, "position-1")]
        [InlineData(2, "position-2")]
        [InlineData(3, "position-3")]
        [InlineData(4, "")]
        [InlineData(null, "")]
        public void GetPodiumClass_ForEligibleRidersRank_ReturnsExpectedCss(int? rank, string expectedCss)
        {
            // Arrange
            var ride = new Ride { EventEligibleRidersRank = rank };

            // Act
            var result = ResultsRenderer.GetPodiumClass(rank, ride);

            // Assert
            result.Should().Be(expectedCss);
        }

        [Theory]
        [InlineData(1, "position-1")]
        [InlineData(2, "position-2")]
        [InlineData(3, "position-3")]
        [InlineData(5, "")]
        [InlineData(null, "")]
        public void GetPodiumClass_ForEligibleRoadBikeRidersRank_ReturnsExpectedCss(int? rank, string expectedCss)
        {
            // Arrange
            var ride = new Ride { EventEligibleRoadBikeRidersRank = rank };

            // Act
            var result = ResultsRenderer.GetPodiumClass(rank, ride);

            // Assert
            result.Should().Be(expectedCss);
        }

        [Theory]
        [InlineData(null, null, ClaimStatus.Unknown,     RideEligibility.Valid, "guest-non-club-member")]
        [InlineData(null, 124,  ClaimStatus.SecondClaim, RideEligibility.Valid, "guest-second-claim")]
        [InlineData(1, 123,     ClaimStatus.FirstClaim,  RideEligibility.Valid, "competition-eligible")]

        [InlineData(null, null, ClaimStatus.Unknown,     RideEligibility.DNF, "guest-non-club-member")]
        [InlineData(null, 123,  ClaimStatus.SecondClaim, RideEligibility.DNF, "guest-second-claim")]
        [InlineData(null, 123,  ClaimStatus.FirstClaim,  RideEligibility.DNF, "competition-eligible")]
        public void GetRowClass_ReturnsExpectedCss(int? eligibleRank, int? clubNumber, ClaimStatus claimStatus, RideEligibility rideEligibility, string expectedCss)
        {
            // Arrange
            Competitor competitor = null;

            if (clubNumber != null)
            {
                competitor = new Competitor
                {
                    ClubNumber = (int)clubNumber!,
                    ClaimStatus = claimStatus,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,           // again, pick a valid enum/value
                    Surname = "Smith",
                    GivenName = "John",
                    VetsBucket = null
                };
            };


            var ride = new Ride
            {
                EventEligibleRidersRank = eligibleRank,
                Competitor = competitor,
                ClubNumber = clubNumber,
                Eligibility = rideEligibility
            };

            // Act
            var result = ResultsRenderer.GetRowClass(ride);

            // Assert
            result.Should().Be(expectedCss);
        }
    }
}
