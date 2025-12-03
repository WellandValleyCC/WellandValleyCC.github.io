using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ClubSiteGenerator.Tests
{
    public class ResultsGeneratorTests
    {
        [Fact]
        public void EventCreateFrom_SortsByRankWithDxxAppendedAndSortedMembersThenSecondClaimThenGuests()
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

            rides
                .Where(r => r.EventNumber == calendarEvent.EventNumber)
                .ToList()
                .ForEach(r => r.CalendarEvent = calendarEvent);

            rides
                .Where(r => r.Status == RideStatus.Valid && r.TotalSeconds > 0)
                .ToList()
                .ForEach(r => r.AvgSpeed = calendarEvent.Miles / (r.TotalSeconds / 3600.0));

            // Act
            var eventResults = EventResultsSet.CreateFrom(new[] { calendarEvent }, rides, 1);

            // Assert
            eventResults.Should().NotBeNull();
            eventResults.Rides.Should().NotBeNull();
            eventResults.Rides.Count().Should().Be(rides.Count);

            var ridesArray = eventResults.Rides.ToArray();

            var i = 0;
            ridesArray[i].EventRank.Should().Be(1);  ridesArray[i++].Name.Should().Be("Emily Brown");
            ridesArray[i].EventRank.Should().Be(2);  ridesArray[i++].Name.Should().Be("Liam Johnson");
            ridesArray[i].EventRank.Should().Be(3);  ridesArray[i++].Name.Should().Be("Sophia Taylor");
            ridesArray[i].EventRank.Should().Be(4);  ridesArray[i++].Name.Should().Be("Ethan Wilson");
            ridesArray[i].EventRank.Should().Be(5);  ridesArray[i++].Name.Should().Be("Olivia Clark");
            ridesArray[i].EventRank.Should().Be(6);  ridesArray[i++].Name.Should().Be("Noah Walker");
            ridesArray[i].EventRank.Should().Be(7);  ridesArray[i++].Name.Should().Be("Ava Harris");
            ridesArray[i].EventRank.Should().Be(8);  ridesArray[i++].Name.Should().Be("James Roberts");
            ridesArray[i].EventRank.Should().Be(9);  ridesArray[i++].Name.Should().Be("Daniel Evans");
            ridesArray[i].EventRank.Should().Be(10); ridesArray[i++].Name.Should().Be("Chloe Green");
            ridesArray[i].EventRank.Should().Be(11); ridesArray[i++].Name.Should().Be("Matthew Hall");
            ridesArray[i].EventRank.Should().Be(12); ridesArray[i++].Name.Should().Be("Sophia Young");

            ridesArray[i].Status.Should().Be(RideStatus.DNF);
            ridesArray[i++].Name.Should().Be("Alice Smith"); // DNF FirstClaim member first
            ridesArray[i].Status.Should().Be(RideStatus.DNF);
            ridesArray[i++].Name.Should().Be("John Smith");  // DNF then SecondClaim
            ridesArray[i].Status.Should().Be(RideStatus.DNF);
            ridesArray[i++].Name.Should().Be("zGuest AnotherRiderDnf"); // DNF then guest - sorted alphabetically surname firstname
            ridesArray[i].Status.Should().Be(RideStatus.DNF);
            ridesArray[i++].Name.Should().Be("Guest Rider");

            ridesArray[i].Status.Should().Be(RideStatus.DNS);
            ridesArray[i++].Name.Should().Be("Grace Mitchell"); // DNS FirstClaim member first
            ridesArray[i].Status.Should().Be(RideStatus.DNS);
            ridesArray[i++].Name.Should().Be("Oliver Bennett");  // DNS then SecondClaim
            ridesArray[i].Status.Should().Be(RideStatus.DNS);
            ridesArray[i++].Name.Should().Be("zGuest AnotherRiderDns"); // DNS then guest - sorted alphabetically surname firstname
            ridesArray[i].Status.Should().Be(RideStatus.DNS);
            ridesArray[i++].Name.Should().Be("Guest RiderDns");

            ridesArray[i].Status.Should().Be(RideStatus.DQ);
            ridesArray[i++].Name.Should().Be("Lucas Turner"); // DQ FirstClaim member first
            ridesArray[i].Status.Should().Be(RideStatus.DQ);
            ridesArray[i++].Name.Should().Be("Henry Carter");  // DQ then SecondClaim
            ridesArray[i].Status.Should().Be(RideStatus.DQ);
            ridesArray[i++].Name.Should().Be("zGuest AnotherRiderDq"); // DNS then guest - sorted alphabetically surname firstname
            ridesArray[i].Status.Should().Be(RideStatus.DQ);
            ridesArray[i++].Name.Should().Be("Guest RiderDq"); // DQ then guest
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

            var eventsByNumber = calendarEvents.ToDictionary(e => e.EventNumber);

            // Hydrate rides with their matching CalendarEvent
            foreach (var ride in rides)
            {
                if (eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                {
                    ride.CalendarEvent = ev;
                }
            }

            // Act
            var juvenilesCompetitionResults = JuvenilesCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert
            juvenilesCompetitionResults.CompetitionType.Should().Be("Juveniles");
            juvenilesCompetitionResults.DisplayName.Should().Be("Juveniles Championship");
            juvenilesCompetitionResults.EligibilityStatement.Should().Contain("juvenile");
            juvenilesCompetitionResults.EligibilityStatement.Should().NotContain("junior");
            juvenilesCompetitionResults.EligibilityStatement.Should().NotContain("senior");
            juvenilesCompetitionResults.EligibilityStatement.Should().NotContain("veteran");
            juvenilesCompetitionResults.FileName.Should().Be("2025-juveniles");
            juvenilesCompetitionResults.GenericName.Should().Be("Juveniles");
            juvenilesCompetitionResults.SubFolderName.Should().Be("competitions");

            juvenilesCompetitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Liam Johnson");
            juvenilesCompetitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            juvenilesCompetitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            juvenilesCompetitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            juvenilesCompetitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            juvenilesCompetitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            juvenilesCompetitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();

            juvenilesCompetitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Emily Brown");
            juvenilesCompetitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            juvenilesCompetitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            juvenilesCompetitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            juvenilesCompetitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            juvenilesCompetitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            juvenilesCompetitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();

            juvenilesCompetitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Daniel Evans");
            juvenilesCompetitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            juvenilesCompetitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            juvenilesCompetitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            juvenilesCompetitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            juvenilesCompetitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            juvenilesCompetitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();
        }

        [Fact]
        public void JuniorsCompetitionCreateTable_SortsByScoreBest11Events()
        {
            // Arrange: Junior competitors
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
5001,Smith,Olivia,FirstClaim,true,Junior,
5002,Williams,Noah,FirstClaim,false,Junior,
6001,Taylor,Ava,SecondClaim,false,Junior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with multiple events for each Junior
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,5001,Valid,1,,905,Olivia Smith
1,5002,Valid,2,,912,Noah Williams
1,6001,Valid,3,,918,Ava Taylor
2,5001,Valid,2,,910,Olivia Smith
2,5002,Valid,1,,902,Noah Williams
2,6001,Valid,4,,930,Ava Taylor
3,5001,Valid,3,,920,Olivia Smith
3,5002,Valid,2,,915,Noah Williams
3,6001,Valid,1,,900,Ava Taylor";

            var rides = CsvTestLoader.LoadRidesFromCsv(ridesCsv, competitors);

            var calendarEvents = new[]
            {
                new CalendarEvent { EventNumber = 1, EventName = "Event 1", EventDate = DateTime.Today.AddDays(-2), Miles = 10 },
                new CalendarEvent { EventNumber = 2, EventName = "Event 2", EventDate = DateTime.Today.AddDays(-1), Miles = 10 },
                new CalendarEvent { EventNumber = 3, EventName = "Event 3", EventDate = DateTime.Today, Miles = 10 }
            };

            var eventsByNumber = calendarEvents.ToDictionary(e => e.EventNumber);

            // Hydrate rides with their matching CalendarEvent
            foreach (var ride in rides)
            {
                if (eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                {
                    ride.CalendarEvent = ev;
                }
            }

            // Act
            var juniorsCompetitionResults = JuniorsCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            juniorsCompetitionResults.CompetitionType.Should().Be("Juniors");
            juniorsCompetitionResults.DisplayName.Should().Be("Juniors Championship");
            juniorsCompetitionResults.EligibilityStatement.Should().Contain("junior");
            juniorsCompetitionResults.EligibilityStatement.Should().NotContain("juvenile");
            juniorsCompetitionResults.EligibilityStatement.Should().NotContain("senior");
            juniorsCompetitionResults.EligibilityStatement.Should().NotContain("veteran");
            juniorsCompetitionResults.FileName.Should().Be("2025-juniors");
            juniorsCompetitionResults.GenericName.Should().Be("Juniors");
            juniorsCompetitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring
            juniorsCompetitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Noah Williams");
            juniorsCompetitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            juniorsCompetitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            juniorsCompetitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            juniorsCompetitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            juniorsCompetitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            juniorsCompetitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();

            juniorsCompetitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Olivia Smith");
            juniorsCompetitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            juniorsCompetitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            juniorsCompetitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            juniorsCompetitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            juniorsCompetitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            juniorsCompetitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();

            juniorsCompetitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Ava Taylor");
            juniorsCompetitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            juniorsCompetitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            juniorsCompetitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            juniorsCompetitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            juniorsCompetitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            juniorsCompetitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();
        }

    }
}
