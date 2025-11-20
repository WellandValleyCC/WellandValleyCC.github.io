using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class ResultsGeneratorTests
    {
        [Fact]
        public void EventCreateTable_SortsByRankWithDxxAppendedAndSortedMembersThenSecondClaimThenGuests()
        {
            // Arrange
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1243,Smith,Alice,FirstClaim,true,Senior,
1244,Smith,John,SecondClaim,false,Senior,
3001,Brown,Emily,FirstClaim,true,Juvenile,
3002,Johnson,Liam,FirstClaim,false,Juvenile,
3003,Taylor,Sophia,FirstClaim,true,Junior,
3004,Wilson,Ethan,FirstClaim,false,Junior,
3005,Clark,Olivia,FirstClaim,true,Senior,
3006,Walker,Noah,FirstClaim,false,Senior,
3007,Harris,Ava,FirstClaim,true,Veteran,12
3008,Roberts,James,FirstClaim,false,Veteran,15
4001,Evans,Daniel,SecondClaim,false,Juvenile,
4002,Green,Chloe,SecondClaim,true,Junior,
4003,Hall,Matthew,SecondClaim,false,Senior,
4004,Young,Sophia,SecondClaim,true,Veteran,14
5001,Mitchell,Grace,FirstClaim,true,Senior,
5002,Turner,Lucas,FirstClaim,false,Senior,
5003,Edwards,Mia,FirstClaim,true,Senior,
5004,Bennett,Oliver,SecondClaim,false,Senior,
5005,Morgan,Isabella,SecondClaim,true,Senior,
5006,Carter,Henry,SecondClaim,false,Senior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with competitor + guest
            var ridesCsv = @"EventNumber,ClubNumber,Eligibility,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,,DQ,,,,Guest RiderDq
1,,DQ,,,,zGuest AnotherRiderDq
1,5002,DQ,,,,Lucas Turner
1,5006,DQ,,,,Henry Carter
1,,DNS,,,,Guest RiderDns
1,,DNS,,,,zGuest AnotherRiderDns
1,5001,DNS,,,,Grace Mitchell
1,5004,DNS,,,,Oliver Bennett
1,,DNF,,,,Guest Rider
1,,DNF,,,,zGuest AnotherRiderDnf
1,1244,DNF,,,,John Smith
1,1243,DNF,,,,Alice Smith
1,4004,Valid,12,8,995,Sophia Young
1,4003,Valid,11,7,982,Matthew Hall
1,4002,Valid,10,6,971,Chloe Green
1,4001,Valid,9,5,963,Daniel Evans
1,3008,Valid,8,,956,James Roberts
1,3007,Valid,7,4,949,Ava Harris
1,3006,Valid,6,,941,Noah Walker
1,3005,Valid,5,3,934,Olivia Clark
1,3004,Valid,4,,927,Ethan Wilson
1,3003,Valid,3,2,918,Sophia Taylor
1,3002,Valid,2,,912,Liam Johnson
1,3001,Valid,1,1,905,Emily Brown";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var calendarEvent = new CalendarEvent { EventNumber = 1, EventName = "Test TT", EventDate = DateTime.Today, Miles = 10 };

            var eventResults = EventResultsSet.CreateFrom(calendarEvent, rides);

            // Act
            //var ordered = ResultsSet.OrderedIneligibleRides(rides, RideEligibility.DNF).ToList();
            var table = eventResults.CreateTable();

            // Assert
            table.Headers.Should().ContainInOrder("Name", "Position", "Road Bike", "Actual Time", "Avg. mph");

            var i = 0;
            table.Rows[i++].Ride.Name.Should().Be("Emily Brown");
            table.Rows[i++].Ride.Name.Should().Be("Liam Johnson");
            table.Rows[i++].Ride.Name.Should().Be("Sophia Taylor");
            table.Rows[i++].Ride.Name.Should().Be("Ethan Wilson");
            table.Rows[i++].Ride.Name.Should().Be("Olivia Clark");
            table.Rows[i++].Ride.Name.Should().Be("Noah Walker");
            table.Rows[i++].Ride.Name.Should().Be("Ava Harris");
            table.Rows[i++].Ride.Name.Should().Be("James Roberts");
            table.Rows[i++].Ride.Name.Should().Be("Daniel Evans");
            table.Rows[i++].Ride.Name.Should().Be("Chloe Green");
            table.Rows[i++].Ride.Name.Should().Be("Matthew Hall");
            table.Rows[i++].Ride.Name.Should().Be("Sophia Young");

            table.Rows[i].Cells[3].Should().Be("DNF");
            table.Rows[i++].Ride.Name.Should().Be("Alice Smith"); // DNF FirstClaim member first
            table.Rows[i].Cells[3].Should().Be("DNF");
            table.Rows[i++].Ride.Name.Should().Be("John Smith");  // DNF then SecondClaim
            table.Rows[i].Cells[3].Should().Be("DNF");
            table.Rows[i++].Ride.Name.Should().Be("zGuest AnotherRiderDnf"); // DNF then guest - sorted alphabetically surname firstname
            table.Rows[i].Cells[3].Should().Be("DNF");
            table.Rows[i++].Ride.Name.Should().Be("Guest Rider");

            table.Rows[i].Cells[3].Should().Be("DNS");
            table.Rows[i++].Ride.Name.Should().Be("Grace Mitchell"); // DNS FirstClaim member first
            table.Rows[i].Cells[3].Should().Be("DNS");
            table.Rows[i++].Ride.Name.Should().Be("Oliver Bennett");  // DNS then SecondClaim
            table.Rows[i].Cells[3].Should().Be("DNS");
            table.Rows[i++].Ride.Name.Should().Be("zGuest AnotherRiderDns"); // DNS then guest - sorted alphabetically surname firstname
            table.Rows[i].Cells[3].Should().Be("DNS");
            table.Rows[i++].Ride.Name.Should().Be("Guest RiderDns");

            table.Rows[i].Cells[3].Should().Be("DQ");
            table.Rows[i++].Ride.Name.Should().Be("Lucas Turner"); // DQ FirstClaim member first
            table.Rows[i].Cells[3].Should().Be("DQ");
            table.Rows[i++].Ride.Name.Should().Be("Henry Carter");  // DQ then SecondClaim
            table.Rows[i].Cells[3].Should().Be("DQ");
            table.Rows[i++].Ride.Name.Should().Be("zGuest AnotherRiderDq"); // DNS then guest - sorted alphabetically surname firstname
            table.Rows[i].Cells[3].Should().Be("DQ");
            table.Rows[i++].Ride.Name.Should().Be("Guest RiderDq"); // DQ then guest
        }

        [Fact]
        public void JuvenilesCompetitionCreateTable_SortsByScoreBest11Events()
        {
            // Arrange: Juvenile competitors
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
3001,Brown,Emily,FirstClaim,true,Juvenile,
3002,Johnson,Liam,FirstClaim,false,Juvenile,
4001,Evans,Daniel,SecondClaim,false,Juvenile,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with multiple events for each Juvenile
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,3001,Valid,1,,905,Emily Brown
1,3002,Valid,2,,912,Liam Johnson
1,4001,Valid,3,,918,Daniel Evans
2,3001,Valid,2,,910,Emily Brown
2,3002,Valid,1,,902,Liam Johnson
2,4001,Valid,4,,930,Daniel Evans
3,3001,Valid,3,,920,Emily Brown
3,3002,Valid,2,,915,Liam Johnson
3,4001,Valid,1,,900,Daniel Evans";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var calendarEvents = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Event 1", EventDate = DateTime.Today.AddDays(-2), Miles = 10 },
                new CalendarEvent { EventNumber = 2, EventName = "Event 2", EventDate = DateTime.Today.AddDays(-1), Miles = 10 },
                new CalendarEvent { EventNumber = 3, EventName = "Event 3", EventDate = DateTime.Today, Miles = 10 }
            };

            var juvenilesCompetitionResults = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Act
            var table = juvenilesCompetitionResults.CreateTable();

            // Assert: headers include event columns + totals
            table.Headers.Should().ContainInOrder("Name", "Event 1", "Event 2", "Event 3", "Total (Best 11)");

            var i = 0;
            table.Rows[i].Ride.Name.Should().Be("Emily Brown");
            table.Rows[i].Cells[1].Should().Be("60"); // Event 1 rank 1 → 60 points
            table.Rows[i].Cells[2].Should().Be("55"); // Event 2 rank 2 → 55 points
            table.Rows[i].Cells[3].Should().Be("51"); // Event 3 rank 3 → 51 points
            table.Rows[i++].Cells[4].Should().Be("166"); // total

            table.Rows[i].Ride.Name.Should().Be("Liam Johnson");
            table.Rows[i].Cells[1].Should().Be("55"); // Event 1 rank 2
            table.Rows[i].Cells[2].Should().Be("60"); // Event 2 rank 1
            table.Rows[i].Cells[3].Should().Be("55"); // Event 3 rank 2
            table.Rows[i++].Cells[4].Should().Be("170");

            table.Rows[i].Ride.Name.Should().Be("Daniel Evans");
            table.Rows[i].Cells[1].Should().Be("51"); // Event 1 rank 3
            table.Rows[i].Cells[2].Should().Be("48"); // Event 2 rank 4
            table.Rows[i].Cells[3].Should().Be("60"); // Event 3 rank 1
            table.Rows[i++].Cells[4].Should().Be("159");
        }
    }
}
