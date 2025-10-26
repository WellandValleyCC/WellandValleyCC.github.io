using ClubProcessor.Context;
using ClubProcessor.Services;
using CsvHelper;
using CsvHelper.Configuration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace EventProcessor.Tests
{
    public class CalendarImporterTests
    {
        /// <remarks>
        /// EventID,Date,Start time,Event Name,Miles,Location / Course,Hill Climb,	Club Championship,	Non-Standard 10,	Evening 10,	Hard Ride Series,Sheet Name,isCancelled
        /// 1,2025-04-01,18:30,Medbourne 9.5mile Hardride TT,9.5,Medbourne,,Y,Y,,Y,Event_01,
        /// 4,2025-04-20,08:00,Kibworth 25mile TT,25.0,Kibworth,,Y,Y,,,Event_04,
        /// 5,2025-04-29,18:30,Tour of Langtons 10mile TT,10.0,Tour of Langtons,,Y,,Y,,Event_05,
        /// 23,2025-09-07,10:30:00,Interclub Hillclimb RFW,0.0,Nevill Holt ,Y,Y,Y,,,Event_23, 
        /// </remarks>
        [Fact]
        public void ParseCalendarEvents_ValidCsv_ParsesCorrectly()
        {
            // Arrange
            var csvContent = @"EventID,Date,Start time,Event Name,Miles,Location / Course,Hill Climb,Club Championship,Non-Standard 10,Evening 10,Hard Ride Series,Sheet Name,isCancelled
1,2025-04-01,18:30,Medbourne TT,9.5,Medbourne,Y,Y,N,Y,Y,Event_01,N";

            using var reader = new StringReader(csvContent);
            using var context = DbContextFactory.CreateEventContext();
            var importer = new CalendarImporter(context);

            // Act
            var records = importer.ParseCalendarEvents(reader);

            // Assert
            records.Should().HaveCount(1);
            var evt = records[0];

            evt.Should().NotBeNull();
            evt.EventID.Should().Be(1);
            evt.EventDate.Should().Be(new DateTime(2025, 4, 1));
            evt.StartTime.Should().Be(TimeSpan.FromMinutes(18 * 60 + 30));
            evt.EventName.Should().Be("Medbourne TT");
            evt.Miles.Should().Be(9.5);
            evt.Location.Should().Be("Medbourne");
            evt.IsHillClimb.Should().BeTrue();
            evt.IsClubChampionship.Should().BeTrue();
            evt.IsNonStandard10.Should().BeFalse();
            evt.IsEvening10.Should().BeTrue();
            evt.IsHardRideSeries.Should().BeTrue();
            evt.SheetName.Should().Be("Event_01");
            evt.IsCancelled.Should().BeFalse();
        }
    }
}