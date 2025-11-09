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
            alice.League.Should().Be(League.League1, "Alice (ClubNumber 1001) should be assigned to League 1");

            var bob = context.Competitors.Single(c => c.Id == 2);
            bob.League.Should().Be(League.Premier, "Bob (ClubNumber 1002) should remain in Premier League");

            var charlie = context.Competitors.Single(c => c.Id == 3);
            charlie.League.Should().Be(League.Undefined, "Charlie (ClubNumber 1003) should be cleared from league assignment");
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
        public void Import_WhenMultipleLeagueEntriesMatch_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new CompetitorDbContext(options);
            var importer = new LeagueMembershipImporter(context, DateTime.UtcNow);

            // Two competitors with same ClubNumber + Name
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
                    Surname = "Smith",
                    GivenName = "John",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = false,
                    AgeGroup = AgeGroup.Senior,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow,
                    League = League.Undefined
                }
            });
            context.SaveChanges();

            var csvPath = Path.GetTempFileName();
            File.WriteAllLines(csvPath, new[]
            {
                "ClubNumber,ClubMemberName,LeagueDivision",
                "1001,Alice Smith,Prem",
                "1002,John Smith,Prem",
                "1001,Alice Smith,1"
            });

            // Act
            Action act = () => importer.Import(csvPath);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*multiple competitors found*")
               .WithMessage("*ClubNumber=1001*")
               .WithMessage("*Name=Alice Smith*");
        }

    }
}
