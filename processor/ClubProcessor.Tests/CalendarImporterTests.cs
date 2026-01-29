using ClubCore.Context;
using ClubCore.Models;
using ClubProcessor.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Tests
{
    public class CalendarImporterTests
    {
        [Fact]
        public void ImportFromCsv_ShouldYieldSixEvents_WithCorrectProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new EventDbContext(options);
            var importer = new CalendarImporter(context);

            // Seed ONLY valid clubs (e.g., RFW, HCRC)
            context.RoundRobinClubs.AddRange(
                new RoundRobinClub { ShortName = "RFW", FullName = "Rockingham Forest Wheelers", WebsiteUrl = "URL" },
                new RoundRobinClub { ShortName = "HCRC", FullName = "Horton Cycling & Running Club", WebsiteUrl = "URL" }
            );
            context.SaveChanges();

            var testCsvPath = "test-data/calendar_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath,
                "Event Number,Date,Start time,Event Name,Round Robin Club,Miles,Location / Course,Hill Climb,Club Championship,Non-Standard 10,Evening 10,Hard Ride Series,Round Robin Event,Sheet Name,isCancelled\n" +
                "1,2025-04-01,18:30,Medbourne 9.5mile Hardride TT,,9.5,Medbourne,,Y,Y,,Y,,Event_01,\n" +
                "2,2025-04-06,09:00,Medbourne 9.5mile NDCA Hardride TT,,9.5,Medbourne,,Y,Y,,Y,,Event_02,\n" +
                "3,2025-04-15,18:30,Hallaton 5 5mile TT,,5.0,Hallaton 5,,Y,Y,,,,Event_03,\n" +
                "4,2025-04-20,08:00,Kibworth 25mile TT,,25.0,Kibworth,,Y,Y,,,,Event_04,\n" +
                "5,2025-04-29,18:30,Tour of Langtons 10mile TT,HCRC,10.0,Tour of Langtons,,Y,,Y,,Y,Event_05,\n" +
                "6,2025-05-06,18:30,Tur Langton/Hallaton 5+5mile TT,,10.0,Tur Langton/Hallaton,,Y,Y,,,,Event_06,\n" +
                "23,2025-09-07,10:30:00,Interclub Hillclimb RFW,RFW,0.88,Nevill Holt ,Y,Y,Y,,,,Event_23,\n");


            // Act
            importer.ImportFromCsv(testCsvPath);

            // Assert
            context.CalendarEvents.Should().HaveCount(7);

            var events = context.CalendarEvents.OrderBy(e => e.EventNumber).ToList();

            // Define expected values for each row
            var expected = new[]
            {
                new {
                    Number = 1, Date = new DateTime(2025, 4, 1), Time = TimeSpan.FromHours(18.5),
                    Name = "Medbourne 9.5mile Hardride TT", RoundRobinClub = "", Miles = 9.5, Location = "Medbourne",
                    HillClimb = false, ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = true, RoundRobinEvent = false, Cancelled = false
                },
                new {
                    Number = 2, Date = new DateTime(2025, 4, 6), Time = TimeSpan.FromHours(9),
                    Name = "Medbourne 9.5mile NDCA Hardride TT", RoundRobinClub = "", Miles = 9.5, Location = "Medbourne",
                    HillClimb = false, ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = true, RoundRobinEvent = false, Cancelled = false
                },
                new {
                    Number = 3, Date = new DateTime(2025, 4, 15), Time = TimeSpan.FromHours(18.5),
                    Name = "Hallaton 5 5mile TT", RoundRobinClub = "", Miles = 5.0, Location = "Hallaton 5",
                    HillClimb = false, ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = false, RoundRobinEvent = false, Cancelled = false
                },
                new {
                    Number = 4, Date = new DateTime(2025, 4, 20), Time = TimeSpan.FromHours(8),
                    Name = "Kibworth 25mile TT", RoundRobinClub = "", Miles = 25.0, Location = "Kibworth",
                    HillClimb = false,  ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = false, RoundRobinEvent = false, Cancelled = false
                },
                new {
                    Number = 5, Date = new DateTime(2025, 4, 29), Time = TimeSpan.FromHours(18.5),
                    Name = "Tour of Langtons 10mile TT", RoundRobinClub = "HCRC", Miles = 10.0, Location = "Tour of Langtons",
                    HillClimb = false, ClubChamp = true, NonStd10 = false, Evening10 = true, HardRide = false, RoundRobinEvent = true, Cancelled = false
                },
                new {
                    Number = 6, Date = new DateTime(2025, 5, 6), Time = TimeSpan.FromHours(18.5),
                    Name = "Tur Langton/Hallaton 5+5mile TT", RoundRobinClub = "", Miles = 10.0, Location = "Tur Langton/Hallaton",
                    HillClimb = false, ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = false, RoundRobinEvent = false, Cancelled = false
                },
                new {
                    Number = 23, Date = new DateTime(2025, 9, 7), Time = TimeSpan.FromHours(10.5),
                    Name = "Interclub Hillclimb RFW", RoundRobinClub = "RFW", Miles = 0.88, Location = "Nevill Holt",
                    HillClimb = true, ClubChamp = true, NonStd10 = true, Evening10 = false, HardRide = false, RoundRobinEvent = false, Cancelled = false
                }
            };

            // Loop through and assert each property
            for (int i = 0; i < expected.Length; i++)
            {
                var ev = events[i];
                var exp = expected[i];
                string ctx = $"because Event {exp.Number} ({exp.Name}) should match the CSV row";

                ev.EventNumber.Should().Be(exp.Number, ctx);
                ev.EventDate.Should().Be(exp.Date, ctx);
                ev.StartTime.Should().Be(exp.Time, ctx);
                ev.EventName.Should().Be(exp.Name, ctx);
                ev.RoundRobinClub.Should().Be(exp.RoundRobinClub, ctx);
                ev.Miles.Should().Be(exp.Miles, ctx);
                ev.Location.Should().Be(exp.Location, ctx);
                ev.IsHillClimb.Should().Be(exp.HillClimb, ctx);
                ev.IsClubChampionship.Should().Be(exp.ClubChamp, ctx);
                ev.IsNonStandard10.Should().Be(exp.NonStd10, ctx);
                ev.IsEvening10.Should().Be(exp.Evening10, ctx);
                ev.IsHardRideSeries.Should().Be(exp.HardRide, ctx);
                ev.IsRoundRobinEvent.Should().Be(exp.RoundRobinEvent, ctx);
                ev.IsCancelled.Should().Be(exp.Cancelled, ctx);
            }
        }

        [Fact]
        public void ImportFromCsv_InvalidRoundRobinClub_ShouldThrowValidationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new EventDbContext(options);

            // Seed ONLY valid clubs (e.g., RFW, HCRC)
            context.RoundRobinClubs.AddRange(
                new RoundRobinClub { ShortName = "RFW", FullName = "Rockingham Forest Wheelers", WebsiteUrl="URL" },
                new RoundRobinClub { ShortName = "HCRC", FullName = "Horton Cycling & Running Club", WebsiteUrl="URL" }
            );
            context.SaveChanges();

            var importer = new CalendarImporter(context);

            var testCsvPath = "test-data/calendar_invalid_rrclub.csv";
            Directory.CreateDirectory("test-data");

            // Insert a CSV row with an INVALID Round Robin Club: "XYZ"
            File.WriteAllText(
                testCsvPath,
                "Event Number,Date,Start time,Event Name,Round Robin Club,Miles,Location / Course,Hill Climb,Club Championship,Non-Standard 10,Evening 10,Hard Ride Series,Round Robin Event,Sheet Name,isCancelled\n" +
                "1,2025-04-01,18:30,Invalid Club Test,XYZ,10.0,Medbourne,,Y,,Y,,Y,Event_01,\n"
            );

            // Act
            Action act = () => importer.ImportFromCsv(testCsvPath);

            // Assert
            act.Should()
                .Throw<Exception>() // Replace with your specific exception type if you have one
                .WithMessage("*XYZ*"); // Ensure the invalid club code appears in the error message
        }

        [Fact]
        public void ImportFromCsv_WvccAsRoundRobinClub_ShouldBeAccepted()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new EventDbContext(options);

            // Seed ONLY valid clubs (e.g., RFW, HCRC)
            context.RoundRobinClubs.AddRange(
                new RoundRobinClub { ShortName = "RFW", FullName = "Rockingham Forest Wheelers", WebsiteUrl = "URL" },
                new RoundRobinClub { ShortName = "HCRC", FullName = "Horton Cycling & Running Club", WebsiteUrl = "URL" }
            );
            context.SaveChanges();

            var importer = new CalendarImporter(context);

            var testCsvPath = "test-data/calendar_invalid_rrclub.csv";
            Directory.CreateDirectory("test-data");

            // Insert a CSV row with an INVALID Round Robin Club: "XYZ"
            File.WriteAllText(
                testCsvPath,
                "Event Number,Date,Start time,Event Name,Round Robin Club,Miles,Location / Course,Hill Climb,Club Championship,Non-Standard 10,Evening 10,Hard Ride Series,Round Robin Event,Sheet Name,isCancelled\n" +
                "1,2025-04-01,18:30,Invalid Club Test,WVCC,10.0,Medbourne,,Y,,Y,,Y,Event_01,\n"
            );

            // Act
            importer.ImportFromCsv(testCsvPath);

            // Assert
            context.CalendarEvents.Should().HaveCount(1);
        }
    }
}

