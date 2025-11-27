using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class EventRendererTests
    {
        [Fact]
        public void Render_IncludesTitleAndHeader()
        {
            // Arrange: calendar event
            var ev = new CalendarEvent
            {
                EventNumber = 11,
                EventName = "Walcote Interclub 25mile Hardride TT",
                EventDate = new DateTime(2025, 6, 15),
                Miles = 25
            };

            // Arrange: competitor + ride
            var competitor = new Competitor
            {
                ClubNumber = 123,
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,
                AgeGroup = AgeGroup.Senior,
                Surname = "Smith",
                GivenName = "John",
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                League = League.Undefined,
                VetsBucket = null
            };

            var ride = new Ride
            {
                EventNumber = 11,
                EventRank = 1,
                Status = RideStatus.Valid,
                Competitor = competitor,
                ClubNumber = competitor.ClubNumber,
                EventEligibleRidersRank = 1,
                CalendarEvent = ev
            };

            var resultsSet = EventResultsSet.CreateFrom(new[] { ev }, new[] { ride }, 11);
            var renderer = new EventRenderer(resultsSet, numberOfEvents: 20);

            // Act
            var html = renderer.Render();

            // Assert
            html.Should().Contain("<title>Event 11: Walcote Interclub 25mile Hardride TT</title>");
            html.Should().Contain("<span class=\"event-number\">Event 11:</span>");
            html.Should().Contain("Sunday, 15 June 2025");
            html.Should().Contain("Distance: 25 miles");
            html.Should().Contain("href=\"event-10.html\"");
            html.Should().Contain("href=\"../preview.html\"");
            html.Should().Contain("href=\"event-12.html\"");
            html.Should().Contain("competition-eligible");
            html.Should().Contain("guest-second-claim");
            html.Should().Contain("guest-non-club-member");
            html.Should().Contain("class=\"position-1\"");
            html.Should().Contain("<tr class=\"competition-eligible\">");
        }

        [Fact]
        public void Render_EventOnePrevWrapsToTotalEvents()
        {
            var ev = new CalendarEvent
            {
                EventNumber = 1,
                EventName = "Dummy",
                EventDate = new DateTime(2025, 1, 1),
                Miles = 10
            };

            var resultsSet = EventResultsSet.CreateFrom(new[] { ev }, Array.Empty<Ride>(), 1);
            var renderer = new EventRenderer(resultsSet, numberOfEvents: 20);

            var html = renderer.Render();

            html.Should().Contain("href=\"event-20.html\""); // prev wraps
            html.Should().Contain("href=\"event-02.html\""); // next
        }

        [Fact]
        public void Render_LastEventNextWrapsToOne()
        {
            var ev = new CalendarEvent
            {
                EventNumber = 20,
                EventName = "Dummy",
                EventDate = new DateTime(2025, 1, 1),
                Miles = 10
            };

            var resultsSet = EventResultsSet.CreateFrom(new[] { ev }, Array.Empty<Ride>(), 20);
            var renderer = new EventRenderer(resultsSet, numberOfEvents: 20);

            var html = renderer.Render();

            html.Should().Contain("href=\"event-19.html\""); // prev
            html.Should().Contain("href=\"event-01.html\""); // next wraps
        }
    }
}
