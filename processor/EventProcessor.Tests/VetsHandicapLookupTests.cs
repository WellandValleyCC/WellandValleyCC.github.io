using System;
using FluentAssertions;
using Xunit;
using ClubProcessor.Services;

namespace ClubProcessor.Tests.Services
{
    public class VetsHandicapLookupTests
    {
        private readonly VetsHandicapLookup _provider = VetsHandicapLookup.ForSeason(2025); // uses 2024 standards

        [Theory]
        [InlineData(2025, 5.0, false, 1)] // Male, bucket 1
        [InlineData(2025, 5.0, true, 1)]  // Female, bucket 1
        [InlineData(2025, 25.0, false, 10)]
        [InlineData(2025, 0.88, true, 5)]
        public void GetHandicapSeconds_ReturnsExpected_ForValidInputs(int year, double distance, bool isFemale, int bucket)
        {
            var seconds = _provider.GetHandicapSeconds(year, distance, isFemale, bucket);
            seconds.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(2025, 0.5)]
        [InlineData(2025, 99.0)]
        [InlineData(2025, 12.3)]
        public void GetHandicapSeconds_Throws_ForUnsupportedDistance(int year, double distance)
        {
            Action act = () => _provider.GetHandicapSeconds(year, distance, false, 1);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*Unsupported distance*");
        }

        [Theory]
        [InlineData(2025, 0)]
        [InlineData(2025, 41)]
        [InlineData(2025, -5)]
        public void GetHandicapSeconds_Throws_ForOutOfRangeBucket(int year, int bucket)
        {
            Action act = () => _provider.GetHandicapSeconds(year, 5.0, false, bucket);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*vetsBucket must be between*");
        }

        [Fact]
        public void GetHandicapSeconds_Throws_IfBucketNotMapped()
        {
            Action act = () => _provider.GetHandicapSeconds(2025, 2.0, false, 40);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("Unsupported distance*");
        }
    }
}
