using ClubCore.Models;
using ClubSiteGenerator.Services.Hydration;
using FluentAssertions;

namespace ClubSiteGenerator.Tests
{
    public class RideHydrator_RoundRobinTests
    {
        private static RoundRobinRider Rider(string name, string club, bool isFemale = false) =>
            new RoundRobinRider
            {
                Name = name,
                RoundRobinClub = club,
                IsFemale = isFemale
            };

        private static Ride RideFor(RoundRobinRider rr, string? rrClubOverride = null) =>
            new Ride
            {
                Name = rr.DecoratedName,                 // must match decorated name
                ClubNumber = null,                       // RR only applies when ClubNumber is null
                RoundRobinClub = rrClubOverride ?? rr.RoundRobinClub
            };

        // ------------------------------------------------------------
        // Matching behaviour
        // ------------------------------------------------------------

        [Fact]
        public void AttachRoundRobinRiders_ShouldAttachMatchingRider()
        {
            var rr = Rider("Alice Smith", "Ratae", true);
            var ride = RideFor(rr);

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().Be(rr);
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldMatchCaseInsensitively()
        {
            var rr = Rider("ALICE SMITH", "RATAE");
            var ride = new Ride
            {
                Name = "alice smith (ratae)",   // lower case
                ClubNumber = null,
                RoundRobinClub = "Ratae"
            };

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().Be(rr);
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldMatchTrimmedNames()
        {
            var rr = Rider("Alice Smith", "Ratae");
            var ride = new Ride
            {
                Name = "   Alice Smith (Ratae)   ",
                ClubNumber = null,
                RoundRobinClub = "Ratae"
            };

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().Be(rr);
        }

        // ------------------------------------------------------------
        // Skipping behaviour
        // ------------------------------------------------------------

        [Fact]
        public void AttachRoundRobinRiders_ShouldIgnoreRidesWithClubNumber()
        {
            var rr = Rider("Alice Smith", "Ratae");

            var ride = new Ride
            {
                Name = rr.DecoratedName,
                ClubNumber = 123,          // skip
                RoundRobinClub = "Ratae"
            };

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().BeNull();
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldIgnoreRidesWithEmptyRoundRobinClub()
        {
            var rr = Rider("Alice Smith", "Ratae");

            var ride = new Ride
            {
                Name = rr.DecoratedName,
                ClubNumber = null,
                RoundRobinClub = ""        // skip
            };

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().BeNull();
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldIgnoreWVCC()
        {
            var rr = Rider("Alice Smith", "Ratae");

            var ride = new Ride
            {
                Name = rr.DecoratedName,
                ClubNumber = null,
                RoundRobinClub = "WVCC"    // skip
            };

            RideHydrator.AttachRoundRobinRiders(new[] { ride }, new[] { rr });

            ride.RoundRobinRider.Should().BeNull();
        }

        // ------------------------------------------------------------
        // Error behaviour
        // ------------------------------------------------------------

        [Fact]
        public void AttachRoundRobinRiders_ShouldThrowIfRideNameIsNull()
        {
            var ride = new Ride
            {
                Name = null,
                ClubNumber = null,
                RoundRobinClub = "Ratae"
            };

            Action act = () =>
                RideHydrator.AttachRoundRobinRiders(new[] { ride }, Array.Empty<RoundRobinRider>());

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*RoundRobin*Name*");
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldThrowIfNoMatchingRider()
        {
            var ride = new Ride
            {
                Name = "Bob Jones (Ratae)",
                ClubNumber = null,
                RoundRobinClub = "Ratae"
            };

            Action act = () =>
                RideHydrator.AttachRoundRobinRiders(new[] { ride }, Array.Empty<RoundRobinRider>());

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Bob Jones*");
        }

        [Fact]
        public void AttachRoundRobinRiders_ShouldListAllMissingRiders()
        {
            var rides = new[]
            {
                new Ride { Name = "Alice (Ratae)", ClubNumber = null, RoundRobinClub = "Ratae" },
                new Ride { Name = "Bob (Ratae)",   ClubNumber = null, RoundRobinClub = "Ratae" }
            };

            Action act = () =>
                RideHydrator.AttachRoundRobinRiders(rides, Array.Empty<RoundRobinRider>());

            var ex = act.Should().Throw<InvalidOperationException>().Which;

            ex.Message.Should().Contain("Alice");
            ex.Message.Should().Contain("Bob");
        }
    }
}