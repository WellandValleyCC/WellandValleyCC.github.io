using ClubCore.Models;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Tests.Helpers;
using AngleSharp;
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

        [Fact]
        public async Task Render_ShouldIncludeCompetitorRow_WithCorrectClasses()
        {
            // Arrange: renderer setup (same as your test)
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

            // Parse HTML with AngleSharp
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert: DOM checks
            var cells = document.QuerySelectorAll("table.results tbody tr td");

            // Competitor name cell
            cells.Should().ContainSingle(c => c.TextContent == "Alice Smith");

            // Points cell for Event 1
            cells.Should().ContainSingle(c =>
                c.TextContent == "60" &&
                c.ClassList.Contains("non-ten-mile-event"));

            // DNS placeholder cell
            cells.Should().Contain(c =>
                c.TextContent == "-" &&
                c.ClassList.Contains("non-ten-mile-event"));

            // Footer timestamp
            var footer = document.QuerySelector("footer p.generated");
            footer.Should().NotBeNull();
            footer!.TextContent.Should().MatchRegex(@"Generated .*UTC");
        }

        [Fact]
        public async Task Render_ShouldAlignAllHeaderAndBodyClasses()
        {
            // Arrange: same setup as before
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

            // Parse HTML
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert: header vs body class alignment
            var headers = document.QuerySelectorAll("table.results thead th");
            var rows = document.QuerySelectorAll("table.results tbody tr");

            headers.Should().NotBeEmpty();
            rows.Should().NotBeEmpty();

            // For each column, check that all body cells share the same class as the header
            for (int col = 0; col < headers.Length; col++)
            {
                var expectedClass = headers[col].ClassName;

                foreach (var row in rows)
                {
                    var cell = row.Children[col];
                    cell.ClassName.Should().Be(expectedClass,
                        $"Column '{headers[col].TextContent.Trim()}' should align class between header and body");
                }
            }
        }
    }
}
