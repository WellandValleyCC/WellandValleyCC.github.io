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
        public void Render_ShouldIncludeCompetitionTitleAndPoints()
        {
            // Arrange
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
            var renderer = new CompetitionRenderer(resultsSet);

            // Act
            var html = renderer.Render();

            // Assert: title, competitor, and points appear
            html.Should().Contain("<title>")
                .And.Contain("Alice Smith")
                .And.Contain("60"); // points from position 1
        }

        [Fact]
        public async Task Render_ShouldIncludeLegendWithCorrectEntries()
        {
            // Arrange
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
            var renderer = new CompetitionRenderer(resultsSet);

            // Act
            var html = renderer.Render();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert: legend structure and text
            var legend = document.QuerySelector("div.legend");
            legend.Should().NotBeNull();

            var spans = legend!.QuerySelectorAll("span");
            spans.Should().HaveCount(2);

            spans[0].ClassList.Should().Contain("ten-mile-event");
            spans[0].TextContent.Should().Be("10‑mile events");

            spans[1].ClassList.Should().Contain("non-ten-mile-event");
            spans[1].TextContent.Should().Be("Other events");
        }

        [Fact]
        public async Task Render_ShouldIncludeCompetitorRow_WithCorrectClasses()
        {
            // Arrange
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
            var renderer = new CompetitionRenderer(resultsSet);

            // Act
            var html = renderer.Render();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            var table = document.QuerySelector("table.results");
            table.Should().NotBeNull();

            var headerCells = table!.QuerySelectorAll("thead th");

            // Build index from data-col-index (leaf headers only)
            var headerIndex = headerCells
                .Where(h => h.HasAttribute("data-col-index"))
                .Select(h => (Key: h.GetAttribute("data-col-index")!, Index: int.Parse(h.GetAttribute("data-col-index")!)))
                .ToDictionary(x => x.Key, x => x.Index);

            // Convenience lookups
            int idx(string key) => headerIndex[key];

            // Name/Best8/Scoring11 aren’t emitted in row 3 as leaf headers,
            // but their indices are deterministic:
            headerIndex["0"] = 0; // Name
            headerIndex["5"] = 6; // Scoring 11
            headerIndex["6"] = 5; // 10-mile TTs Best 8

            var row = table.QuerySelectorAll("tbody tr")
                           .FirstOrDefault(r => r.Children.Any(c => c.TextContent.Trim() == "Alice Smith"));
            row.Should().NotBeNull();

            // Name column
            var nameCell = row!.Children[idx("0")];
            nameCell.TextContent.Trim().Should().Be("Alice Smith");
            nameCell.ClassName.Should().Be("competitor-name");

            // Event cells
            // Use idx("7").. for events, etc.
            var event1Cell = row!.Children[idx("7")];
            event1Cell.TextContent.Trim().Should().Be("60");
            event1Cell.ClassList.Should().Contain("ten-mile-event");

            var event2Cell = row!.Children[idx("8")];
            event2Cell.TextContent.Trim().Should().Be("-");
            event2Cell.ClassList.Should().Contain("non-ten-mile-event");

            var event3Cell = row!.Children[idx("9")];
            event3Cell.TextContent.Trim().Should().Be("-");
            event3Cell.ClassList.Should().Contain("non-ten-mile-event");

            // Summary column
            var ttBest8Cell = row.Children[idx("5")];
            ttBest8Cell.ClassName.Should().Contain("best-8");

            // Footer timestamp
            var footer = document.QuerySelector("footer p.generated");
            footer.Should().NotBeNull();
            footer!.TextContent.Should().MatchRegex(@"Generated .*UTC");
        }

        [Fact]
        public async Task Render_ShouldUseSemanticClassesForMultiRowHeaders()
        {
            // Arrange
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
            var renderer = new CompetitionRenderer(resultsSet);

            // Act
            var html = renderer.Render();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert
            var headerRows = document.QuerySelectorAll("table.results thead tr");
            headerRows.Should().HaveCount(3, "There should be three header rows: number, date, title");

            // Row 1: event numbers
            foreach (var th in headerRows[0].Children.Skip(5)) // skip fixed columns
                th.ClassList.Should().Contain("event-number");

            // Row 2: event titles
            foreach (var th in headerRows[1].Children.Skip(0))
                th.ClassList.Should().Contain("event-title");

            // Row 3: event dates
            foreach (var th in headerRows[2].Children.Skip(5))
                th.ClassList.Should().Contain("event-date");

            // Row 1: fixed columns
            var fixedHeaders = new[] { "Name", "Current rank", "Events completed", "10-mile TTs Best 8", "Scoring 11" };
            foreach (var fixedHeader in fixedHeaders)
            {
                var th = headerRows[0].Children.First(h => h.TextContent.Trim() == fixedHeader);
                th.ClassList.Should().Contain("fixed-column-title");
            }
        }

        [Fact]
        public async Task Render_ShouldApplyLegendClassesToBodyCells()
        {
            // Arrange
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
1,1,Valid,1,,,Alice Smith";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);
            DataLoader.AttachReferencesToRides(rides, competitors, calendar);

            var resultsSet = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendar);
            var renderer = new CompetitionRenderer(resultsSet);

            // Act
            var html = renderer.Render();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            // Assert: legend entries exist
            var legendSpans = document.QuerySelectorAll("div.legend span");
            legendSpans.Should().HaveCount(2);

            var legendClasses = legendSpans.Select(s => s.ClassName).ToList();
            legendClasses.Should().Contain(new[] { "ten-mile-event", "non-ten-mile-event" });

            // Assert: body event cells use only classes present in legend
            var rows = document.QuerySelectorAll("table.results tbody tr");
            rows.Should().NotBeEmpty();

            foreach (var row in rows)
            {
                // skip fixed columns
                for (int col = 7; col < row.Children.Length; col++)
                {
                    var cell = row.Children[col];
                    cell.ClassList.Should().IntersectWith(legendClasses,
                        $"Body cell '{cell.TextContent.Trim()}' should use a class represented in the legend");
                }
            }
        }
    }
}
