using AutoFixture;
using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Services.Hydration;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class RideHydratorTests
    {
        private readonly Fixture _fixture = new();

        public RideHydratorTests()
        {
            _fixture.Customize<Competitor>(c =>
                c.Without(x => x.VetsBucket));

            _fixture.Customize<Ride>(c =>
                c.Without(r => r.Competitor));
        }

        // ------------------------------------------------------------
        // Calendar Event Hydration
        // ------------------------------------------------------------

        [Fact]
        public void AttachCalendarEvents_ShouldAttachCalendarEvent()
        {
            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Alice Ride")
                .With(r => r.EventNumber, 1)
                .With(r => r.CalendarEvent, (CalendarEvent?)null)
                .Create();

            RideHydrator.AttachCalendarEvents(new[] { ride }, new[] { calendarEvent });

            ride.CalendarEvent.Should().Be(calendarEvent);
        }

        [Fact]
        public void AttachCalendarEvents_ShouldThrowIfCalendarEventMissing()
        {
            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Bob Ride")
                .With(r => r.EventNumber, 99)
                .Create();

            Action act = () =>
                RideHydrator.AttachCalendarEvents(new[] { ride }, Array.Empty<CalendarEvent>());

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.EventNumber}*");
        }

        // ------------------------------------------------------------
        // Competitor Hydration
        // ------------------------------------------------------------

        [Fact]
        public void AttachCompetitors_ShouldAttachCompetitorSnapshot()
        {
            var competitor = _fixture.Build<Competitor>()
                .With(c => c.ClubNumber, 123)
                .With(c => c.CreatedUtc, new DateTime(2024, 01, 01))
                .Without(c => c.VetsBucket)
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

            RideHydrator.AttachCompetitors(new[] { ride }, new[] { competitor }, new[] { calendarEvent });

            ride.Competitor.Should().Be(competitor);
        }

        [Fact]
        public void AttachCompetitors_ShouldThrowIfCompetitorMissing()
        {
            var calendarEvent = _fixture.Build<CalendarEvent>()
                .With(e => e.EventNumber, 1)
                .With(e => e.EventDate, new DateTime(2024, 06, 01))
                .Create();

            var ride = _fixture.Build<Ride>()
                .With(r => r.Name, "Charlie Ride")
                .With(r => r.ClubNumber, 999)
                .With(r => r.EventNumber, 1)
                .Create();

            Action act = () =>
                RideHydrator.AttachCompetitors(new[] { ride }, Array.Empty<Competitor>(), new[] { calendarEvent });

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.ClubNumber}*");
        }

        [Fact]
        public void AttachCompetitors_ShouldPickLatestSnapshotBeforeEventDate()
        {
            var snapshots = new[]
            {
                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket)
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "OldSnapshotGivenName")
                    .With(c => c.Surname, "OldSnapshotSurname")
                    .With(c => c.CreatedUtc, new DateTime(2024, 01, 01))
                    .Create(),

                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket)
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "NewSnapshotGivenName")
                    .With(c => c.Surname, "NewSnapshotSurname")
                    .With(c => c.CreatedUtc, new DateTime(2024, 05, 01))
                    .Create(),

                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket)
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "TooLateSnapshotGivenName")
                    .With(c => c.Surname, "TooLateSnapshotSurname")
                    .With(c => c.CreatedUtc, new DateTime(2024, 07, 01))
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
                .Create();

            RideHydrator.AttachCompetitors(new[] { ride }, snapshots, new[] { calendarEvent });

            ride.Competitor!.FullName.Should().Be("NewSnapshotGivenName NewSnapshotSurname");
        }

        [Fact]
        public void AttachCompetitors_ShouldThrowIfNoSnapshotBeforeEventDate()
        {
            var snapshots = new[]
            {
                _fixture.Build<Competitor>()
                    .Without(c => c.VetsBucket)
                    .With(c => c.ClubNumber, 123)
                    .With(c => c.GivenName, "TooLateSnapshotGivenName")
                    .With(c => c.Surname, "TooLateSnapshotSurname")
                    .With(c => c.CreatedUtc, new DateTime(2024, 07, 01))
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
                .Create();

            Action act = () =>
                RideHydrator.AttachCompetitors(new[] { ride }, snapshots, new[] { calendarEvent });

            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{ride.ClubNumber}*");
        }
    }
}