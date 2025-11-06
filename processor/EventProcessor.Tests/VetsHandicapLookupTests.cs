using System;
using FluentAssertions;
using Xunit;
using ClubProcessor.Services;

namespace ClubProcessor.Tests.Services
{
    public class VetsHandicapLookupTests
    {
        private readonly VetsHandicapLookup _provider = new();

        [Theory]
        [InlineData(5.0, false, 1)] // Male, bucket 1
        [InlineData(5.0, true, 1)]  // Female, bucket 1
        [InlineData(25.0, false, 10)]
        [InlineData(0.88, true, 5)]
        public void GetHandicapSeconds_ReturnsExpected_ForValidInputs(double distance, bool isFemale, int bucket)
        {
            var seconds = _provider.GetHandicapSeconds(distance, isFemale, bucket);
            seconds.Should().BeGreaterThan(0);
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(99.0)]
        [InlineData(12.3)]
        public void GetHandicapSeconds_Throws_ForUnsupportedDistance(double distance)
        {
            Action act = () => _provider.GetHandicapSeconds(distance, false, 1);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*Unsupported distance*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(41)]
        [InlineData(-5)]
        public void GetHandicapSeconds_Throws_ForOutOfRangeBucket(int bucket)
        {
            Action act = () => _provider.GetHandicapSeconds(5.0, false, bucket);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("*vetsBucket must be between*");
        }

        [Fact]
        public void GetHandicapSeconds_Throws_IfBucketNotMapped()
        {
            Action act = () => _provider.GetHandicapSeconds(2.0, false, 40);
            act.Should().Throw<ArgumentOutOfRangeException>()
               .WithMessage("Unsupported distance*");
        }
    }
}
