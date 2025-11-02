using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Xunit;

namespace ClubProcessor.Tests
{
    public class CompetitorImporterTests
    {
        [Fact]
        public void Import_ShouldRunWithoutError_ForValidCsv()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new CompetitorImporter(context, DateTime.UtcNow);

            var testCsvPath = "test-data/competitors_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                "9999,Doe,John,First Claim,false,false,false,true,false,2025-08-31");

            // Act
            importer.Import(testCsvPath);

            // Assert
            context.Competitors.Should().ContainSingle()
                .Which.ClubNumber.Should().Be(9999);
        }

        [Fact]
        public void Import_ShouldYieldTwoRows_WhenClaimStatusChanges()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importerYesterday = new CompetitorImporter(context, DateTime.UtcNow.AddDays(-1));
            var importerToday = new CompetitorImporter(context, DateTime.UtcNow);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                "9999,Doe,John,First Claim,false,false,false,true,false,2025-08-31");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                "9999,Doe,John,Second Claim,false,false,false,true,false,2025-08-31");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            context.Competitors.Should().HaveCount(2);

            var competitors = context.Competitors.OrderBy(c => c.Id).ToList();
            competitors[0].ClaimStatus.Should().Be(ClaimStatus.FirstClaim);
            competitors[1].ClaimStatus.Should().Be(ClaimStatus.SecondClaim);
        }

        [Fact]
        public void Import_ShouldYieldOneRow_WhenClaimStatusDoesNotChange()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            var testCsvPath1 = "test-data/competitors_test_1.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(
                testCsvPath1,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,First Claim,false,false,false,true,false,{yesterday:yyyy-MM-dd}");

            var testCsvPath2 = "test-data/competitors_test_2.csv";
            File.WriteAllText(
                testCsvPath2,
                "ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate\n" +
                $"9999,Doe,John,First Claim,false,false,false,true,false,{today:yyyy-MM-dd}");

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            context.Competitors.Should().HaveCount(1);

            var competitor = context.Competitors.Single();
            competitor.CreatedUtc.Should().Be(yesterday);
            competitor.LastUpdatedUtc.Should().Be(today);
        }

        [Fact]
        public void Import_ShouldApplyImportDateToTimestamps()
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

            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new CompetitorImporter(context, today);

            // Act
            importer.Import(csvPath);

            // Assert
            var imported = context.Competitors.Single(c => c.ClubNumber == 9999);
            imported.CreatedUtc.Should().Be(importDate);
            imported.LastUpdatedUtc.Should().Be(importDate);
        }

        [Fact]
        public void Import_ShouldApplyImportDateToTimestamps_WhenChangingClaimStatus()
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

            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            context.Competitors.Should().HaveCount(2);

            var competitors = context.Competitors.OrderBy(c => c.Id).ToList();
            competitors[0].ClaimStatus.Should().Be(ClaimStatus.FirstClaim);
            competitors[0].CreatedUtc.Should().Be(earlyImportDate);
            competitors[0].LastUpdatedUtc.Should().Be(earlyImportDate);

            competitors[1].ClaimStatus.Should().Be(ClaimStatus.SecondClaim);
            competitors[1].CreatedUtc.Should().Be(laterImportDate);
            competitors[1].LastUpdatedUtc.Should().Be(laterImportDate);
        }

        [Fact]
        public void Import_ShouldApplyImportDateToTimestamps_WhenOtherPropertyIsChanged()
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

            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importerYesterday = new CompetitorImporter(context, yesterday);
            var importerToday = new CompetitorImporter(context, today);

            // Act
            importerYesterday.Import(testCsvPath1);
            importerToday.Import(testCsvPath2);

            // Assert
            var imported = context.Competitors.Single(c => c.ClubNumber == 9999);
            imported.ClaimStatus.Should().Be(ClaimStatus.FirstClaim);
            imported.CreatedUtc.Should().Be(earlyImportDate);
            imported.LastUpdatedUtc.Should().Be(laterImportDate);
        }

        [Fact]
        public void CompetitorImporter_ImportWithNewSetOfCompetitors_YieldsOnlyCompetitorsFromLatestCsvFile()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            using var context = new CompetitorDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            context.Competitors.AddRange(new[]
            {
                new Competitor
                {
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    IsJuvenile = false,
                    IsJunior = false,
                    IsSenior = true,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 01),
                    LastUpdatedUtc = new DateTime(2025, 01, 01)
                },
                new Competitor
                {
                    ClubNumber = 1002,
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 01),
                    LastUpdatedUtc = new DateTime(2025, 01, 01)
                }
            });
            context.SaveChanges();

            var assembledCompetitors = context.Competitors.OrderBy(c => c.ClubNumber).ToList();
            assembledCompetitors.Should().HaveCount(2);
            assembledCompetitors.Should().Contain(c => c.ClubNumber == 1001 && c.Surname == "Smith");
            assembledCompetitors.Should().Contain(c => c.ClubNumber == 1002 && c.Surname == "Brown");

            var csv = new StringBuilder();
            csv.AppendLine("ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate,VetsBucket");
            csv.AppendLine("2001,Doe,John,First Claim,False,False,False,True,False,2025-01-25,");
            csv.AppendLine("2002,Lee,Sarah,First Claim,True,False,True,False,False,2025-01-25,");
            csv.AppendLine("2003,Khan,Omar,First Claim,False,False,False,False,True,2025-01-25,5");

            var csvPath = "test-data/competitor_timestamp_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(csvPath, csv.ToString());

            var importer = new CompetitorImporter(context, DateTime.UtcNow);

            // Act
            importer.Import(csvPath);

            // Assert
            var allCompetitors = context.Competitors.OrderBy(c => c.ClubNumber).ToList();
            allCompetitors.Should().HaveCount(3);
            allCompetitors.Should().Contain(c => c.ClubNumber == 2001 && c.Surname == "Doe");
            allCompetitors.Should().Contain(c => c.ClubNumber == 2002 && c.Surname == "Lee");
            allCompetitors.Should().Contain(c => c.ClubNumber == 2003 && c.Surname == "Khan");
            allCompetitors.Should().NotContain(c => c.ClubNumber == 1001 || c.ClubNumber == 1002);
        }

        [Fact]
        public void CompetitorImporter_ImportWithMixOfOldAndNewCompetitors_YieldsOnlyCompetitorsFromLatestCsvFileButAllOldRowsForThemToo()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            using var context = new CompetitorDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            // Seed DB with 2 pre-existing competitors
            context.Competitors.AddRange(new[]
            {
                // 1001, Alice Smith - first claim - one row in the SQL table
                new Competitor
                {
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    IsJuvenile = false,
                    IsJunior = false,
                    IsSenior = true,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 01),
                    LastUpdatedUtc = new DateTime(2025, 01, 01)
                },
                // 1002, Bob Browm - first claim, changes to second claim - two rows in the SQL table
                new Competitor
                {
                    ClubNumber = 1002,
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 01),
                    LastUpdatedUtc = new DateTime(2025, 01, 01)
                },
                new Competitor
                {
                    ClubNumber = 1002,
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 02, 01),
                    LastUpdatedUtc = new DateTime(2025, 02, 01)
                },
                // 1003, John Doe - first claim, has been modified (e.g. name spelling correction) - one row in the SQL table
                new Competitor
                {
                    ClubNumber = 1003,
                    Surname = "Doe",
                    GivenName = "John",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = false,
                    IsSenior = true,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 25),
                    LastUpdatedUtc = new DateTime(2025, 01, 25)
                },
                // 1004, Sarah Lee - first claim, then second claim, then first claim then modified - three rows in the SQL table
                new Competitor
                {
                    ClubNumber = 1004,
                    Surname = "Lee",
                    GivenName = "Sarah",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 01, 25),
                    LastUpdatedUtc = new DateTime(2025, 01, 25)
                },
                new Competitor
                {
                    ClubNumber = 1004,
                    Surname = "Lee",
                    GivenName = "Sarah",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = true,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 02, 25),
                    LastUpdatedUtc = new DateTime(2025, 02, 25)
                },
                new Competitor
                {
                    ClubNumber = 1004,
                    Surname = "Lee",
                    GivenName = "Sara",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    IsJuvenile = false,
                    IsJunior = true,
                    IsSenior = false,
                    IsVeteran = false,
                    CreatedUtc = new DateTime(2025, 03, 25),
                    LastUpdatedUtc = new DateTime(2025, 03, 25)
                },
                // 1005, Omar Khan - first claim - one row in the SQL table
                new Competitor
                {
                    ClubNumber = 1005,
                    Surname = "Khan",
                    GivenName = "Omar",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = false,
                    IsSenior = false,
                    IsVeteran = true,
                    VetsBucket = 5,
                    CreatedUtc = new DateTime(2025, 01, 25),
                    LastUpdatedUtc = new DateTime(2025, 01, 25)
                }
            });
            context.SaveChanges();

            var assembledCompetitors = context.Competitors
                .OrderBy(c => c.ClubNumber)
                .ToList();
            Assert.Equal(8, assembledCompetitors.Count);


            // Prepare CSV input with 4 existing competitors, but with isVeteran set True in all cases and all set to First Claim
            var csv = new StringBuilder();
            csv.AppendLine("ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate,VetsBucket");
            csv.AppendLine("1001,Smith,Alice,First Claim,True,False,False,False,True,2025-02-25,5");
            csv.AppendLine("1002,Brown,Bob,First Claim,False,False,False,False,True,2025-02-25,10");
            csv.AppendLine("1003,Doe,John,First Claim,False,False,False,False,True,2025-02-25,1");
            csv.AppendLine("1004,Lee,Sara,First Claim,True,False,False,False,True,2025-02-25,15");

            var csvPath = "test-data/competitor_timestamp_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(csvPath, csv.ToString());

            using var reader = new StringReader(csv.ToString());

            DateTime today = DateTime.UtcNow;
            var importer = new CompetitorImporter(context, today);

            // Act
            importer.Import(csvPath);

            // Assert
            Assert.Equal(1, context.Competitors.Count(c => c.ClubNumber == 1001));
            Assert.Equal(3, context.Competitors.Count(c => c.ClubNumber == 1002));
            Assert.Equal(1, context.Competitors.Count(c => c.ClubNumber == 1003));
            Assert.Equal(3, context.Competitors.Count(c => c.ClubNumber == 1004));
            Assert.Equal(0, context.Competitors.Count(c => c.ClubNumber == 1005));
        }
    }

}

