using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using EventProcessor.Tests.Helpers; // for TestCompetitors
using FluentAssertions;
using System.Text.RegularExpressions;

namespace EventProcessor.Tests
{
    public class VetsBucketIntegrationTests
    {
        // ageGroup, vetsBucket (nullable), expectException
        [Theory]
        [InlineData(AgeGroup.Veteran, 1, false)]   // veteran with bucket -> OK
        [InlineData(AgeGroup.Veteran, null, true)]// veteran without bucket -> should throw
        [InlineData(AgeGroup.Senior, null, false)] // non-veteran without bucket -> OK
        [InlineData(AgeGroup.Senior, 5, true)]     // non-veteran with bucket -> should throw
        [InlineData(AgeGroup.Junior, 2, true)]     // non-veteran with bucket -> should throw
        [InlineData(AgeGroup.Juvenile, null, false)] // juvenile without bucket -> OK
        public void Competitor_VetsBucketValidation_ByAgeGroup(
            AgeGroup ageGroup,
            int? vetsBucket,
            bool expectException)
        {
            // Arrange

            // Act
            Action act = () => {
                var comp = new Competitor
                {
                    Id = 0,
                    ClubNumber = 9999,
                    Surname = "Smith",
                    GivenName = "Alex",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = ageGroup,
                    VetsBucket = vetsBucket,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                };
            };

            // Assert
            if (expectException)
            {
                act.Should()
                   .Throw<ArgumentException>();
                   //.WithMessage("*vetsBucket*");
            }
            else
            {
                act.Should().NotThrow();
            }
        }

        // Local validation that mirrors the rule you want enforced.
        // Replace or remove this if you move the guard into Competitor.Validate()
        private static void ValidateVetsBucket(Competitor c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            if (c.VetsBucket.HasValue && !c.IsVeteran)
                throw new ArgumentException("vetsBucket may only be specified for veteran competitors", nameof(c.VetsBucket));
        }

        [Fact]
        public void DemoteVeteranAndPromoteSenior_WhenVetsBucketCleared_Succeeds()
        {
            // Arrange
            var competitor = new Competitor
            {
                ClubNumber = 1234,
                Surname = "Brown",
                GivenName = "Charlie",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,

                AgeGroup = AgeGroup.Veteran,
                VetsBucket = 3
            };

            // Act
            competitor.AgeGroup = AgeGroup.Undefined;     // Clear the veteran flag
            competitor.VetsBucket = null;                 // Clear bucket before setting a non-veteran age group
            competitor.AgeGroup = AgeGroup.Senior;      // Set to senior

            // Assert
            competitor.IsVeteran.Should().BeFalse();
            competitor.IsSenior.Should().BeTrue();
            competitor.VetsBucket.Should().BeNull();
        }
    }
}
