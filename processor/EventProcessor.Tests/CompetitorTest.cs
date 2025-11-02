using FluentAssertions;
using EventProcessor.Tests.Helpers; // for TestCompetitors

namespace EventProcessor.Tests
{
    public class VetsBucketIntegrationTests
    {
        [Fact]
        public void OnlyVeterans_have_VetsBucket_and_allVeterans_have_bucket_ge_1()
        {
            // Arrange
            var competitors = TestCompetitors.All;

            // Act / Assert
            // Veterans must have VetsBucket >= 1
            var veterans = competitors.Where(c => c.AgeGroup == AgeGroup.IsVeteran).ToList();
            veterans.Should().NotBeEmpty("test data contains veteran competitors to validate");

            foreach (var vet in veterans)
            {
                var bucket = vet.VetsBucket;
                bucket.Should().HaveValue($"competitor {vet.ClubNumber} ({vet.Surname}) is in AgeGroup.IsVeteran and must have a VetsBucket");
                int value = bucket!.Value;
                value.Should().BeGreaterOrEqualTo(1, $"VetsBucket for veteran {vet.ClubNumber} must be >= 1");
            }

            // Non-veterans must not have a VetsBucket
            var nonVeterans = competitors.Where(c => c.AgeGroup != AgeGroup.IsVeteran).ToList();
            nonVeterans.Should().NotBeEmpty("test data contains non-veteran competitors to validate");

            foreach (var nonVet in nonVeterans)
            {
                nonVet.VetsBucket.Should().BeNull($"competitor {nonVet.ClubNumber} ({nonVet.Surname}) is not a veteran and must not have a VetsBucket");
            }
        }
    }
}
