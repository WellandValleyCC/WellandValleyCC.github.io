using ClubCore.Models;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Tests.Helpers; // CsvTestLoader
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class JuvenilesCompetitionTests
    {
        [Fact]
        public void AssignRanks_TieAware_StopAtNull()
        {
            // Arrange
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10 TT", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "25 Mile TT", IsEvening10 = false },
                new CalendarEvent { EventNumber = 3, EventName = "Hill Climb 5 Mile", IsEvening10 = false }
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1001,Smith,Alice,FirstClaim,false,Juvenile,
1002,Jones,Bob,FirstClaim,false,Juvenile,
1003,Adams,Cara,FirstClaim,true,Juvenile,
1004,Brown,Dan,FirstClaim,false,Juvenile,
1005,Green,Emily,FirstClaim,true,Juvenile,
1006,White,Frank,FirstClaim,false,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1001,Valid,1,,905,Alice Smith
2,1001,Valid,1,,1900,Alice Smith
3,1001,Valid,2,,2500,Alice Smith
1,1002,Valid,1,,905,Bob Jones
2,1002,Valid,2,,1910,Bob Jones
3,1002,Valid,1,,2490,Bob Jones
1,1003,Valid,3,,915,Cara Adams
2,1003,Valid,3,,1920,Cara Adams
3,1003,Valid,4,,2520,Cara Adams
1,1004,Valid,4,,920,Dan Brown
2,1004,Valid,4,,1930,Dan Brown
3,1004,DNS,,,,Dan Brown
1,1005,Valid,5,,1950,Emily Green
3,1005,Valid,5,,2550,Emily Green"; // DanBrown only has one valid non-ten-mile ride.  EmilyGreen has only one non-ten ride.

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var groups = rides.Where(r => r.Competitor != null).GroupBy(r => r.Competitor!);

            var results = groups
                .Select(g => CompetitionResultsCalculator.BuildCompetitorResult(g, calendar))
                .ToList();

            // Act  
            results = CompetitionResultsCalculator.SortResults(results).ToList();


            // Assert
            results.Single(r => r.Competitor.Surname == "Smith").FullCompetition.Rank.Should().Be(1);
            results.Single(r => r.Competitor.Surname == "Jones").FullCompetition.Rank.Should().Be(1);
            results.Single(r => r.Competitor.Surname == "Adams").FullCompetition.Rank.Should().Be(3);
            results.Single(r => r.Competitor.Surname == "Brown").FullCompetition.Rank.Should().BeNull("because Dan Brown has two non-ten-mile rides, but one is a DNS");
            results.Single(r => r.Competitor.Surname == "Green").FullCompetition.Rank.Should().BeNull("because Emily has not done two non-ten events");
        }

        [Theory]
        [InlineData(30.5, "31")]
        [InlineData(30.4, "30")]
        [InlineData(null, "n/a")]
        public void Best8TenMileDisplay_RoundsOrShowsNa(double? input, string expected)
        {
            var result = new CompetitorResult
            {
                TenMileCompetition = new CompetitionScore { Points = input }
            };
            result.TenMileCompetition.PointsDisplay.Should().Be(expected);
        }

        [Fact]
        public void SortResults_TieredOrdering()
        {
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,
2,Clark,Dan,FirstClaim,false,Juvenile,
3,Brown,Bob,FirstClaim,false,Juvenile,
4,Adams,Zoe,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,600,Alice Smith
1,2,Valid,2,,700,Dan Clark
1,3,Valid,3,,800,Bob Brown";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var groups = rides.Where(r => r.Competitor != null).GroupBy(r => r.Competitor!);
            var results = groups
                .Select(g => CompetitionResultsCalculator.BuildCompetitorResult(g, calendar))
                .ToList();

            var ordered = CompetitionResultsCalculator.SortResults(results).ToList();

            // None are eligible for the full competition, so results should be in 10-mile events order
            ordered[0].Competitor.FullName.Should().Be("Alice Smith");
            ordered[1].Competitor.FullName.Should().Be("Dan Clark");
            ordered[2].Competitor.FullName.Should().Be("Bob Brown");

        }

        [Fact]
        public void SortResults_TieredOrdering_AssignsBothRanks()
        {
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10 TT", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "25 Mile TT", IsEvening10 = false },
                new CalendarEvent { EventNumber = 3, EventName = "Hill Climb 5 Mile", IsEvening10 = false }
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,
2,Clark,Dan,FirstClaim,false,Juvenile,
3,Brown,Bob,FirstClaim,false,Juvenile,
4,Davidson,Sally,FirstClaim,true,Juvenile,
5,Adams,Zoe,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,2,,700,Alice Smith
1,2,Valid,3,,800,Dan Clark
1,3,Valid,4,,900,Bob Brown
1,4,Valid,1,,650,Sally Davidson
2,1,Valid,4,,900,Alice Smith
2,2,Valid,3,,800,Dan Clark
2,3,Valid,2,,700,Bob Brown
2,4,Valid,1,,650,Sally Davidson
3,1,Valid,4,,900,Alice Smith
3,2,Valid,3,,800,Dan Clark
3,3,Valid,2,,700,Bob Brown
3,4,Valid,1,,650,Sally Davidson";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var groups = rides.Where(r => r.Competitor != null).GroupBy(r => r.Competitor!);
            var results = groups
                .Select(g => CompetitionResultsCalculator.BuildCompetitorResult(g, calendar))
                .ToList();

            // Add Zoe manually with no rides
            results.Add(new CompetitorResult { Competitor = competitors.Single(c => c.Surname == "Adams") });

            // Act: order by full competition
            var ordered = CompetitionResultsCalculator.SortResults(results).ToList();
            
            // Assert: full competition ordering
            ordered[0].Competitor.FullName.Should().Be("Sally Davidson");
            ordered[1].Competitor.FullName.Should().Be("Bob Brown");
            ordered[2].Competitor.FullName.Should().Be("Dan Clark");
            ordered[3].Competitor.FullName.Should().Be("Alice Smith");
            ordered[4].Competitor.FullName.Should().Be("Zoe Adams");

            // Assert: ranks for full competition
            ordered[0].FullCompetition.Rank.Should().Be(1);
            ordered[1].FullCompetition.Rank.Should().Be(2);
            ordered[2].FullCompetition.Rank.Should().Be(3);
            ordered[3].FullCompetition.Rank.Should().Be(4);
            ordered[4].FullCompetition.Rank.Should().Be(null);

            // Assert: ranks for tens competition
            ordered[0].TenMileCompetition.Rank.Should().Be(1);
            ordered[1].TenMileCompetition.Rank.Should().Be(4);
            ordered[2].TenMileCompetition.Rank.Should().Be(3);
            ordered[3].TenMileCompetition.Rank.Should().Be(2);
            ordered[4].TenMileCompetition.Rank.Should().BeNull();
        }
    }
}