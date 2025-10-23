using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Xunit;

namespace EventProcessor.Tests
{
    public class EventProcessorTests
    {
        private EventDbContext CreateEventContext()
        {
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseInMemoryDatabase(databaseName: "EventDb_" + Guid.NewGuid())
                .Options;

            return new EventDbContext(options);
        }

        private CompetitorDbContext CreateCompetitorContext()
        {
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(databaseName: "CompetitorDb_" + Guid.NewGuid())
                .Options;

            return new CompetitorDbContext(options);
        }

        [Fact]
        public void ProcessFolder_ValidCsvFiles_ProcessesEvents()
        {
            // Arrange
            var eventContext = CreateEventContext();
            var competitorContext = CreateCompetitorContext();
            var importer = new EventsImporter(eventContext, competitorContext);
            var folderPath = CreateTestFolderWithCsvs();

            // Act
            importer.ImportFromFolder(folderPath);

            // Assert
            Assert.True(eventContext.Rides.Any()); // Basic assertion — refine as needed
        }

        [Fact]
        public void ImportFromFolder_WithValidStructure_ImportsEventData()
        {
            // Arrange
            var eventContext = CreateEventContext();
            var competitorContext = CreateCompetitorContext();
            var importer = new EventsImporter(eventContext, competitorContext);
            var folderPath = CreateTestFolderWithCsvs();

            // Act
            importer.ImportFromFolder(folderPath);

            // Assert
            var ride = eventContext.Rides.SingleOrDefault(r => r.Name == "Ian Allen" && r.EventNumber == 1);
            Assert.NotNull(ride);
            Assert.Equal(1458.5, ride.TotalSeconds, 2);
            Assert.True(ride.IsRoadBike);
        }

        private string CreateTestFolderWithCsvs()
        {
            var root = Path.Combine(Path.GetTempPath(), "TestExtracted_" + Guid.NewGuid());
            var yearFolder = Path.Combine(root, "2025");
            var eventsFolder = Path.Combine(yearFolder, "events");

            Directory.CreateDirectory(eventsFolder);

            // Create dummy calendar and competitor files
            File.WriteAllText(Path.Combine(yearFolder, "calendar_2025.csv"),
                "Date,EventName\n2025-05-01,TT01");

            File.WriteAllText(Path.Combine(yearFolder, "competitors_2025.csv"),
                "ClubNumber,Name\n1286,Ian Allen");

            // Create one sample event file
            var eventCsv = new[]
            {
                "Number/Name,H,M,S,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed",
                "1286,0,24,18.5,r,,Ian Allen,00:24:18.5,"
            };

            File.WriteAllLines(Path.Combine(eventsFolder, "Event_01.csv"), eventCsv);

            return yearFolder; // Return path to 2025 folder
        }
    }
}
