using System;
using Xunit;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Renderers;
using ClubCore.Models;
using ClubCore.Models.Enums;
using System.Collections.Generic;

namespace ClubSiteGenerator.Tests
{
    public class EventRendererTests
    {
        [Fact]
        public void Render_IncludesTitleAndHeader()
        {
            var table = new HtmlTable(
                new List<string> { "Name", "Position", "Road Bike", "Time" },
                new List<HtmlRow>
                {
                    new HtmlRow(
                        new List<string> { "John Smith", "1", "", "00:57:08" },
                        new Ride
                        {
                            EventEligibleRidersRank = 1,
                            ClubNumber = 123,
                            Competitor = new Competitor
                            {
                                ClubNumber = 123,
                                ClaimStatus = ClaimStatus.FirstClaim,
                                IsFemale = false,
                                AgeGroup = AgeGroup.Senior,
                                Surname = "Smith",
                                GivenName = "John"
                            }
                        })
                });

            var renderer = new EventRenderer(
                table,
                "Walcote Interclub 25mile Hardride TT",
                eventNumber: 11,
                totalEvents: 20,
                eventDate: new DateOnly(2025, 6, 15),
                eventMiles: 25);

            var html = renderer.Render();

            // Title element
            Assert.Contains("<title>Event 11: Walcote Interclub 25mile Hardride TT</title>", html);

            // Header with event number
            Assert.Contains("<span class=\"event-number\">Event 11:</span>", html);

            // Date and distance
            Assert.Contains("Sunday, 15 June 2025", html);
            Assert.Contains("Distance: 25 miles", html);

            // Navigation links
            Assert.Contains("href=\"event-10.html\"", html); // prev
            Assert.Contains("href=\"../preview.html\"", html); // index
            Assert.Contains("href=\"event-12.html\"", html); // next

            // Legend spans
            Assert.Contains("competition-eligible", html);
            Assert.Contains("guest-second-claim", html);
            Assert.Contains("guest-non-club-member", html);

            // Podium class applied
            Assert.Contains("class=\"position-1\"", html);

            // Row classification
            Assert.Contains("<tr class=\"competition-eligible\">", html);
        }

        [Fact]
        public void Render_EventOnePrevWrapsToTotalEvents()
        {
            var table = new HtmlTable(new List<string>(), new List<HtmlRow>());
            var renderer = new EventRenderer(table, "Dummy", 1, 20, new DateOnly(2025, 1, 1), 10);
            var html = renderer.Render();

            Assert.Contains("href=\"event-20.html\"", html); // prev wraps
            Assert.Contains("href=\"event-02.html\"", html); // next
        }

        [Fact]
        public void Render_LastEventNextWrapsToOne()
        {
            var table = new HtmlTable(new List<string>(), new List<HtmlRow>());
            var renderer = new EventRenderer(table, "Dummy", 20, 20, new DateOnly(2025, 1, 1), 10);
            var html = renderer.Render();

            Assert.Contains("href=\"event-19.html\"", html); // prev
            Assert.Contains("href=\"event-01.html\"", html); // next wraps
        }
    }
}
