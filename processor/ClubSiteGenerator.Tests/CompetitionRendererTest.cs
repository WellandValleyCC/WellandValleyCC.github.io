using ClubCore.Models;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class CompetitionRendererTest
    {
        [Fact]
        public void Render_ShouldIncludeCompetitionTitleAndCode()
        {
            // Arrange: minimal calendar + competitors
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "25 Mile", IsEvening10 = false }
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,900,Alice Smith
2,1,DNS,,,,Alice Smith";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar);
            var renderer = new CompetitionRenderer(resultsSet, calendar);

            // Act
            var html = renderer.Render();

            // Assert: check key elements
            html.Should().Contain("<title>")
                .And.Contain("Alice Smith")
                .And.Contain("60"); // points from position 1 in points table
        }

        [Fact]
        public void Render_ShouldIncludeLegendAndCssClasses()
        {
            // Arrange: minimal calendar + competitors
            var calendar = new[]
            {
            new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
            new CalendarEvent { EventNumber = 2, EventName = "25 Mile", IsEvening10 = false }
        };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Jones,Bob,FirstClaim,false,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,,Bob Jones";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar);
            var renderer = new CompetitionRenderer(resultsSet, calendar);

            // Act
            var html = renderer.Render();

            // Assert legend and CSS classes
            html.Should().Contain("legend")
                .And.Contain("ten-mile-event")
                .And.Contain("non-ten-mile-event");
        }

        [Fact]
        public void Render_ShouldIncludeStatusAndPoints()
        {
            // Arrange: minimal calendar + competitors
            var calendar = new[]
            {
        new CalendarEvent { EventNumber = 1, EventName = "Event 1", IsEvening10 = true },
        new CalendarEvent { EventNumber = 2, EventName = "Event 2", IsEvening10 = false },
        new CalendarEvent { EventNumber = 3, EventName = "Event 3", IsEvening10 = false }
    };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,,Alice Smith
2,1,DNS,,,,Alice Smith";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar);
            var renderer = new CompetitionRenderer(resultsSet, calendar);

            // Act
            var html = renderer.Render();

            // Assert: check key elements
            html.Should().Contain("Alice Smith"); // competitor name
            html.Should().Contain("60");          // points for Event 1
        }
    }
}
