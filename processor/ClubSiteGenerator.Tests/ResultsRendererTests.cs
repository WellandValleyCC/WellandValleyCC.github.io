using AutoFixture;
using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Renderers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Emit;

namespace ClubSiteGenerator.Tests
{
    public class ResultsRendererTests
    {
        private readonly Fixture _fixture = new();

        [Theory]
        [InlineData(null, null, ClaimStatus.Unknown,     RideStatus.Valid, "guest-non-club-member")]
        [InlineData(null, 124,  ClaimStatus.SecondClaim, RideStatus.Valid, "guest-second-claim")]
        [InlineData(1, 123,     ClaimStatus.FirstClaim,  RideStatus.Valid, "competition-eligible")]

        [InlineData(null, null, ClaimStatus.Unknown,     RideStatus.DNF, "guest-non-club-member")]
        [InlineData(null, 123,  ClaimStatus.SecondClaim, RideStatus.DNF, "guest-second-claim")]
        [InlineData(null, 123,  ClaimStatus.FirstClaim,  RideStatus.DNF, "competition-eligible")]
        public void GetRowClass_ReturnsExpectedCss(int? eligibleRank, int? clubNumber, ClaimStatus claimStatus, RideStatus rideEligibility, string expectedCss)
        {
            // Arrange
            Competitor? competitor = null;

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
                Status = rideEligibility
            };

            // Act
            var result = EventRenderer.GetRowClass(ride);

            // Assert
            result.Should().Be(expectedCss);
        }
    }
}
