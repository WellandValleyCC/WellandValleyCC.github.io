using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Enums;
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
            juvenilesCompetitionResults.CompetitionType.Should().Be(CompetitionType.Juveniles);
            juvenilesCompetitionResults.DisplayName.Should().Be("Club Championship - Juveniles");
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
            var competitionResults = JuniorsCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            competitionResults.CompetitionType.Should().Be(CompetitionType.Juniors);
            competitionResults.DisplayName.Should().Be("Club Championship - Juniors");
            competitionResults.EligibilityStatement.Should().Contain("junior");
            competitionResults.EligibilityStatement.Should().NotContain("juvenile");
            competitionResults.EligibilityStatement.Should().NotContain("senior");
            competitionResults.EligibilityStatement.Should().NotContain("veteran");
            competitionResults.FileName.Should().Be("2025-juniors");
            competitionResults.GenericName.Should().Be("Juniors");
            competitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring
            competitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Noah Williams");
            competitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            competitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            competitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Olivia Smith");
            competitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            competitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            competitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Ava Taylor");
            competitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            competitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            competitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();
        }

        [Fact]
        public void VeteransCompetitionCreateTable_SortsByScoreBest11Events()
        {
            // Arrange: Veteran competitors (all with VetsBucket = 5)
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
7001,Anderson,George,FirstClaim,false,Veteran,5
7002,Clark,Sophia,FirstClaim,true,Veteran,5
8001,Moore,James,SecondClaim,false,Veteran,5";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with multiple events for each Veteran
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,7001,Valid,1,,905,George Anderson
1,7002,Valid,2,,912,Sophia Clark
1,8001,Valid,3,,918,James Moore
2,7001,Valid,2,,910,George Anderson
2,7002,Valid,1,,902,Sophia Clark
2,8001,Valid,4,,930,James Moore
3,7001,Valid,3,,920,George Anderson
3,7002,Valid,2,,915,Sophia Clark
3,8001,Valid,1,,900,James Moore";

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
            var competitionResults = VeteransCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            competitionResults.CompetitionType.Should().Be(CompetitionType.Veterans);
            competitionResults.DisplayName.Should().Be("Club Championship - Veterans");
            competitionResults.EligibilityStatement.Should().Contain("veteran");
            competitionResults.EligibilityStatement.Should().NotContain("juvenile");
            competitionResults.EligibilityStatement.Should().NotContain("junior");
            competitionResults.EligibilityStatement.Should().NotContain("senior");
            competitionResults.FileName.Should().Be("2025-veterans");
            competitionResults.GenericName.Should().Be("Veterans");
            competitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring (same totals as Juveniles/Juniors pattern)
            competitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Sophia Clark");
            competitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            competitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            competitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[1].Competitor.FullName.Should().Be("George Anderson");
            competitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            competitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            competitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[2].Competitor.FullName.Should().Be("James Moore");
            competitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            competitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            competitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();
        }

        [Fact]
        public void WomensCompetitionCreateTable_SortsByScoreBest11Events()
        {
            // Arrange: Women competitors (mix of age groups, some veterans with VetsBucket values)
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
9001,Hall,Emma,FirstClaim,true,Juvenile,
9002,Green,Olivia,FirstClaim,true,Junior,
9003,White,Sophia,SecondClaim,true,Senior,
9004,King,Ava,FirstClaim,true,Veteran,3
9005,Scott,Mia,SecondClaim,true,Veteran,7
9006,Young,Isabella,FirstClaim,true,Veteran,12";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with multiple events for each competitor
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,9001,Valid,1,,905,Emma Hall
1,9002,Valid,2,,912,Olivia Green
1,9003,Valid,3,,918,Sophia White
1,9004,Valid,4,,930,Ava King
1,9005,Valid,5,,940,Mia Scott
1,9006,Valid,6,,950,Isabella Young
2,9001,Valid,2,,910,Emma Hall
2,9002,Valid,1,,902,Olivia Green
2,9003,Valid,4,,930,Sophia White
2,9004,Valid,3,,920,Ava King
2,9005,Valid,5,,940,Mia Scott
2,9006,Valid,6,,950,Isabella Young
3,9001,Valid,3,,920,Emma Hall
3,9002,Valid,2,,915,Olivia Green
3,9003,Valid,1,,900,Sophia White
3,9004,Valid,4,,930,Ava King
3,9005,Valid,5,,940,Mia Scott
3,9006,Valid,6,,950,Isabella Young";

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
            var competitionResults = WomenCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            competitionResults.CompetitionType.Should().Be(CompetitionType.Women);
            competitionResults.DisplayName.Should().Be("Club Championship - Women");
            competitionResults.EligibilityStatement.Should().Match(s => s.Contains("women") || s.Contains("female"));
            competitionResults.FileName.Should().Be("2025-women");
            competitionResults.GenericName.Should().Be("Women");
            competitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring (pattern same as other tests, totals predictable)
            competitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Olivia Green");
            competitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            competitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            competitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();
            
            competitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Emma Hall");
            competitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            competitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            competitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();
            
            competitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Sophia White");
            competitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            competitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            competitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();

            // The remaining veterans (Ava, Mia, Isabella) will follow with lower totals
            competitionResults.ScoredRides[3].Competitor.FullName.Should().Be("Ava King");
            competitionResults.ScoredRides[4].Competitor.FullName.Should().Be("Mia Scott");
            competitionResults.ScoredRides[5].Competitor.FullName.Should().Be("Isabella Young");
        }


        [Fact]
        public void RoadBikeWomenCompetitionCreateTable_SortsByScoreBest11Events()
        {
            // Arrange: Road Bike Women competitors
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
1001,Pinnock,Milly,FirstClaim,true,Junior,
1002,Isaac,Ruby,FirstClaim,true,Juvenile,
1003,Moore,Jane,SecondClaim,true,Senior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with both EventRank and EventRoadBikeRank
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,1001,Valid,1,1,905,Milly Pinnock
1,1002,Valid,2,2,912,Ruby Isaac
1,1003,Valid,3,3,918,Jane Moore
2,1001,Valid,2,2,910,Milly Pinnock
2,1002,Valid,1,1,902,Ruby Isaac
2,1003,Valid,4,4,930,Jane Moore
3,1001,Valid,3,3,920,Milly Pinnock
3,1002,Valid,2,2,915,Ruby Isaac
3,1003,Valid,1,1,900,Jane Moore";

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
            var competitionResults = RoadBikeWomenCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            competitionResults.CompetitionType.Should().Be(CompetitionType.RoadBikeWomen);
            competitionResults.DisplayName.Should().Be("Club Championship - Road Bike Women");
            competitionResults.EligibilityStatement.Should().Contain("road bike");
            competitionResults.EligibilityStatement.Should().Match(s => s.Contains("women") || s.Contains("female"));
            competitionResults.FileName.Should().Be("2025-road-bike-women");
            competitionResults.GenericName.Should().Be("Road Bike Women");
            competitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring (same totals pattern as other categories)
            competitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Ruby Isaac");
            competitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            competitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);

            competitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Milly Pinnock");
            competitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            competitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);

            competitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Jane Moore");
            competitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            competitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
        }

        [Fact]
        public void SeniorsCompetitionCreateTable_SortsByScoreBest11Events_WithMixedAgeGroups()
        {
            // Arrange: competitors from mixed age groups (all eligible for Seniors)
            var competitorsCsv = @"ClubNumber,Surname,GivenName,ClaimStatus,IsFemale,AgeGroup,VetsBucket
7001,Brown,Emily,FirstClaim,true,Juvenile,
7002,Johnson,Liam,FirstClaim,false,Junior,
8001,Evans,Daniel,SecondClaim,false,Senior,";

            var competitors = CsvTestLoader.LoadCompetitorsFromCsv(competitorsCsv);

            // Assemble: rides CSV with multiple events for each competitor
            var ridesCsv = @"EventNumber,ClubNumber,Status,EventRank,EventRoadBikeRank,TotalSeconds,Name
1,7001,Valid,1,,905,Emily Brown
1,7002,Valid,2,,912,Liam Johnson
1,8001,Valid,3,,918,Daniel Evans
2,7001,Valid,2,,910,Emily Brown
2,7002,Valid,1,,902,Liam Johnson
2,8001,Valid,4,,930,Daniel Evans
3,7001,Valid,3,,920,Emily Brown
3,7002,Valid,2,,915,Liam Johnson
3,8001,Valid,1,,900,Daniel Evans";

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
            var competitionResults = SeniorsCompetitionResultsSet.CreateFrom(rides, calendarEvents);

            // Assert: metadata
            competitionResults.CompetitionType.Should().Be(CompetitionType.Seniors);
            competitionResults.DisplayName.Should().Be("Club Championship - Seniors");
            competitionResults.EligibilityStatement.Should().Contain("any age");
            competitionResults.EligibilityStatement.Should().NotContain("juvenile");
            competitionResults.EligibilityStatement.Should().NotContain("junior");
            competitionResults.EligibilityStatement.Should().NotContain("veteran");
            competitionResults.FileName.Should().Be("2025-seniors");
            competitionResults.GenericName.Should().Be("Seniors");
            competitionResults.SubFolderName.Should().Be("competitions");

            // Assert: scoring
            competitionResults.ScoredRides[0].Competitor.FullName.Should().Be("Liam Johnson");
            competitionResults.ScoredRides[0].AllEvents.Points.Should().Be(170);
            competitionResults.ScoredRides[0].AllEvents.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].FullCompetition.Points.Should().Be(170);
            competitionResults.ScoredRides[0].FullCompetition.Rank.Should().Be(1);
            competitionResults.ScoredRides[0].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[0].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[1].Competitor.FullName.Should().Be("Emily Brown");
            competitionResults.ScoredRides[1].AllEvents.Points.Should().Be(166);
            competitionResults.ScoredRides[1].AllEvents.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].FullCompetition.Points.Should().Be(166);
            competitionResults.ScoredRides[1].FullCompetition.Rank.Should().Be(2);
            competitionResults.ScoredRides[1].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[1].TenMileCompetition.Rank.Should().BeNull();

            competitionResults.ScoredRides[2].Competitor.FullName.Should().Be("Daniel Evans");
            competitionResults.ScoredRides[2].AllEvents.Points.Should().Be(159);
            competitionResults.ScoredRides[2].AllEvents.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].FullCompetition.Points.Should().Be(159);
            competitionResults.ScoredRides[2].FullCompetition.Rank.Should().Be(3);
            competitionResults.ScoredRides[2].TenMileCompetition.Points.Should().BeNull();
            competitionResults.ScoredRides[2].TenMileCompetition.Rank.Should().BeNull();
        }
    }
}
