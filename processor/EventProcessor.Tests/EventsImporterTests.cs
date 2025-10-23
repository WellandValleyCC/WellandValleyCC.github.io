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
    public class EventsImporterTests
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
            Assert.Equal(21, eventContext.Rides.Count());
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
            var ride = eventContext.Rides.SingleOrDefault(r => r.Name == "Theo Marlin" && r.EventNumber == 1);
            Assert.NotNull(ride);
            Assert.Equal(101, ride.ClubNumber);
            Assert.Equal("00:24:18", ride.ActualTime);
            Assert.Equal(1458.0, ride.TotalSeconds, 2);
            Assert.True(ride.IsRoadBike);
            Assert.Equal(RideEligibility.DNF, ride.Eligibility);
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
                "ClubNumber,Name\n101,Theo Marlin");

            // Create one sample event file
            var eventCsv = new[]
            {
                "Number/Name,H,M,S,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed",
                "101,0.0,24.0,18.0,r,DNF,Theo Marlin,00:24:18,",
                "102,0.0,24.0,53.0,,,Lena Corwin,00:24:53,",
                "103,0.0,26.0,1.0,,,Jasper Flint,00:26:01,",
                "104,0.0,26.0,15.0,,,Mira Voss,00:26:15,",
                "105,0.0,26.0,17.0,r,,Nico Bramley,00:26:17,",
                "106,0.0,26.0,29.0,r,,Kian Mercer,00:26:29,",
                "107,0.0,26.0,44.0,,,Rhea Talbot,00:26:44,",
                "108,0.0,26.0,46.0,r,,Zane Holloway,00:26:46,",
                "109,0.0,26.0,55.0,,,Talia Wren,00:26:55,",
                "110,0.0,27.0,21.0,r,,Elior Quinn,00:27:21,",
                "111,0.0,27.0,23.0,r,,Sage Delaney,00:27:23,",
                "Nova Penn,0.0,27.0,30.0,,,Nova Penn,00:27:30,X",
                "112,0.0,27.0,35.0,r,,Rex Calder,00:27:35,",
                "Isla Fenwick,0.0,28.0,42.0,r,,Isla Fenwick,00:28:42,X",
                "113,0.0,28.0,45.0,,,Dorian Pike,00:28:45,",
                "114,0.0,29.0,28.0,,,Arlo Vane,00:29:28,",
                "115,0.0,29.0,30.0,r,,Lyra Maddox,00:29:30,",
                "116,0.0,30.0,25.0,r,,Finnley Rhodes,00:30:25,",
                "117,0.0,30.0,30.0,,,Juno Merrick,00:30:30,",
                "118,0.0,30.0,50.0,,,Cassian Lowe,00:30:50,",
                "119,0.0,31.0,27.0,,,Bryn Hollis,00:31:27,",
            };

            File.WriteAllLines(Path.Combine(eventsFolder, "Event_01.csv"), eventCsv);

            return yearFolder; // Return path to 2025 folder
        }
    }
}
