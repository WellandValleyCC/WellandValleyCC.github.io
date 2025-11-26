using ClubCore.Models;
using ClubSiteGenerator.Services;
using FluentAssertions;
using AutoFixture;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Tests
{
    public class DataLoaderTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void AttachReferencesToRides_ShouldAttachCompetitorAndCalendarEvent()
        {
            // Arrange
            var competitor = _fixture.Build<Competitor>()
                .With(c => c.ClubNumber, 123)
                .With(c => c.AgeGroup, AgeGroup.Senior)
                .With(c => c.VetsBucket, (int?)null)
                .With(c => c.ClaimStatus, ClaimStatus.FirstClaim)
                .With(c => c.CreatedUtc, new DateTime(2024, 01, 01))
                .Create();

            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Alice Ride")
                .With(r => r.ClubNumber, 123)
                .With(r => r.EventNumber, 1)
                .With(r => r.Competitor, (Competitor?)null)
                .Create();

            // Act
            DataLoader.AttachReferencesToRides(
                new[] { ride },
                new[] { competitor },
                new[] { calendarEvent });

            // Assert
            ride.Competitor.Should().Be(competitor);
            ride.CalendarEvent.Should().Be(calendarEvent);
        }

        [Fact]
        public void AttachReferencesToRides_ShouldThrowIfCalendarEventMissing()
        {
            var competitor = new Competitor
            {
                ClubNumber = 123,
                GivenName = "Alice",
                Surname = "Smith",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,
                AgeGroup = AgeGroup.Senior,
                CreatedUtc = new DateTime(2024, 01, 01),
                LastUpdatedUtc = new DateTime(2024, 01, 01),
                League = League.Undefined
            };


            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Bob Ride")
                .With(r => r.ClubNumber, 456)
                .With(r => r.EventNumber, 99)
                .With(r => r.Competitor, (Competitor?)null)
                .Create();

            Action act = () => DataLoader.AttachReferencesToRides(
                new[] { ride },
                new[] { competitor },
                Array.Empty<CalendarEvent>());

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.EventNumber}*");
        }

        [Fact]
        public void AttachReferencesToRides_ShouldThrowIfCompetitorMissing()
        {
            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Charlie Ride")
                .With(r => r.ClubNumber, 999)
                .With(r => r.EventNumber, 1)
                .With(r => r.Competitor, (Competitor?)null)
                .Create();

            Action act = () => DataLoader.AttachReferencesToRides(
                new[] { ride },
                Array.Empty<Competitor>(),
                new[] { calendarEvent });

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.ClubNumber}*");
        }

        [Fact]
        public void AttachReferencesToRides_ShouldPickLatestCompetitorSnapshotBeforeEventDate()
        {
            var snapshots = new[]
            {
                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket) 
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "OldSnapshotGivenName")
                    .With(c => c.Surname, "OldSnapshotSurname")
                    .With(c => c.AgeGroup, AgeGroup.Senior)
                    .With(c => c.ClaimStatus, ClaimStatus.SecondClaim)
                    .With(c => c.CreatedUtc, new DateTime(2024, 01, 01))
                    .Without(c => c.VetsBucket)
                    .Create(),
                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket) 
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "NewSnapshotGivenName")
                    .With(c => c.Surname, "NewSnapshotSurname")
                    .With(c => c.AgeGroup, AgeGroup.Senior)
                    .With(c => c.ClaimStatus, ClaimStatus.FirstClaim)
                    .With(c => c.CreatedUtc, new DateTime(2024, 05, 01))
                    .Without(c => c.VetsBucket)
                    .Create(),
                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket)
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "TooLateSnapshotGivenName")
                    .With(c => c.Surname, "TooLateSnapshotSurname")
                    .With(c => c.AgeGroup, AgeGroup.Senior)
                    .With(c => c.ClaimStatus, ClaimStatus.SecondClaim)
                    .With(c => c.CreatedUtc, new DateTime(2024, 07, 01))
                    .Without(c => c.VetsBucket)
                    .Create()
            };

            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Alice Ride")
                .With(r => r.ClubNumber, 123)
                .With(r => r.EventNumber, 1)
                .With(r => r.Competitor, (Competitor?)null)
                .Create();

            DataLoader.AttachReferencesToRides(
                new[] { ride },
                snapshots,
                new[] { calendarEvent });

            ride?.Competitor?.FullName.Should().Be("NewSnapshotGivenName NewSnapshotSurname");
        }

        [Fact]
        public void AttachReferencesToRides_ShouldThrowIfNoSnapshotBeforeEventDate()
        {
            var snapshots = new[]
            {
                _fixture.Build<Competitor>()
                .Without(c => c.VetsBucket)
                .With(c => c.ClubNumber, 123)
                .With(c => c.GivenName, "TooLateSnapshotGivenName")
                .With(c => c.Surname, "TooLateSnapshotSurname")
                .With(c => c.AgeGroup, AgeGroup.Senior)
                .With(c => c.ClaimStatus, ClaimStatus.SecondClaim)
                .With(c => c.CreatedUtc, new DateTime(2024, 07, 01))
                .Without(c => c.VetsBucket)
                .Create()
            };

            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "TooLateSnapshotGivenName TooLateSnapshotSurname")
                .With(r => r.ClubNumber, 123)
                .With(r => r.EventNumber, 1)
                .With(r => r.Competitor, (Competitor?)null)
                .Create();

            Action act = () => DataLoader.AttachReferencesToRides(
                new[] { ride },
                snapshots,
                new[] { calendarEvent });

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.ClubNumber}*");
        }
    }
}
