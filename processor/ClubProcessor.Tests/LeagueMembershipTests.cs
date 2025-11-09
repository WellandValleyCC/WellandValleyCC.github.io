using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ClubProcessor.Tests
{
    public class LeagueMembershipImporterTests
    {
        [Fact]
        public void Import_UpdatesCompetitorLeagueAssignmentsAndClearsObsolete()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            context.Competitors.AddRange(new[]
            {
                new Competitor
                {
                    Id = 1,
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    League = League.Undefined
                },
                new Competitor
                {
                    Id = 2,
                    ClubNumber = 1002,
                    Surname = "Jones",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    League = League.Premier
                },
                new Competitor
                {
                    Id = 3,
                    ClubNumber = 1003,
                    Surname = "Taylor",
                    GivenName = "Charlie",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    League = League.League2
                }
            });
            context.SaveChanges();

            // Simulate Leagues_YYYY.csv contents
            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubNumber, ClubMemberName, LeagueDivision",
                "1001,Alice Smith,1",     // Alice newly assigned to League 1
                "1002,Bob Jones,Prem"   // Bob remains in Prem
                // Charlie (ClubNumber==1003) missing --> should be cleared
            });

            // Act
            importer.Import(csvPath);

            // Assert
            var alice = context.Competitors.Single(c => c.Id == 1);
            alice.League.Should().Be(League.League1,
                because: "Alice (ClubNumber 1001) should be assigned to League 1");

            var bob = context.Competitors.Single(c => c.Id == 2);
            bob.League.Should().Be(League.Premier,
                because: "Bob (ClubNumber 1002) should remain in Premier League");

            var charlie = context.Competitors.Single(c => c.Id == 3);
            charlie.League.Should().Be(League.Undefined,
                because: "Charlie (ClubNumber 1003) should be cleared from league assignment");
        }

        [Fact]
        public void Import_WithMultiRowCompetitorts_UpdatesCompetitorLeagueAssignmentsAndClearsObsolete()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            var oldCompetitorImportDate = DateTime.UtcNow.AddDays(-30);
            var newerCompetitorImportDate = DateTime.UtcNow.AddDays(-5);

            context.Competitors.AddRange(new[]
            {
                // old rows
                new Competitor
                {
                    Id = 1,
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = true,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldCompetitorImportDate,
                    LastUpdatedUtc = oldCompetitorImportDate,
                    League = League.Undefined
                },
                new Competitor
                {
                    Id = 2,
                    ClubNumber = 1002,
                    Surname = "Jones",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldCompetitorImportDate,
                    LastUpdatedUtc = oldCompetitorImportDate,
                    League = League.Premier
                },
                new Competitor
                {
                    Id = 3,
                    ClubNumber = 1003,
                    Surname = "Taylor",
                    GivenName = "Charlie",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldCompetitorImportDate,
                    LastUpdatedUtc = oldCompetitorImportDate,
                    League = League.League2
                },
                // new rows (simulating multi-row competitors)
                new Competitor
                {
                    Id = 4,
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newerCompetitorImportDate,
                    LastUpdatedUtc = newerCompetitorImportDate,
                    League = League.Undefined
                },
                new Competitor
                {
                    Id = 5,
                    ClubNumber = 1002,
                    Surname = "Jones",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newerCompetitorImportDate,
                    LastUpdatedUtc = newerCompetitorImportDate,
                    League = League.Premier
                },
                new Competitor
                {
                    Id = 6,
                    ClubNumber = 1003,
                    Surname = "Taylor",
                    GivenName = "Charlie",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newerCompetitorImportDate,
                    LastUpdatedUtc = newerCompetitorImportDate,
                    League = League.League2
                }
            });
            context.SaveChanges();

            // Simulate Leagues_YYYY.csv contents
            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubNumber, ClubMemberName, LeagueDivision",
                "1001,Alice Smith,1",     // Alice newly assigned to League 1
                "1002,Bob Jones,Prem"   // Bob remains in Prem
                // Charlie (ClubNumber==1003) missing --> should be cleared
            });

            // Act
            importer.Import(csvPath);

            // Assert
            // Alice: all rows for ClubNumber 1001 should be League1
            var aliceRows = context.Competitors.Where(c => c.ClubNumber == 1001).ToList();
            aliceRows.Should().NotBeEmpty("Alice should have competitor rows");
            aliceRows.Should().OnlyContain(c => c.League == League.League1,
                because: "Alice (ClubNumber 1001) should be assigned to League 1 in all rows");

            // Bob: all rows for ClubNumber 1002 should remain Premier
            var bobRows = context.Competitors.Where(c => c.ClubNumber == 1002).ToList();
            bobRows.Should().NotBeEmpty("Bob should have competitor rows");
            bobRows.Should().OnlyContain(c => c.League == League.Premier,
                because: "Bob (ClubNumber 1002) should remain in Premier League across all rows");

            // Charlie: all rows for ClubNumber 1003 should be cleared
            var charlieRows = context.Competitors.Where(c => c.ClubNumber == 1003).ToList();
            charlieRows.Should().NotBeEmpty("Charlie should have competitor rows");
            charlieRows.Should().OnlyContain(c => c.League == League.Undefined,
                because: "Charlie (ClubNumber 1003) should be cleared from league assignment in all rows");
        }

        [Fact]
        public void Import_WhenCsvMissingClubNumberColumn_ShouldThrowFormatException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            // Malformed CSV: missing ClubNumber column
            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubMemberName,League",   // Only 2 headers instead of 3
                "Alice Smith,1",
                "Bob Jones,Prem"
            });

            // Act
            Action act = () => importer.Import(csvPath);

            // Assert
            act.Should().Throw<FormatException>()
               .WithMessage("*Leagues CSV is missing required column: ClubNumber*");
        }

        [Fact]
        public void Import_WhenDuplicateLeagueEntriesInCsv_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            context.Competitors.Add(new Competitor
            {
                Id = 1,
                ClubNumber = 1001,
                Surname = "Smith",
                GivenName = "Alice",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = true,
                AgeGroup = AgeGroup.Senior,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow,
                League = League.Undefined
            });
            context.SaveChanges();

            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubNumber,ClubMemberName,LeagueDivision",
                "1001,Alice Smith,Prem",
                "1001,Alice Smith,1" // duplicate row for same competitor
            });

            // Act
            Action act = () => importer.Import(csvPath);

            // Assert
            act.Should().Throw<FormatException>()
               .WithMessage("*duplicate competitor rows*")
               .WithMessage("*1001-Alice Smith*");
        }

        [Fact]
        public void Import_WhenCompetitorMissingFromCsv_ShouldClearLeagueAssignmentsAcrossAllRows()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            // Arrange
            var oldDate = DateTime.UtcNow.AddDays(-30);
            var newDate = DateTime.UtcNow.AddDays(-5);

            context.Competitors.AddRange(new[]
            {
                // Alice (ClubNumber 1001) – two rows, initially Undefined
                new Competitor
                {
                    Id = 1,
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = true,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldDate,
                    LastUpdatedUtc = oldDate,
                    League = League.Undefined
                },
                new Competitor
                {
                    Id = 101,
                    ClubNumber = 1001,
                    Surname = "Smith",
                    GivenName = "Alice",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = true,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newDate,
                    LastUpdatedUtc = newDate,
                    League = League.Undefined
                },

                // Bob (ClubNumber 1002) – two rows, initially Premier
                new Competitor
                {
                    Id = 2,
                    ClubNumber = 1002,
                    Surname = "Jones",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldDate,
                    LastUpdatedUtc = oldDate,
                    League = League.Premier
                },
                new Competitor
                {
                    Id = 102,
                    ClubNumber = 1002,
                    Surname = "Jones",
                    GivenName = "Bob",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newDate,
                    LastUpdatedUtc = newDate,
                    League = League.Premier
                },

                // Charlie (ClubNumber 1003) – two rows, initially League2
                new Competitor
                {
                    Id = 3,
                    ClubNumber = 1003,
                    Surname = "Taylor",
                    GivenName = "Charlie",
                    ClaimStatus = ClaimStatus.SecondClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = oldDate,
                    LastUpdatedUtc = oldDate,
                    League = League.League2
                },
                new Competitor
                {
                    Id = 103,
                    ClubNumber = 1003,
                    Surname = "Taylor",
                    GivenName = "Charlie",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = newDate,
                    LastUpdatedUtc = newDate,
                    League = League.League2
                }
            });
            context.SaveChanges();

            // CSV does not include Charlie at all
            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubNumber,ClubMemberName,LeagueDivision",
                "1001,Alice Smith,1",
                "1002,Bob Jones,Prem"
            });

            // Act
            importer.Import(csvPath);

            // Assert
            var charlieRows = context.Competitors.Where(c => c.ClubNumber == 1003).ToList();
            charlieRows.Should().NotBeEmpty("Charlie should have competitor rows");
            charlieRows.Should().OnlyContain(c => c.League == League.Undefined,
                because: "Charlie (ClubNumber 1003) is missing from the CSV and should be cleared in all rows");
        }

    }
}
