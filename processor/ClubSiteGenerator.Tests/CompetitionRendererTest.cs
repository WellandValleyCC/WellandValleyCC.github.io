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
        public async Task Render_ShouldIncludeLegendWithCorrectEntries()
        {
            // Arrange: renderer setup (same as before)
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Event 1 - ten", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "Event 2 - 25mile", IsEvening10 = false },
                new CalendarEvent { EventNumber = 3, EventName = "Event 3 - 9.5mile hardride", IsEvening10 = false }
            };

            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1,Smith,Alice,FirstClaim,true,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1,Valid,1,,,Alice Smith";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);
            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar);
            var renderer = new CompetitionRenderer(resultsSet, calendar);

            // Act
            var html = renderer.Render();

            // Parse HTML
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert: legend structure
            var legend = document.QuerySelector("div.legend");
            legend.Should().NotBeNull("the legend div should be present");

            var spans = legend!.QuerySelectorAll("span");
            spans.Should().HaveCount(2, "legend should contain exactly two entries");

            // Assert: first entry
            spans[0].ClassList.Should().Contain("ten-mile-event");
            spans[0].TextContent.Should().Be("10‑mile events");

            // Assert: second entry
            spans[1].ClassList.Should().Contain("non-ten-mile-event");
            spans[1].TextContent.Should().Be("Other events");
        }

        [Fact]
        public void Render_ShouldIncludeStatusAndPoints()
        {
            // Arrange: minimal calendar + competitors
            var calendar = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Event 1 - ten", IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, EventName = "Event 2 - 25mile", IsEvening10 = false },
                new CalendarEvent { EventNumber = 3, EventName = "Event 3 - 9.5mile hardride", IsEvening10 = false }
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

            // Parse HTML
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Target the results table
            var table = document.QuerySelector("table.results");
            table.Should().NotBeNull();

            var headers = table!.QuerySelectorAll("thead th");
            headers.Should().NotBeEmpty();

            // Map header text -> column index
            var headerIndex = headers
                .Select((h, i) => (Text: h.TextContent.Trim(), Index: i))
                .ToDictionary(x => x.Text, x => x.Index);

            // Find the row for "Alice Smith"
            var row = table.QuerySelectorAll("tbody tr")
                           .FirstOrDefault(r => r.Children.Any(c => c.TextContent.Trim() == "Alice Smith"));
            row.Should().NotBeNull("Alice Smith row should be present");

            // Competitor name cell (anchored)
            var nameCell = row!.Children[headerIndex["Name"]];
            nameCell.TextContent.Trim().Should().Be("Alice Smith");
            nameCell.ClassName.Should().BeNullOrEmpty("Name column should not carry event classes");

            // Event 1 (10-mile): should be ten-mile-event and show 60
            var event1Cell = row.Children[headerIndex["Event 1"]];
            event1Cell.TextContent.Trim().Should().Be("60");
            event1Cell.ClassList.Should().Contain("ten-mile-event");

            // Event 2 (non-10): should be non-ten-mile-event and show "-" (DNS)
            var event2Cell = row.Children[headerIndex["Event 2"]];
            event2Cell.TextContent.Trim().Should().Be("-");
            event2Cell.ClassList.Should().Contain("non-ten-mile-event");

            // Event 3 (non-10): should be non-ten-mile-event and show "-" (no ride)
            var event3Cell = row.Children[headerIndex["Event 3"]];
            event3Cell.TextContent.Trim().Should().Be("-");
            event3Cell.ClassList.Should().Contain("non-ten-mile-event");

            // Optional: verify the "10-mile TTs Best 8" summary is classless (fixed column)
            var ttBest8Cell = row.Children[headerIndex["10-mile TTs Best 8"]];
            ttBest8Cell.ClassName.Should().BeNullOrEmpty("Summary columns should not carry event classes");

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

            // 1. Check header class rules
            var fixedHeaders = new[] { "Name", "Current rank", "Events completed", "10-mile TTs Best 8", "Scoring 11" };
            foreach (var fixedHeader in fixedHeaders)
            {
                var th = headers.First(h => h.TextContent.Trim() == fixedHeader);
                th.ClassName.Should().BeNullOrEmpty($"'{fixedHeader}' should not have a CSS class");
            }

            foreach (var ev in calendar)
            {
                var th = headers.First(h => h.TextContent.Trim() == ev.EventName);

                if (ev.IsEvening10)
                {
                    th.ClassList.Should().Contain("ten-mile-event", $"'{ev.EventName}' should be marked as a ten-mile-event");
                }
                else
                {
                    th.ClassList.Should().Contain("non-ten-mile-event", $"'{ev.EventName}' should be marked as a non-ten-mile-event");
                }
            }

            // 2. For each column, check that all body cells share the same class as the header
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
