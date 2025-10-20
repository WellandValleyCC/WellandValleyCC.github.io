using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

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
                "9999,Doe,John,First Claim,false,false,false,true,false");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9999,Doe,John,Second Claim,false,false,false,true,false");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            Assert.Equal(2, context.Competitors.Count());
            var first = context.Competitors.OrderBy(c => c.Id).First();
            Assert.Equal("First Claim", first.ClaimStatus);
            var second = context.Competitors.OrderBy(c => c.Id).Skip(1).First();
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
                "9999,Doe,John,First Claim,false,false,false,true,false");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
                "9999,Doe,John,First Claim,false,false,false,true,false");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            Assert.Equal(1, context.Competitors.Count());
            var competitor = context.Competitors.First();
            Assert.Equal(yesterday, competitor.CreatedUtc);
            Assert.Equal(today, competitor.LastUpdatedUtc);
        }

        [Theory, AutoData]
        public void Import_ShouldApplyImportDateToTimestamps(Competitor template)
        {
            // Arrange
            var importDate = new DateTime(2025, 2, 20);
            var today = DateTime.UtcNow;

            var csvPath = "test-data/competitor_timestamp_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                csvPath,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,First Claim,false,false,true,false,false,{importDate:yyyy-MM-dd}");

            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var importer = new CompetitorImporter(context, today);

            // Act
            importer.Import(csvPath);

            // Assert
            var imported = context.Competitors.Single(c => c.ClubNumber == "9999");
            Assert.Equal(importDate, imported.CreatedUtc);
            Assert.Equal(importDate, imported.LastUpdatedUtc);
        }

        [Theory, AutoData]
        public void Import_ShouldApplyImportDateToTimestamps_WhenChangingClaimStatus(Competitor template)
        {
            // Arrange
            var earlyImportDate = new DateTime(2025, 2, 20);
            var laterImportDate = new DateTime(2025, 3, 20);
            var today = DateTime.UtcNow;
            var yesterday = DateTime.UtcNow.AddDays(-1);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,First Claim,false,false,false,true,false,{earlyImportDate:yyyy-MM-dd}");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,Second Claim,false,false,false,true,false,{laterImportDate:yyyy-MM-dd}");

            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            Assert.Equal(2, context.Competitors.Count());
            var first = context.Competitors.OrderBy(c => c.Id).First();
            Assert.Equal("First Claim", first.ClaimStatus);
            Assert.Equal(earlyImportDate, first.CreatedUtc);
            Assert.Equal(earlyImportDate, first.LastUpdatedUtc);
            var second = context.Competitors.OrderBy(c => c.Id).Skip(1).First();
            Assert.Equal("Second Claim", second.ClaimStatus);
            Assert.Equal(laterImportDate, second.CreatedUtc);
            Assert.Equal(laterImportDate, second.LastUpdatedUtc);
        }



        [Theory, AutoData]
        public void Import_ShouldApplyImportDateToTimestamps_WhenOtherPropertyIsChanged(Competitor template)
        {
            // Arrange
            var earlyImportDate = new DateTime(2025, 2, 20);
            var laterImportDate = new DateTime(2025, 3, 20);
            var today = DateTime.UtcNow;
            var yesterday = DateTime.UtcNow.AddDays(-1);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,Johnathon,First Claim,false,false,false,true,false,{earlyImportDate:yyyy-MM-dd}");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,First Claim,false,false,false,true,false,{laterImportDate:yyyy-MM-dd}");

            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ClubDbContext(options);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            var imported = context.Competitors.Single(c => c.ClubNumber == "9999");
            Assert.Equal("First Claim", imported.ClaimStatus);
            Assert.Equal(earlyImportDate, imported.CreatedUtc);
            Assert.Equal(laterImportDate, imported.LastUpdatedUtc);
        }
    }
}
