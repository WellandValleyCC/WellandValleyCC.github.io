using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubSiteGenerator.Tests
{
    public class ResultsGeneratorTests
    {
        [Fact]
        public void OrderedIneligibleRides_SortsMembersThenSecondClaimThenGuests()
        {
            // Arrange
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1243,Smith,Alice,FirstClaim,true,Senior,
1244,Smith,John,SecondClaim,false,Senior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with competitor + guest
            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1243,DNF,,,0,Alice Smith
1,1244,DNF,,,0,John Smith
1,,DNF,,,0,Guest Rider";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var calendarEvent = new CalendarEvent { EventNumber = 1, EventName = "Test TT", EventDate = DateTime.Today, Miles = 10 };

            var eventResults = new EventResultsSet(1, new[] { calendarEvent }, rides);

            // Act
            //var ordered = ResultsSet.OrderedIneligibleRides(rides, RideEligibility.DNF).ToList();
            var table = eventResults.CreateTable();

            // Assert
            table.Headers.Should().ContainInOrder("Name", "Position", "Road Bike", "Actual Time", "Avg. mph");

            table.Rows[0].Ride.Name.Should().Be("Alice Smith"); // FirstClaim member first
            table.Rows[1].Ride.Name.Should().Be("John Smith");  // then SecondClaim
            table.Rows[2].Ride.Name.Should().Be("Guest Rider"); // then guest

            table.Rows[0].Cells[3].Should().Be("DNF"); // Alice Smith
            table.Rows[1].Cells[3].Should().Be("DNF"); // John Smith
            table.Rows[2].Cells[3].Should().Be("DNF"); // Guest Rider

        }
    }
}
