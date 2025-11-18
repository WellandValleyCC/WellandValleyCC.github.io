using ClubCore.Models.Enums;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class RideHelperTests
    {
        [Fact]
        public void LoadRidesFromCsv_ParsesCompetitorsAndGuestsCorrectly()
        {
            // Assemble: invented competitors
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
2001,Thorne,Elijah,FirstClaim,false,Juvenile,
2002,Marwick,Lila,FirstClaim,true,Junior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with competitor + guest
            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,2001,Valid,1,1,1520,Elijah Thorne
1,2002,DNF,,,,Lila Marwick
1,,DNS,,,,Guest Rider";

            // Act
            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            // Assert
            rides.Should().HaveCount(3);

            var elijahRide = rides.Single(r => r.ClubNumber == 2001);
            elijahRide.Competitor.Should().NotBeNull();
            elijahRide.Status.Should().Be(RideStatus.Valid);
            elijahRide.EventRank.Should().Be(1);

            var lilaRide = rides.Single(r => r.ClubNumber == 2002);
            lilaRide.Competitor.Should().NotBeNull();
            lilaRide.Status.Should().Be(RideStatus.DNF);

            var guestRide = rides.Single(r => r.ClubNumber == null);
            guestRide.Competitor.Should().BeNull();
            guestRide.Status.Should().Be(RideStatus.DNS);
        }

        [Fact]
        public void LoadRidesFromCsv_InvalidEligibility_ShouldThrow()
        {
            // Assemble: invented competitor
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
2001,Thorne,Elijah,FirstClaim,false,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: bad eligibility value
            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds
1,2001,NotAnEligibility,1,1,1500";

            // Act
            Action act = () => CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("*NotAnEligibility*");
        }

        [Fact]
        public void LoadRidesFromCsv_MissingClubNumberForCompetitor_ShouldThrow()
        {
            // Assemble: competitor list
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
2001,Thorne,Elijah,FirstClaim,false,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: ride row with club number that doesn't exist
            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds
1,9999,Valid,1,1,1500";

            // Act
            Action act = () => CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*9999*");
        }
    }
}
