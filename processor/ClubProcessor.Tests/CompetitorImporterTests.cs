using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Services;
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

        [Fact]
        public void CompetitorImporter_ImportWithNewSetOfCompetitors_YieldsOnlyCompetitorsFromLatestCsvFile()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            using var context = new ClubDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            // Seed DB with 2 pre-existing competitors
            context.Competitors.AddRange(new[]
            {
                new Competitor
                {
                    ClubNumber = "1001",
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1002",
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = "First Claim",
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

            var assembledCompetitors = context.Competitors
                .OrderBy(c => c.ClubNumber)
                .ToList();
            Assert.Equal(2, assembledCompetitors.Count);
            Assert.Contains(assembledCompetitors, c => c.ClubNumber == "1001" && c.Surname == "Smith");
            Assert.Contains(assembledCompetitors, c => c.ClubNumber == "1002" && c.Surname == "Brown");

            // Prepare CSV input with 3 new competitors
            var csv = new StringBuilder();
            csv.AppendLine("ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate");
            csv.AppendLine("2001,Doe,John,First Claim,False,False,False,True,False,2025-01-25");
            csv.AppendLine("2002,Lee,Sarah,First Claim,True,False,True,False,False,2025-01-25");
            csv.AppendLine("2003,Khan,Omar,First Claim,False,False,False,True,True,2025-01-25");

            var csvPath = "test-data/competitor_timestamp_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(csvPath, csv.ToString());

            using var reader = new StringReader(csv.ToString());

            DateTime today = DateTime.UtcNow;
            var importer = new CompetitorImporter(context, today);

            // Act
            importer.Import(csvPath);

            // Assert
            var allCompetitors = context.Competitors
                .OrderBy(c => c.ClubNumber)
                .ToList();

            Assert.Equal(3, allCompetitors.Count);
            Assert.Contains(allCompetitors, c => c.ClubNumber == "2001" && c.Surname == "Doe");
            Assert.Contains(allCompetitors, c => c.ClubNumber == "2002" && c.Surname == "Lee");
            Assert.Contains(allCompetitors, c => c.ClubNumber == "2003" && c.Surname == "Khan");
            Assert.DoesNotContain(allCompetitors, c => c.ClubNumber == "1001" || c.ClubNumber == "1002");
        }

        [Fact]
        public void CompetitorImporter_ImportWithMixOfOldAndNewCompetitors_YieldsOnlyCompetitorsFromLatestCsvFileButAllOldRowsForThemToo()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            using var context = new ClubDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            // Seed DB with 2 pre-existing competitors
            context.Competitors.AddRange(new[]
            {
                // 1001, Alice Smith - first claim - one row in the SQL table
                new Competitor
                {
                    ClubNumber = "1001",
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1002",
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1002",
                    Surname = "Brown",
                    GivenName = "Bob",
                    ClaimStatus = "Second Claim",
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
                    ClubNumber = "1003",
                    Surname = "Doe",
                    GivenName = "John",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1004",
                    Surname = "Lee",
                    GivenName = "Sarah",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1004",
                    Surname = "Lee",
                    GivenName = "Sarah",
                    ClaimStatus = "Second Claim",
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
                    ClubNumber = "1004",
                    Surname = "Lee",
                    GivenName = "Sara",
                    ClaimStatus = "First Claim",
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
                    ClubNumber = "1005",
                    Surname = "Khan",
                    GivenName = "Omar",
                    ClaimStatus = "First Claim",
                    IsFemale = false,
                    IsJuvenile = false,
                    IsJunior = false,
                    IsSenior = true,
                    IsVeteran = true,
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
            csv.AppendLine("ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran,ImportDate");
            csv.AppendLine("1001,Smith,Alice,First Claim,True,False,False,False,True,2025-02-25");
            csv.AppendLine("1002,Brown,Bob,First Claim,False,False,False,False,True,2025-02-25");
            csv.AppendLine("1003,Doe,John,First Claim,False,False,False,False,True,2025-02-25");
            csv.AppendLine("1004,Lee,Sara,First Claim,True,False,False,False,True,2025-02-25");

            var csvPath = "test-data/competitor_timestamp_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(csvPath, csv.ToString());

            using var reader = new StringReader(csv.ToString());

            DateTime today = DateTime.UtcNow;
            var importer = new CompetitorImporter(context, today);

            // Act
            importer.Import(csvPath);

            // Assert
            Assert.Equal(1, context.Competitors.Count(c => c.ClubNumber == "1001"));
            Assert.Equal(3, context.Competitors.Count(c => c.ClubNumber == "1002"));
            Assert.Equal(1, context.Competitors.Count(c => c.ClubNumber == "1003"));
            Assert.Equal(3, context.Competitors.Count(c => c.ClubNumber == "1004"));
            Assert.Equal(0, context.Competitors.Count(c => c.ClubNumber == "1005"));
        }
    }
}

