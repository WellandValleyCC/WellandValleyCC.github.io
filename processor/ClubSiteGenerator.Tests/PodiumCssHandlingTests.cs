using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AutoFixture;
using ClubCore.Models;
using ClubSiteGenerator.Models.Extensions;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class PodiumCssHandlingTests
    {
        private static IHtmlDocument ParseHtml(string html)
        {
            var parser = new HtmlParser();
            return parser.ParseDocument(html);
        }

        [Theory]
        [InlineData(1, "gold")]
        [InlineData(2, "silver")]
        [InlineData(3, "bronze")]
        [InlineData(null, "")]
        [InlineData(4, "")]
        public void RideExtension_GetEventRankMedal_ReturnsExpectedClass(int? rank, string expected)
        {
            // Arrange: dummy ride
            var ride = new Ride { EventEligibleRidersRank = rank };

            // Act
            var cssClass = ride.GetEventEligibleRidersRankClass();

            // Assert
            cssClass.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, "gold")]
        [InlineData(2, "silver")]
        [InlineData(3, "bronze")]
        [InlineData(null, "")]
        [InlineData(4, "")]
        public void RideExtension_GetEventRoadBikeRankMedal_ReturnsExpectedClass(int? rank, string expected)
        {
            // Arrange: dummy ride
            var ride = new Ride { EventEligibleRoadBikeRidersRank = rank };

            // Act
            var cssClass = ride.GetEventEligibleRoadBikeRidersRankClass();

            // Assert
            cssClass.Should().Be(expected);
        }

        [Theory]
        [InlineData(1, "gold")]
        [InlineData(2, "silver")]
        [InlineData(3, "bronze")]
        [InlineData(null, "")]
        [InlineData(4, "")]
        public void CompetitionRenderer_RendersScoring11Cell_WithExpectedCssClass(int? expectedRank, string expectedClass)
        {
            var rules = new CompetitionRules(tenMileCount: 4, nonTenMinimum: 2, mixedEventCount: 3);

            // Arrange:
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "25 miler", IsEvening10 = false },
                new CalendarEvent { EventNumber = 3, EventName = "Another 25 miler", IsEvening10 = false },
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,
2,Jones,Bob,FirstClaim,false,Juvenile,
3,Brown,Charlie,FirstClaim,false,Juvenile,
4,Adams,Charlie,FirstClaim,false,Juvenile,
5,Johnson,Zoe,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,500,Alice Smith
1,2,Valid,2,,600,Bob Jones
1,3,Valid,3,,700,Charlie Brown
1,4,Valid,4,,800,Charlie Adams
2,1,Valid,1,,500,Alice Smith
2,2,Valid,2,,600,Bob Jones
2,3,Valid,3,,700,Charlie Brown
2,4,Valid,4,,800,Charlie Adams
3,1,Valid,1,,500,Alice Smith
3,2,Valid,2,,600,Bob Jones
3,3,Valid,3,,700,Charlie Brown
3,4,Valid,4,,800,Charlie Adams
2,5,Valid,1,,1000,Zoe Johnson";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar, rules);
            var renderer = new CompetitionRenderer(resultsSet, rules);

            var competitor = resultsSet.ScoredRides
                .FirstOrDefault(r => r.TenMileCompetition.Rank == expectedRank)?.Competitor
                ?? competitors.First();

            // Act
            var html = renderer.Render();

            // Assert
            var doc = ParseHtml(html);

            var row = doc.QuerySelectorAll("tr")
                .FirstOrDefault(tr => tr.TextContent.Contains(competitor.FullName));

            row.Should().NotBeNull();

            // get all <td> cells in this row
            var cells = row!.QuerySelectorAll("td");

            // defensive check
            cells.Length.Should().BeGreaterThan(4);

            // pick the 6th cell (index 5)
            var tdClass = cells[5].GetAttribute("class") ?? string.Empty;

            if (string.IsNullOrEmpty(expectedClass))
            {
                // Assert that none of the medal classes are present
                tdClass.Should().NotContain("gold")
                       .And.NotContain("silver")
                       .And.NotContain("bronze");
            }
            else
            {
                // Expect the podium class to be present
                tdClass.Should().Contain(expectedClass);
            }
        }

        [Theory]
        [InlineData(1, "gold")]
        [InlineData(2, "silver")]
        [InlineData(3, "bronze")]
        [InlineData(null, "")]
        [InlineData(4, "")]
        public void CompetitionRenderer_Render_IncludesExpectedBest8PodiumCssClass(int? expectedRank, string expectedClass)
        {
            var rules = new CompetitionRules(tenMileCount: 4, nonTenMinimum: 1, mixedEventCount: 3);

            // Arrange:
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "25 miler", IsEvening10 = false }
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,
2,Jones,Bob,FirstClaim,false,Juvenile,
3,Brown,Charlie,FirstClaim,false,Juvenile,
4,Adams,Charlie,FirstClaim,false,Juvenile,
5,Johnson,Zoe,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,500,Alice Smith
1,2,Valid,2,,600,Bob Jones
1,3,Valid,3,,700,Charlie Brown
1,4,Valid,4,,800,Charlie Adams
2,5,Valid,1,,1000,Zoe Johnson";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar, rules);
            var renderer = new CompetitionRenderer(resultsSet, rules);
            
            var competitor = resultsSet.ScoredRides
                .FirstOrDefault(r => r.TenMileCompetition.Rank == expectedRank)?.Competitor
                ?? competitors.First();

            // Act
            var html = renderer.Render();

            // Assert
            var doc = ParseHtml(html);

            var row = doc.QuerySelectorAll("tr")
                .FirstOrDefault(tr => tr.TextContent.Contains(competitor.FullName));

            row.Should().NotBeNull();

            // get all <td> cells in this row
            var cells = row!.QuerySelectorAll("td");

            // defensive check
            cells.Length.Should().BeGreaterThan(6);

            // pick the 7th cell (index 6)
            var tdClass = cells[6].GetAttribute("class") ?? string.Empty;

            if (string.IsNullOrEmpty(expectedClass))
            {
                // Assert that none of the medal classes are present
                tdClass.Should().NotContain("gold")
                       .And.NotContain("silver")
                       .And.NotContain("bronze");
            }
            else
            {
                // Expect the podium class to be present
                tdClass.Should().Contain(expectedClass);
            }
        }
    }
}
