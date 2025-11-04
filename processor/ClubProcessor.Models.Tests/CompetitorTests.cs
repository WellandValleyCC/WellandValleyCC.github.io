using AutoFixture;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using FluentAssertions;

public class CompetitorTests
{
    private readonly Fixture _fixture = new();

    [Theory]
    [InlineData("Ian", "Allen", "Ian Allen")]
    [InlineData(" Ian ", "Allen", "Ian Allen")]
    [InlineData("Ian", " Allen ", "Ian Allen")]
    [InlineData("", "Allen", "Allen")]
    [InlineData("Ian", "", "Ian")]
    [InlineData("", "", "")]
    public void FullName_ConcatenatesGivenNameAndSurname(string given, string surname, string expected)
    {
        // Assemble
        var competitor = _fixture.Build<Competitor>()
            .With(c => c.GivenName, given)
            .With(c => c.Surname, surname)
            // set fields which would otherwise fail validation
            .With(c => c.AgeGroup, AgeGroup.Senior)
            .With(c => c.VetsBucket, (int?)null)
            // ----------
            .Create();

        // Act
        var fullName = competitor.FullName;

        // Assert
        fullName.Should().Be(expected);
    }

    [Theory]
    [InlineData("Ian Allen", "Ian Allen", true)]
    [InlineData("ian allen", "Ian Allen", true)]
    [InlineData("IanAllen", "Ian Allen", true)]
    [InlineData("Allen Ian", "Ian Allen", false)]
    [InlineData("Ian", "Ian Allen", false)]
    [InlineData("", "Ian Allen", false)]
    public void MatchesName_ReturnsExpectedResult(string input, string fullName, bool expected)
    {
        // Assemble
        var parts = fullName.Split(' ');
        var competitor = _fixture.Build<Competitor>()
            .With(c => c.GivenName, parts.ElementAtOrDefault(0) ?? "")
            .With(c => c.Surname, parts.ElementAtOrDefault(1) ?? "")
            // set fields which would otherwise fail validation
            .With(c => c.AgeGroup, AgeGroup.Senior)
            .With(c => c.VetsBucket, (int?)null)
            // ----------
            .Create();

        // Act
        var result = competitor.MatchesName(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Validate_ThrowsIfClaimStatusIsUnknown()
    {
        var competitor = _fixture.Build<Competitor>()
            .With(c => c.ClaimStatus, ClaimStatus.Unknown)
            // set fields which would otherwise fail validation
            .With(c => c.AgeGroup, AgeGroup.Senior)
            .With(c => c.VetsBucket, (int?)null)
            // ----------
            .Create();

        // Act
        Action act = () => competitor.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*ClaimStatus must be explicitly set*");
    }

}
