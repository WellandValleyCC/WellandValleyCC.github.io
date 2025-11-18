using ClubCore.Models.Enums;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class CompetitorHelperTests
    {
        [Fact]
        public void LoadCompetitorsFromCsv_ParsesInventedNamesCorrectly()
        {
            // Assemble: human-readable invented CSV
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
2001,Thorne,Elijah,FirstClaim,false,Juvenile,
2002,Marwick,Lila,FirstClaim,true,Junior,
2003,Fenwick,Orson,SecondClaim,false,Senior,
2004,Greaves,Amara,FirstClaim,true,Senior,
2005,Hollis,Benedict,FirstClaim,false,Veteran,12
2006,Rowntree,Sylvie,FirstClaim,true,Veteran,7";

            // Act: load via test utility
            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assert: check parsing correctness
            competitors.Should().HaveCount(6);

            var elijah = competitors.Single(c => c.ClubNumber == 2001);
            elijah.Surname.Should().Be("Thorne");
            elijah.GivenName.Should().Be("Elijah");
            elijah.ClaimStatus.Should().Be(ClaimStatus.FirstClaim);
            elijah.IsFemale.Should().BeFalse();
            elijah.AgeGroup.Should().Be(AgeGroup.Juvenile);
            elijah.VetsBucket.Should().BeNull();

            var sylvie = competitors.Single(c => c.ClubNumber == 2006);
            sylvie.Surname.Should().Be("Rowntree");
            sylvie.IsFemale.Should().BeTrue();
            sylvie.AgeGroup.Should().Be(AgeGroup.Veteran);
            sylvie.VetsBucket.Should().Be(7);
        }

        [Fact]
        public void LoadCompetitorsFromCsv_MissingColumn_ShouldThrow()
        {
            // Assemble: CSV missing the VetsBucket column
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup
3002,Invented,MissingColumn,FirstClaim,true,Senior";

            // Act
            Action act = () => CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assert
            act.Should().Throw<IndexOutOfRangeException>();
        }
    }
}
