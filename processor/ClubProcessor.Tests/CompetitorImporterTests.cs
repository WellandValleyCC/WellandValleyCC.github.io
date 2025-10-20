using Xunit;
using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Tests
{
    public class CompetitorImporterTests
    {
        [Fact]
        public void Import_ShouldRunWithoutError_ForValidCsv()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var importer = new CompetitorImporter(context, DateTime.UtcNow);

            var testCsvPath = "test-data/competitors_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9999,Doe,John,First Claim,false,false,false,true,false");

            // Act
            importer.Import(testCsvPath);

            // Assert
            Assert.Single(context.Competitors);
        }

        [Fact]
        public void Import_ShouldYieldTwoRows_WhenClaimStatusChanges()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var importerYesterday = new CompetitorImporter(context, DateTime.UtcNow.AddDays(-1));
            var importerToday = new CompetitorImporter(context, DateTime.UtcNow);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9997,Doe,John,First Claim,false,false,false,true,false");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9997,Doe,John,Second Claim,false,false,false,true,false");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            Assert.Equal(2, context.Competitors.Where(c => c.ClubNumber == "9997").Count());
            var first = context.Competitors.Where(c => c.ClubNumber == "9997").OrderBy(c => c.Id).First();
            Assert.Equal("First Claim", first.ClaimStatus);
            var second = context.Competitors.Where(c => c.ClubNumber == "9997").OrderBy(c => c.Id).Skip(1).First();
            Assert.Equal("Second Claim", second.ClaimStatus);
        }

        [Fact]
        public void Import_ShouldYieldOneRow_WhenClaimStatusDoesNotChange()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var today = DateTime.UtcNow;
            var yesterday = today.AddDays(-1);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9998,Doe,John,First Claim,false,false,false,true,false");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9998,Doe,John,First Claim,false,false,false,true,false");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            Assert.Equal(1, context.Competitors.Where(c => c.ClubNumber == "9998").Count());
            var competitor = context.Competitors.Where(c => c.ClubNumber == "9998").First();
            Assert.Equal(yesterday, competitor.CreatedUtc);
            Assert.Equal(today, competitor.LastUpdatedUtc);
        }
    }
}
