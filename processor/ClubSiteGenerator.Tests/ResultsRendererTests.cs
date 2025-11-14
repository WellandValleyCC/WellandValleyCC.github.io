using AutoFixture;
using FluentAssertions;
using ClubCore.Models;
using ClubSiteGenerator.Services;

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
    }
}
