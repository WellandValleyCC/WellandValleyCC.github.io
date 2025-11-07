using AutoFixture.Xunit2;
using ClubProcessor.Calculators;
using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Orchestration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using System.Text;

namespace EventProcessor.Tests
{
    public class CompetitionPointsCalculatorTests
    {
        private static void AssertJuvenileRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.JuvenilesPosition.Should().Be(expected.Position, $"expected JuvenilesPosition {expected.Position} for {context}");
            ride.JuvenilesPoints.Should().Be(expected.Points, $"expected JuvenilesPoints {expected.Points} for {context}");
        }

        private static void AssertJuniorRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.JuniorsPosition.Should().Be(expected.Position, $"expected JuniorsPosition {expected.Position} for {context}");
            ride.JuniorsPoints.Should().Be(expected.Points, $"expected JuniorsPoints {expected.Points} for {context}");
        }


        void AssertSeniorRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.SeniorsPosition.Should().Be(expected.Position, $"expected SeniorsPosition {expected.Position} for {context}");
            ride.SeniorsPoints.Should().Be(expected.Points, $"expected SeniorsPoints {expected.Points} for {context}");
        }

        void AssertVeteranRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.VeteransPosition.Should().Be(expected.Position, $"expected VeteransPosition {expected.Position} for {context}");
            ride.VeteransPoints.Should().Be(expected.Points,     $"expected VeteransPoints {expected.Points} for {context}");
        }

        private static void AssertWomenRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.WomenPosition.Should().Be(expected.Position, $"expected WomenPosition {expected.Position} for {context}");
            ride.WomenPoints.Should().Be(expected.Points, $"expected WomenPoints {expected.Points} for {context}");
        }

        private static void AssertRoadBikeMenRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.RoadBikeMenPosition.Should().Be(expected.Position, $"expected RoadBikeMenPosition {expected.Position} for {context}");
            ride.RoadBikeMenPoints.Should().Be(expected.Points, $"expected RoadBikeMenPoints {expected.Points} for {context}");
        }

        private static void AssertRoadBikeWomenRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.RoadBikeWomenPosition.Should().Be(expected.Position, $"expected RoadBikeWomenPosition {expected.Position} for {context}");
            ride.RoadBikeWomenPoints.Should().Be(expected.Points, $"expected RoadBikeWomenPoints {expected.Points} for {context}");
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_RanksJuvenileRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());

            var competitorsByClubNumber = competitors.ToDictionary(c => c.ClubNumber);

            var allEventRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var expectedEvent1 = new[]
            {
                // Event 1 actual juvenile results:
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60.0), // 890s FirstClaim IsJuvenile
                (ClubNumber: 1001, Name: "Mia Bates", Position: 2, Points: 53), // 900s FirstClaim IsJuvenile
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 53), // 900s Honorary IsJuvenile
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 4, Points: 48), // 915s Honorary IsJuvenile
                (ClubNumber: 1002, Name: "Isla Carson", Position: 5, Points: 46), // 920s FirstClaim IsJuvenile
             };

            var expectedEvent2 = new[]
            {
                // Event 2 actual juvenile results:
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60.0), // 915s Honorary IsJuvenile
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55), // 930s Honorary IsJuvenile
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 51), // 940s FirstClaim IsJuvenile
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 4, Points: 48), // 955s FirstClaim IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 5, Points: 46), // 970s FirstClaim IsJuvenile
            };

            var expectedEvent3 = new[]
            {
                // Event 3 actual juvenile results:
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60.0), // 925s Honorary IsJuvenile
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55), // 940s Honorary IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51), // 980s FirstClaim IsJuvenile
            };

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Arrange helper structures
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(allRides);

            // Optional debug output (set breakpoint here or write to ITestOutputHelper)
            var debug = TestHelpers.RenderJuvenileDebugOutput(allRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // place breakpoint here to inspect

            // Assert via helper (use ridesByEvent, not allEventRides)
            void AssertExpectedForEventSimple(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    CompetitionPointsCalculatorTests.AssertJuvenileRideMatchesExpected(ridesForEvent, exp);
            }

            // Assert for events 1..3
            AssertExpectedForEventSimple(1, expectedEvent1);
            AssertExpectedForEventSimple(2, expectedEvent2);
            AssertExpectedForEventSimple(3, expectedEvent3);
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_ConsidersCompetitorClaimStatusHistoryUsingTestCompetitors(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            // Start with the stable juveniles from TestCompetitors.All
            var baseCompetitors = TestCompetitors.All.ToList();

            // Create future versions for two juvenile club numbers drawn from TestCompetitors.All
            // (choose two known juvenile club numbers from the file, e.g. 1001 and 1002)
            var miaBatesFutures = CompetitorFactory.CreateFutureVersions(
                baseCompetitors.GetByClubNumber(1001),
                snapshots: 3,
                interval: TimeSpan.FromDays(60));

            var islaCarsonFutures = CompetitorFactory.CreateFutureVersions(
                baseCompetitors.GetByClubNumber(1002),
                snapshots: 3,
                interval: TimeSpan.FromDays(60));

            var oscarEdwardsFutures = CompetitorFactory.CreateFutureVersions(
                baseCompetitors.GetByClubNumber(2012),
                snapshots: 10,
                interval: TimeSpan.FromDays(30));

            // Combine stable competitors with the futures (futures are additional rows for same ClubNumber)
            var competitors = baseCompetitors
                .Concat(miaBatesFutures)
                .Concat(islaCarsonFutures)
                .Concat(oscarEdwardsFutures)
                .ToList();

            // Scorer setup
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());


            // Build Competitor snapshots grouped by club number for resolution by ride date
            var competitorsByClubNumber = competitors
                .Where(c => c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.CreatedUtc).ToList());

            // Select event rides where the rider exists in our snapshots
            var allEventRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var expectedEvent1 = new[]
            {
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60.0), // 890s FirstClaim IsJuvenile
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 55), // 900s Honorary IsJuvenile
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 3, Points: 51), // 915s Honorary IsJuvenile
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60.0), // 915s Honorary IsJuvenile
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55), // 930s Honorary IsJuvenile
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 49.5), // 940s FirstClaim IsJuvenile    Order is: 60,55,51,48,46,44,42,40,39,38,37,36,35
                (ClubNumber: 2012, Name: "Oscar Edwards", Position: 3, Points: 49.5), // 940s FirstClaim IsJuvenile   51 + 48 = 99 points.  Shared between two, is 49.5 each.
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 5, Points: 46), // 955s FirstClaim IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 6, Points: 44), // 970s FirstClaim IsJuvenile
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60.0), // 925s Honorary IsJuvenile
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55), // 940s Honorary IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51), // 980s FirstClaim IsJuvenile
            };

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Arrange helper structures
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(allRides);

            // Optional debug output (this render uses the same resolution rules as the assertions)
            var debug = TestHelpers.RenderJuvenileDebugOutput(allRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // breakpoint-friendly

            // Assert using the same assertion helper
            void AssertExpectedForEventWithHistory(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    CompetitionPointsCalculatorTests.AssertJuvenileRideMatchesExpected(ridesForEvent, exp);
            }

            // Assert for events 1..3
            AssertExpectedForEventWithHistory(1, expectedEvent1);
            AssertExpectedForEventWithHistory(2, expectedEvent2);
            AssertExpectedForEventWithHistory(3, expectedEvent3);
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForAllAgeRanges_ThrowsIfCompetitorNotCreateForRideDate(
            List<CalendarEvent> calendar)
        {
            // Arrange
            const int eventNumber = 2;
            var baseCompetitors = TestCompetitors.All.ToList();
            var baseRides =TestRides.All.ToList();

            // Inject future-dated competitors for club numbers present in Event 2
            // These will have CreatedUtc > EventDate, so resolution should fail
            var futureCompetitors = new List<Competitor>
            {
                CompetitorFactory.Create(4001, "Harper", "Sylvie", ClaimStatus.FirstClaim, true, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4002, "Cross", "Damon", ClaimStatus.FirstClaim, false, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4003, "Langford", "Tessa", ClaimStatus.FirstClaim, true, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4004, "Blake", "Ronan", ClaimStatus.FirstClaim, false, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4005, "Frost", "Imogen", ClaimStatus.FirstClaim, true, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4006, "Drake", "Callum", ClaimStatus.FirstClaim, false, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4007, "Winslow", "Freya", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, DateTime.UtcNow.AddDays(50),10),
                CompetitorFactory.Create(4008, "Thorne", "Jasper", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, DateTime.UtcNow.AddDays(50),1),
            };

            var competitors = baseCompetitors.Concat(futureCompetitors).ToList();

            // Some rides for Competitors which have not been created yet (Competitor row is dated in future).
            // This is not a valid situation.  These riders should cause
            // scoring to throw InvalidOperationException("Scoring aborted: missing competitors detected. Please check the membership list.");
            var ridesUsingFutureCompetitors = new List<Ride>
            {
                RideFactory.CreateClubMemberRide(2, 4001, "Sylvie Harper", totalSeconds: 910),  // Juv F FirstClaim
                RideFactory.CreateClubMemberRide(2, 4002, "Damon Cross", totalSeconds: 905),    // Juv M FirstClaim
                RideFactory.CreateClubMemberRide(2, 4003, "Tessa Langford", totalSeconds: 930), // Jun F FirstClaim
                RideFactory.CreateClubMemberRide(2, 4004, "Ronan Blake", totalSeconds: 920),    // Jun M FirstClaim
                RideFactory.CreateClubMemberRide(2, 4005, "Imogen Frost", totalSeconds: 880),   // Sen F FirstClaim
                RideFactory.CreateClubMemberRide(2, 4006, "Callum Drake", totalSeconds: 870),   // Sen M FirstClaim
                RideFactory.CreateClubMemberRide(2, 4007, "Freya Winslow", totalSeconds: 990),  // Vet F FirstClaim
                RideFactory.CreateClubMemberRide(2, 4008, "Jasper Thorne", totalSeconds: 975)   // Vet M FirstClaim
            };

            var rides = baseRides.Concat(ridesUsingFutureCompetitors).ToList();

            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());

            // Build snapshot dictionary
            var competitorsByClubNumber = competitors
                .Where(c => c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.CreatedUtc).ToList());

            // Filter rides for Event 2
            var eventRides = rides
                .Where(r => r.EventNumber == eventNumber && r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue)
                .ToList();

            // Act & Assert
            var act = () => scorer.ProcessAll(rides, competitors, calendar);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Scoring aborted: missing competitors detected. Please check the membership list.");
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForSeniors_RanksAllEligibleRidersNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());

            // Competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Build grouping and debug output
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderSeniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // breakpoint-friendly

            // Expected data for Seniors competition
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn", Position: 1, Points: 60), // 830s FirstClaim IsSenior
                (ClubNumber: 1052, Name: "Thomas Reid", Position: 2, Points: 55), // 845s FirstClaim IsSenior
                (ClubNumber: 1041, Name: "Charlotte Nash", Position: 3, Points: 51), // 850s FirstClaim IsSenior
                (ClubNumber: 1042, Name: "Emily Owens", Position: 4, Points: 48), // 860s FirstClaim IsSenior
                (ClubNumber: 3051, Name: "Aaron Quincy", Position: 5, Points: 46), // 865s Honorary IsSenior
                (ClubNumber: 3041, Name: "Ella Norris", Position: 6, Points: 44), // 875s Honorary IsSenior
                (ClubNumber: 1031, Name: "Oliver King", Position: 7, Points: 42), // 880s FirstClaim IsJunior
                (ClubNumber: 1011, Name: "Liam Evans", Position: 8, Points: 39.5), // 890s FirstClaim IsJuvenile
                (ClubNumber: 3031, Name: "Reece Kirk", Position: 8, Points: 39.5), // 890s Honorary IsJunior
                (ClubNumber: 1032, Name: "Harry Lewis", Position: 10, Points: 38), // 895s FirstClaim IsJunior
                (ClubNumber: 1001, Name: "Mia Bates", Position: 11, Points: 36.5), // 900s FirstClaim IsJuvenile
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 11, Points: 36.5), // 900s Honorary IsJuvenile
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 13, Points: 35), // 915s Honorary IsJuvenile
                (ClubNumber: 1002, Name: "Isla Carson", Position: 14, Points: 34), // 920s FirstClaim IsJuvenile
                (ClubNumber: 1071, Name: "Peter Walker", Position: 15, Points: 33), // 930s FirstClaim IsVeteran
                (ClubNumber: 1021, Name: "Amelia Hughes", Position: 16, Points: 31.5), // 940s FirstClaim IsJunior
                (ClubNumber: 3021, Name: "Zara Hayes", Position: 16, Points: 31.5), // 940s Honorary IsJunior
                (ClubNumber: 3071, Name: "Graham Watson", Position: 18, Points: 30), // 970s Honorary IsVeteran
                (ClubNumber: 3061, Name: "Diana Thompson", Position: 19, Points: 29), // 985s Honorary IsVeteran
                (ClubNumber: 1062, Name: "Alison Underwood", Position: 20, Points: 28), // 995s FirstClaim IsVeteran
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price", Position: 1, Points: 60), // 840s FirstClaim IsSenior
                (ClubNumber: 3052, Name: "Charlie Robinson", Position: 2, Points: 55), // 845s Honorary IsSenior
                (ClubNumber: 1053, Name: "Daniel Shaw", Position: 3, Points: 49.5), // 855s FirstClaim IsSenior
                (ClubNumber: 3042, Name: "Katie Olsen", Position: 3, Points: 49.5), // 855s Honorary IsSenior
                (ClubNumber: 3032, Name: "Tyler Lloyd", Position: 5, Points: 46), // 895s Honorary IsJunior
                (ClubNumber: 1022, Name: "Sophie Irwin", Position: 6, Points: 44), // 900s FirstClaim IsJunior
                (ClubNumber: 1023, Name: "Grace Jackson", Position: 7, Points: 41), // 915s FirstClaim IsJunior
                (ClubNumber: 3012, Name: "Max Franklin", Position: 7, Points: 41), // 915s Honorary IsJuvenile
                (ClubNumber: 3022, Name: "Megan Irving", Position: 9, Points: 39), // 925s Honorary IsJunior
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 10, Points: 38), // 930s Honorary IsJuvenile
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 11, Points: 37), // 940s FirstClaim IsJuvenile
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 12, Points: 36), // 955s FirstClaim IsJuvenile
                (ClubNumber: 1072, Name: "Martin Xavier", Position: 13, Points: 35), // 965s FirstClaim IsVeteran
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 14, Points: 34), // 970s FirstClaim IsJuvenile
                (ClubNumber: 3072, Name: "Trevor York", Position: 15, Points: 33), // 975s Honorary IsVeteran
                (ClubNumber: 1061, Name: "Helen Turner", Position: 16, Points: 32), // 980s FirstClaim IsVeteran
                (ClubNumber: 1062, Name: "Alison Underwood", Position: 17, Points: 30.5), // 995s FirstClaim IsVeteran
                (ClubNumber: 3062, Name: "Fiona Ursula", Position: 17, Points: 30.5), // 995s Honorary IsVeteran
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3053, Name: "Joseph Stevens", Position: 1, Points: 60.0), // 855s Honorary IsSenior
                (ClubNumber: 1043, Name: "Lucy Price", Position: 2, Points: 55), // 860s FirstClaim IsSenior
                (ClubNumber: 3043, Name: "Georgia Parker", Position: 3, Points: 51), // 865s Honorary IsSenior
                (ClubNumber: 1041, Name: "Charlotte Nash", Position: 4, Points: 48), // 870s FirstClaim IsSenior
                (ClubNumber: 3033, Name: "Ryan Mitchell", Position: 5, Points: 46), // 890s Honorary IsJunior
                (ClubNumber: 1031, Name: "Oliver King", Position: 6, Points: 44), // 905s FirstClaim IsJunior
                (ClubNumber: 1033, Name: "Jack Mason", Position: 7, Points: 42), // 910s FirstClaim IsJunior
                (ClubNumber: 3023, Name: "Amber Jennings", Position: 8, Points: 40), // 920s Honorary IsJunior
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 9, Points: 39), // 925s Honorary IsJuvenile
                (ClubNumber: 3003, Name: "Leah Davies", Position: 10, Points: 38), // 940s Honorary IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 11, Points: 37), // 980s FirstClaim IsJuvenile
                (ClubNumber: 1071, Name: "Peter Walker", Position: 12, Points: 36), // 990s FirstClaim IsVeteran
                (ClubNumber: 3073, Name: "Derek Zimmer", Position: 13, Points: 35), // 1000s Honorary IsVeteran
                (ClubNumber: 1073, Name: "Colin Young", Position: 14, Points: 34), // 1005s FirstClaim IsVeteran
                (ClubNumber: 1063, Name: "Janet Vaughn", Position: 15, Points: 33), // 1020s FirstClaim IsVeteran
                (ClubNumber: 3063, Name: "Paula Valentine", Position: 16, Points: 32), // 1025s Honorary IsVeteran
            };

            // Local assert runner
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertSeniorRideMatchesExpected(ridesForEvent, exp);
            }

            // Assert for events 1..3
            AssertExpectedForEvent(1, expectedEvent1);
            AssertExpectedForEvent(2, expectedEvent2);
            AssertExpectedForEvent(3, expectedEvent3);
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForSeniors_ConsidersCompetitorClaimStatusHistory(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var baseCompetitors = TestCompetitors.All.ToList();

            // create some future/past snapshots for a few club numbers as needed for the test
            var futuresA = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1051), snapshots: 3, interval: TimeSpan.FromDays(30));
            var futuresB = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1041), snapshots: 2, interval: TimeSpan.FromDays(60));
            var futuresC = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1062), snapshots: 1, interval: TimeSpan.FromDays(90));

            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());

            var competitors = baseCompetitors.Concat(futuresA).Concat(futuresB).Concat(futuresC).ToList();

            // Build competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderSeniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            // Expected data for Seniors competition
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn", Position: 1, Points: 60.0), // 830s FirstClaim IsSenior
                (ClubNumber: 1052, Name: "Thomas Reid", Position: 2, Points: 55), // 845s FirstClaim IsSenior
                // excluded by Futures changing ClaimStatus         (ClubNumber: 1041, Name: "Charlotte Nash",  Position:  3, Points: 51),// time: 850
                (ClubNumber: 1042, Name: "Emily Owens", Position: 3, Points: 51), // 860s FirstClaim IsSenior
                (ClubNumber: 3051, Name: "Aaron Quincy", Position: 4, Points: 48), // 865s Honorary IsSenior
                (ClubNumber: 3041, Name: "Ella Norris", Position: 5, Points: 46), // 875s Honorary IsSenior
                (ClubNumber: 1031, Name: "Oliver King", Position: 6, Points: 44), // 880s FirstClaim IsJunior
                (ClubNumber: 1011, Name: "Liam Evans", Position: 7, Points: 41), // 890s FirstClaim IsJuvenile
                (ClubNumber: 3031, Name: "Reece Kirk", Position: 7, Points: 41), // 890s Honorary IsJunior
                (ClubNumber: 1032, Name: "Harry Lewis", Position: 9, Points: 39), // 895s FirstClaim IsJunior
                (ClubNumber: 1001, Name: "Mia Bates", Position: 10, Points: 37.5), // 900s FirstClaim IsJuvenile
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 10, Points: 37.5), // 900s Honorary IsJuvenile
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 12, Points: 36), // 915s Honorary IsJuvenile
                (ClubNumber: 1002, Name: "Isla Carson", Position: 13, Points: 35), // 920s FirstClaim IsJuvenile
                (ClubNumber: 1071, Name: "Peter Walker", Position: 14, Points: 34), // 930s FirstClaim IsVeteran
                (ClubNumber: 1021, Name: "Amelia Hughes", Position: 15, Points: 32.5), // 940s FirstClaim IsJunior
                (ClubNumber: 3021, Name: "Zara Hayes", Position: 15, Points: 32.5), // 940s Honorary IsJunior
                (ClubNumber: 3071, Name: "Graham Watson", Position: 17, Points: 31), // 970s Honorary IsVeteran
                (ClubNumber: 3061, Name: "Diana Thompson", Position: 18, Points: 30), // 985s Honorary IsVeteran
                (ClubNumber: 1062, Name: "Alison Underwood", Position: 19, Points: 29), // 995s FirstClaim IsVeteran
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price", Position: 1, Points: 60), // 840s FirstClaim IsSenior
                (ClubNumber: 3052, Name: "Charlie Robinson", Position: 2, Points: 55), // 845s Honorary IsSenior
                (ClubNumber: 1053, Name: "Daniel Shaw", Position: 3, Points: 49.5), // 855s FirstClaim IsSenior
                (ClubNumber: 3042, Name: "Katie Olsen", Position: 3, Points: 49.5), // 855s Honorary IsSenior
                (ClubNumber: 3032, Name: "Tyler Lloyd", Position: 5, Points: 46), // 895s Honorary IsJunior
                (ClubNumber: 1022, Name: "Sophie Irwin", Position: 6, Points: 44), // 900s FirstClaim IsJunior
                (ClubNumber: 1023, Name: "Grace Jackson", Position: 7, Points: 41), // 915s FirstClaim IsJunior
                (ClubNumber: 3012, Name: "Max Franklin", Position: 7, Points: 41), // 915s Honorary IsJuvenile
                (ClubNumber: 3022, Name: "Megan Irving", Position: 9, Points: 39), // 925s Honorary IsJunior
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 10, Points: 38), // 930s Honorary IsJuvenile
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 11, Points: 37), // 940s FirstClaim IsJuvenile
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 12, Points: 36), // 955s FirstClaim IsJuvenile
                (ClubNumber: 1072, Name: "Martin Xavier", Position: 13, Points: 35), // 965s FirstClaim IsVeteran
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 14, Points: 34), // 970s FirstClaim IsJuvenile
                (ClubNumber: 3072, Name: "Trevor York", Position: 15, Points: 33), // 975s Honorary IsVeteran
                (ClubNumber: 1061, Name: "Helen Turner", Position: 16, Points: 32), // 980s FirstClaim IsVeteran
                // excluded by Futures changing ClaimStatus        (ClubNumber: 1062, Name: "Alison Underwood",   Position: 17, Points: 30.5), // time: 995
                (ClubNumber: 3062, Name: "Fiona Ursula", Position: 17, Points: 31), // 995s Honorary IsVeteran
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3053, Name: "Joseph Stevens", Position: 1, Points: 60.0), // 855s Honorary IsSenior
                (ClubNumber: 1043, Name: "Lucy Price", Position: 2, Points: 55), // 860s FirstClaim IsSenior
                (ClubNumber: 3043, Name: "Georgia Parker", Position: 3, Points: 51), // 865s Honorary IsSenior
                (ClubNumber: 1041, Name: "Charlotte Nash", Position: 4, Points: 48), // 870s FirstClaim IsSenior
                (ClubNumber: 3033, Name: "Ryan Mitchell", Position: 5, Points: 46), // 890s Honorary IsJunior
                (ClubNumber: 1031, Name: "Oliver King", Position: 6, Points: 44), // 905s FirstClaim IsJunior
                (ClubNumber: 1033, Name: "Jack Mason", Position: 7, Points: 42), // 910s FirstClaim IsJunior
                (ClubNumber: 3023, Name: "Amber Jennings", Position: 8, Points: 40), // 920s Honorary IsJunior
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 9, Points: 39), // 925s Honorary IsJuvenile
                (ClubNumber: 3003, Name: "Leah Davies", Position: 10, Points: 38), // 940s Honorary IsJuvenile
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 11, Points: 37), // 980s FirstClaim IsJuvenile
                (ClubNumber: 1071, Name: "Peter Walker", Position: 12, Points: 36), // 990s FirstClaim IsVeteran
                (ClubNumber: 3073, Name: "Derek Zimmer", Position: 13, Points: 35), // 1000s Honorary IsVeteran
                (ClubNumber: 1073, Name: "Colin Young", Position: 14, Points: 34), // 1005s FirstClaim IsVeteran
                (ClubNumber: 1063, Name: "Janet Vaughn", Position: 15, Points: 33), // 1020s FirstClaim IsVeteran
                (ClubNumber: 3063, Name: "Paula Valentine", Position: 16, Points: 32), // 1025s Honorary IsVeteran
            };

            // Local runner
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertSeniorRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, expectedEvent1);
            AssertExpectedForEvent(2, expectedEvent2);
            AssertExpectedForEvent(3, expectedEvent3);
        }

        [Theory]
        [EventAutoData]
        public void EventRank_IsAssignedAccordingToTotalTime(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Include all eligible rides (club members and guests)
            var validRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Group by event using the existing test helper
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: false);

            // Assert overall EventRank for each event using "first position" tie semantics
            foreach (var kv in ridesByEvent)
            {
                var evtNumber = kv.Key;
                var ridesForEvent = kv.Value;
                if (!ridesForEvent.Any()) continue;

                var ordered = ridesForEvent
                    .OrderBy(r => r.TotalSeconds)
                    .ThenBy(r => r.Name) // deterministic iteration only
                    .ToArray();

                int lastRank = 0;
                double? lastTime = null;
                for (int i = 0; i < ordered.Length; i++)
                {
                    var ride = ordered[i];
                    var time = ride.TotalSeconds;
                    int expectedRank;
                    if (i == 0) expectedRank = 1;
                    else if (Nullable.Equals(time, lastTime)) expectedRank = lastRank;
                    else expectedRank = i + 1;

                    ride.EventRank.Should().Be(expectedRank, because: $"overall position for Event {evtNumber} rider {ride.Name}");

                    lastRank = expectedRank;
                    lastTime = time;
                }
            }
        }

        [Theory]
        [EventAutoData]
        public void EventRoadBikeRank_IsAssignedAmongRoadBikeRidersOnly(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Two groupings:
            // - all eligible rides (for checking "no road riders" behaviour)
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: false);
            // - only eligible road-bike rides (for rank computation)
            var roadRidesByEvent = TestHelpers.BuildRidesByEvent(validRides.Where(r => r.IsRoadBike).ToList(), onlyValidWithClubNumber: false);

            // Assert EventRoadBikeRank computed only from road rides using same tie semantics
            foreach (var kv in ridesByEvent)
            {
                var evtNumber = kv.Key;
                var ridesForEvent = kv.Value;

                var roadOrdered = roadRidesByEvent.TryGetValue(evtNumber, out var rr) && rr.Any()
                    ? rr.OrderBy(r => r.TotalSeconds).ThenBy(r => r.Name).ToArray()
                    : Array.Empty<Ride>();

                if (roadOrdered.Length == 0)
                {
                    // no road-bike riders — expect EventRoadBikeRank not assigned (null) for all rides in event
                    foreach (var r in ridesForEvent)
                        r.EventRoadBikeRank.Should().BeNull(because: $"no road-bike riders in event {evtNumber}");
                    continue;
                }

                int lastRank = 0;
                double? lastTime = null;
                for (int i = 0; i < roadOrdered.Length; i++)
                {
                    var ride = roadOrdered[i];
                    var time = ride.TotalSeconds;
                    int expectedRoadRank;
                    if (i == 0) expectedRoadRank = 1;
                    else if (Nullable.Equals(time, lastTime)) expectedRoadRank = lastRank;
                    else expectedRoadRank = i + 1;

                    ride.EventRoadBikeRank.Should().Be(expectedRoadRank, because: $"road-bike position for Event {evtNumber} rider {ride.Name}");

                    lastRank = expectedRoadRank;
                    lastTime = time;
                }

                // Optionally assert that non-road rides have no EventRoadBikeRank set
                foreach (var nonRoad in ridesForEvent.Where(r => !r.IsRoadBike))
                    nonRoad.EventRoadBikeRank.Should().BeNull(because: $"non-road rider should not receive EventRoadBikeRank in event {evtNumber}");
            }
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_RoadBikeRank_only_counts_roadbike_riders_and_matches_EventRank_where_applicable(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: false);

            foreach (var ridesForEvent in ridesByEvent.Values)
            {
                var roadRides = ridesForEvent.Where(r => r.IsRoadBike).ToList();
                if (!roadRides.Any()) continue;

                // Order by EventRank, which should already be set by ProcessAll and reflect the same tie rule
                var orderedByEventRank = roadRides
                    .OrderBy(r => r.EventRank ?? int.MaxValue)
                    .ThenBy(r => r.Name)
                    .ToArray();

                // Compute expected sequential road ranks using EventRank ordering but preserving tie semantics:
                int lastRank = 0;
                int? lastEventRankValue = null;
                for (int i = 0; i < orderedByEventRank.Length; i++)
                {
                    var ride = orderedByEventRank[i];
                    var eventRankVal = ride.EventRank ?? int.MaxValue;
                    int expectedRoadRank;
                    if (i == 0)
                    {
                        expectedRoadRank = 1;
                    }
                    else if (lastEventRankValue == eventRankVal)
                    {
                        expectedRoadRank = lastRank;
                    }
                    else
                    {
                        expectedRoadRank = i + 1;
                    }

                    ride.EventRoadBikeRank.Should().Be(expectedRoadRank,
                        because: "EventRoadBikeRank should reflect sequential position among road-bike riders using the same overall order");

                    lastRank = expectedRoadRank;
                    lastEventRankValue = eventRankVal;
                }
            }
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuniors_RanksJuniorRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderJuniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertJuniorRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 1031, Name: "Oliver King", Position: 1, Points: 60.0), // 880s FirstClaim IsJunior
                (ClubNumber: 3031, Name: "Reece Kirk", Position: 2, Points: 55), // 890s Honorary IsJunior
                (ClubNumber: 1032, Name: "Harry Lewis", Position: 3, Points: 51), // 895s FirstClaim IsJunior
                (ClubNumber: 1021, Name: "Amelia Hughes", Position: 4, Points: 47), // 940s FirstClaim IsJunior
                (ClubNumber: 3021, Name: "Zara Hayes", Position: 4, Points: 47), // 940s Honorary IsJunior
            });

            AssertExpectedForEvent(2, new[]
            {
                (ClubNumber: 3032, Name: "Tyler Lloyd", Position: 1, Points: 60.0), // 895s Honorary IsJunior
                (ClubNumber: 1022, Name: "Sophie Irwin", Position: 2, Points: 55), // 900s FirstClaim IsJunior
                (ClubNumber: 1023, Name: "Grace Jackson", Position: 3, Points: 51), // 915s FirstClaim IsJunior
                (ClubNumber: 3022, Name: "Megan Irving", Position: 4, Points: 48), // 925s Honorary IsJunior
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 3033, Name: "Ryan Mitchell", Position: 1, Points: 60.0), // 890s Honorary IsJunior
                (ClubNumber: 1031, Name: "Oliver King", Position: 2, Points: 55), // 905s FirstClaim IsJunior
                (ClubNumber: 1033, Name: "Jack Mason", Position: 3, Points: 51), // 910s FirstClaim IsJunior
                (ClubNumber: 3023, Name: "Amber Jennings", Position: 4, Points: 48), // 920s Honorary IsJunior
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuniors_ConsidersCompetitorClaimStatusHistory(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var baseCompetitors = TestCompetitors.All.ToList();
            var futures = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1031), snapshots: 2, interval: TimeSpan.FromDays(45));
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderJuniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertJuniorRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 3031, Name: "Reece Kirk", Position: 1, Points: 60.0), // 890s Honorary IsJunior
                (ClubNumber: 1032, Name: "Harry Lewis", Position: 2, Points: 55), // 895s FirstClaim IsJunior
                (ClubNumber: 1021, Name: "Amelia Hughes", Position: 3, Points: 49.5), // 940s FirstClaim IsJunior
                (ClubNumber: 3021, Name: "Zara Hayes", Position: 3, Points: 49.5), // 940s Honorary IsJunior
            });

            AssertExpectedForEvent(2, new[]    
            {
                (ClubNumber: 3032, Name: "Tyler Lloyd", Position: 1, Points: 60.0), // 895s Honorary IsJunior
                (ClubNumber: 1022, Name: "Sophie Irwin", Position: 2, Points: 55), // 900s FirstClaim IsJunior
                (ClubNumber: 1023, Name: "Grace Jackson", Position: 3, Points: 51), // 915s FirstClaim IsJunior
                (ClubNumber: 3022, Name: "Megan Irving", Position: 4, Points: 48), // 925s Honorary IsJunior
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 3033, Name: "Ryan Mitchell", Position: 1, Points: 60.0), // 890s Honorary IsJunior
                (ClubNumber: 1031, Name: "Oliver King", Position: 2, Points: 55), // 905s FirstClaim IsJunior
                (ClubNumber: 1033, Name: "Jack Mason", Position: 3, Points: 51), // 910s FirstClaim IsJunior
                (ClubNumber: 3023, Name: "Amber Jennings", Position: 4, Points: 48), // 920s Honorary IsJunior
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForWomen_RanksFemaleRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderWomenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertWomenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
                {
                    (ClubNumber: 1041, Name: "Charlotte Nash",   Position: 1, Points: 60.0), // 850s FirstClaim IsSenior
                    (ClubNumber: 1042, Name: "Emily Owens",      Position: 2, Points: 55), // 860s FirstClaim IsSenior
                    (ClubNumber: 3041, Name: "Ella Norris",      Position: 3, Points: 51), // 875s Honorary IsSenior
                    (ClubNumber: 1001, Name: "Mia Bates",        Position: 4, Points: 48), // 900s FirstClaim IsJuvenile
                    (ClubNumber: 3001, Name: "Tia Bennett",      Position: 5, Points: 46), // 915s Honorary IsJuvenile
                    (ClubNumber: 1002, Name: "Isla Carson",      Position: 6, Points: 44), // 920s FirstClaim IsJuvenile
                    (ClubNumber: 1021, Name: "Amelia Hughes",    Position: 7, Points: 41), // 940s FirstClaim IsJunior
                    (ClubNumber: 3021, Name: "Zara Hayes",       Position: 7, Points: 41), // 940s Honorary IsJunior
                    (ClubNumber: 3061, Name: "Diana Thompson",   Position: 9, Points: 39), // 985s Honorary IsVeteran
                    (ClubNumber: 1062, Name: "Alison Underwood", Position: 10, Points: 38) // 995s FirstClaim IsVeteran
                });

            AssertExpectedForEvent(2, new[]
                {
                    (ClubNumber: 1043, Name: "Lucy Price",       Position: 1, Points: 60.0), // 840s FirstClaim IsSenior
                    (ClubNumber: 3042, Name: "Katie Olsen",      Position: 2, Points: 55), // 855s Honorary IsSenior
                    (ClubNumber: 1022, Name: "Sophie Irwin",     Position: 3, Points: 51), // 900s FirstClaim IsJunior
                    (ClubNumber: 1023, Name: "Grace Jackson",    Position: 4, Points: 48), // 915s FirstClaim IsJunior
                    (ClubNumber: 3022, Name: "Megan Irving",     Position: 5, Points: 46), // 925s Honorary IsJunior
                    (ClubNumber: 3002, Name: "Nina Chapman",     Position: 6, Points: 44), // 930s Honorary IsJuvenile
                    (ClubNumber: 1003, Name: "Zoe Dennison",     Position: 7, Points: 42), // 940s FirstClaim IsJuvenile
                    (ClubNumber: 1061, Name: "Helen Turner",     Position: 8, Points: 40), // 980s FirstClaim IsVeteran
                    (ClubNumber: 1062, Name: "Alison Underwood", Position: 9, Points: 38.5), // 995s FirstClaim IsVeteran
                    (ClubNumber: 3062, Name: "Fiona Ursula",     Position: 9, Points: 38.5) // 995s Honorary IsVeteran
                });

            AssertExpectedForEvent(3, new[]
                {
                    (ClubNumber: 1043, Name: "Lucy Price",       Position: 1, Points: 60.0), // 860s FirstClaim IsSenior
                    (ClubNumber: 3043, Name: "Georgia Parker",   Position: 2, Points: 55), // 865s Honorary IsSenior
                    (ClubNumber: 1041, Name: "Charlotte Nash",   Position: 3, Points: 51), // 870s FirstClaim IsSenior
                    (ClubNumber: 3023, Name: "Amber Jennings",   Position: 4, Points: 48), // 920s Honorary IsJunior
                    (ClubNumber: 3003, Name: "Leah Davies",      Position: 5, Points: 46), // 940s Honorary IsJuvenile
                    (ClubNumber: 1063, Name: "Janet Vaughn",     Position: 6, Points: 44), // 1020s FirstClaim IsVeteran
                    (ClubNumber: 3063, Name: "Paula Valentine",  Position: 7, Points: 42) // 1025s Honorary IsVeteran
                });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForWomen_ConsidersCompetitorClaimStatusHistory(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var baseCompetitors = TestCompetitors.All.ToList();
            var futures = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1041), snapshots: 2, interval: TimeSpan.FromDays(45));
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderWomenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertWomenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
                {
// Excluded due to futures                    (ClubNumber: 1041, Name: "Charlotte Nash",   Position: 1, Points: 60.0), // 850s FirstClaim IsSenior
                    (ClubNumber: 1042, Name: "Emily Owens",      Position: 1, Points: 60.0),  // 860s FirstClaim IsSenior
                    (ClubNumber: 3041, Name: "Ella Norris",      Position: 2, Points: 55),    // 875s Honorary IsSenior
                    (ClubNumber: 1001, Name: "Mia Bates",        Position: 3, Points: 51),    // 900s FirstClaim IsJuvenile
                    (ClubNumber: 3001, Name: "Tia Bennett",      Position: 4, Points: 48),    // 915s Honorary IsJuvenile
                    (ClubNumber: 1002, Name: "Isla Carson",      Position: 5, Points: 46),    // 920s FirstClaim IsJuvenile
                    (ClubNumber: 1021, Name: "Amelia Hughes",    Position: 6, Points: 43),    // 940s FirstClaim IsJunior
                    (ClubNumber: 3021, Name: "Zara Hayes",       Position: 6, Points: 43),    // 940s Honorary IsJunior
                    (ClubNumber: 3061, Name: "Diana Thompson",   Position: 8, Points: 40),    // 985s Honorary IsVeteran
                    (ClubNumber: 1062, Name: "Alison Underwood", Position: 9, Points: 39),    // 995s FirstClaim IsVeteran
                });                                                                          

            AssertExpectedForEvent(2, new[]
                {
                    (ClubNumber: 1043, Name: "Lucy Price",       Position: 1, Points: 60.0), // 840s FirstClaim IsSenior
                    (ClubNumber: 3042, Name: "Katie Olsen",      Position: 2, Points: 55),   // 855s Honorary IsSenior
                    (ClubNumber: 1022, Name: "Sophie Irwin",     Position: 3, Points: 51),   // 900s FirstClaim IsJunior
                    (ClubNumber: 1023, Name: "Grace Jackson",    Position: 4, Points: 48),   // 915s FirstClaim IsJunior
                    (ClubNumber: 3022, Name: "Megan Irving",     Position: 5, Points: 46),   // 925s Honorary IsJunior
                    (ClubNumber: 3002, Name: "Nina Chapman",     Position: 6, Points: 44),   // 930s Honorary IsJuvenile
                    (ClubNumber: 1003, Name: "Zoe Dennison",     Position: 7, Points: 42),   // 940s FirstClaim IsJuvenile
                    (ClubNumber: 1061, Name: "Helen Turner",     Position: 8, Points: 40),   // 980s FirstClaim IsVeteran
                    (ClubNumber: 1062, Name: "Alison Underwood", Position: 9, Points: 38.5), // 995s FirstClaim IsVeteran
                    (ClubNumber: 3062, Name: "Fiona Ursula",     Position: 9, Points: 38.5)  // 995s Honorary IsVeteran
                });

            AssertExpectedForEvent(3, new[]
                {
                    (ClubNumber: 1043, Name: "Lucy Price",       Position: 1, Points: 60.0), // 860s FirstClaim IsSenior
                    (ClubNumber: 3043, Name: "Georgia Parker",   Position: 2, Points: 55),   // 865s Honorary IsSenior
/*Futures*/         (ClubNumber: 1041, Name: "Charlotte Nash",   Position: 3, Points: 51), // 870s FirstClaim IsSenior Female TT
                    (ClubNumber: 3023, Name: "Amber Jennings",   Position: 4, Points: 48),   // 920s Honorary IsJunior
                    (ClubNumber: 3003, Name: "Leah Davies",      Position: 5, Points: 46),   // 940s Honorary IsJuvenile
                    (ClubNumber: 1063, Name: "Janet Vaughn",     Position: 6, Points: 44),   // 1020s FirstClaim IsVeteran
                    (ClubNumber: 3063, Name: "Paula Valentine",  Position: 7, Points: 42)    // 1025s Honorary IsVeteran
                });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForRoadBikeMen_RanksMaleRidersOnRoadBikesWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderRoadBikeMenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertRoadBikeMenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 1051, Name: "James Quinn",        Position: 1, Points: 60.0), // 830s FirstClaim IsSenior Male Road
                (ClubNumber: 1031, Name: "Oliver King",        Position: 2, Points: 55), // 880s FirstClaim IsJunior Male Road
                (ClubNumber: 1011, Name: "Liam Evans",         Position: 3, Points: 51), // 890s FirstClaim IsJuvenile Male Road
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 4, Points: 48), // 930s FirstClaim IsVeteran Male Road
            });

            AssertExpectedForEvent(2, new[]
            {
                (ClubNumber: 1072, Name: "Martin Xavier",      Position: 1, Points: 60.0), // 965s FirstClaim IsVeteran Male Road
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 2, Points: 55), // 970s FirstClaim IsJuvenile Male Road
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 1031, Name: "Oliver King",        Position: 1, Points: 60.0), // 905s FirstClaim IsJunior Male Road
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 2, Points: 55), // 980s FirstClaim IsJuvenile Male Road
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 3, Points: 51), // 990s FirstClaim IsVeteran Male Road
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForRoadBikeMen_ConsidersCompetitorClaimStatusHistory(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var baseCompetitors = TestCompetitors.All.ToList();
            var futures = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1051), snapshots: 2, interval: TimeSpan.FromDays(45));
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderRoadBikeMenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertRoadBikeMenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 1031, Name: "Oliver King",  Position: 1, Points: 60.0), // 880s FirstClaim IsJunior Male Road
                (ClubNumber: 1011, Name: "Liam Evans",   Position: 2, Points: 55), // 890s FirstClaim IsJuvenile Male Road
                (ClubNumber: 1071, Name: "Peter Walker", Position: 3, Points: 51), // 930s FirstClaim IsVeteran Male Road
            });

            AssertExpectedForEvent(2, new[]
            {
                (ClubNumber: 1072, Name: "Martin Xavier",      Position: 1, Points: 60.0), // 965s FirstClaim IsVeteran Male Road
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 2, Points: 55), // 970s FirstClaim IsJuvenile Male Road
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 1031, Name: "Oliver King",        Position: 1, Points: 60.0), // 905s FirstClaim IsJunior Male Road
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 2, Points: 55), // 980s FirstClaim IsJuvenile Male Road
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 3, Points: 51), // 990s FirstClaim IsVeteran Male Road
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForRoadBikeWomen_RanksFemaleRidersOnRoadBikesWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderRoadBikeWomenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertRoadBikeWomenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 1041, Name: "Charlotte Nash",     Position: 1, Points: 60.0), // 850s FirstClaim IsSenior Female Road
                (ClubNumber: 3041, Name: "Ella Norris",        Position: 2, Points: 55), // 875s Honorary IsSenior Female Road
                (ClubNumber: 1001, Name: "Mia Bates",          Position: 3, Points: 51), // 900s FirstClaim IsJuvenile Female Road
                (ClubNumber: 3001, Name: "Tia Bennett",        Position: 4, Points: 48), // 915s Honorary IsJuvenile Female Road
                (ClubNumber: 3021, Name: "Zara Hayes",         Position: 5, Points: 46), // 940s Honorary IsJunior Female Road
                (ClubNumber: 3061, Name: "Diana Thompson",     Position: 6, Points: 44), // 985s Honorary IsVeteran Female Road
            });

            AssertExpectedForEvent(2, new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position: 1, Points: 60.0), // 840s FirstClaim IsSenior Female Road
                (ClubNumber: 3042, Name: "Katie Olsen",        Position: 2, Points: 55), // 855s Honorary IsSenior Female Road
                (ClubNumber: 1022, Name: "Sophie Irwin",       Position: 3, Points: 51), // 900s FirstClaim IsJunior Female Road
                (ClubNumber: 3022, Name: "Megan Irving",       Position: 4, Points: 48), // 925s Honorary IsJunior Female Road
                (ClubNumber: 3002, Name: "Nina Chapman",       Position: 5, Points: 46), // 930s Honorary IsJuvenile Female Road
                (ClubNumber: 1003, Name: "Zoe Dennison",       Position: 6, Points: 44), // 940s FirstClaim IsJuvenile Female Road
                (ClubNumber: 1061, Name: "Helen Turner",       Position: 7, Points: 42), // 980s FirstClaim IsVeteran Female Road
                (ClubNumber: 3062, Name: "Fiona Ursula",       Position: 8, Points: 40), // 995s Honorary IsVeteran Female Road
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position: 1, Points: 60.0), // 860s FirstClaim IsSenior Female Road
                (ClubNumber: 3043, Name: "Georgia Parker",     Position: 2, Points: 55), // 865s Honorary IsSenior Female Road
                (ClubNumber: 3023, Name: "Amber Jennings",     Position: 3, Points: 51), // 920s Honorary IsJunior Female Road
                (ClubNumber: 3003, Name: "Leah Davies",        Position: 4, Points: 48), // 940s Honorary IsJuvenile Female Road
                (ClubNumber: 3063, Name: "Paula Valentine",    Position: 5, Points: 46), // 1025s Honorary IsVeteran Female Road
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForRoadBikeWomen_ConsidersCompetitorClaimStatusHistory(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            var baseCompetitors = TestCompetitors.All.ToList();
            var futures = CompetitorFactory.CreateFutureVersions(baseCompetitors.GetByClubNumber(1043), snapshots: 2, interval: TimeSpan.FromDays(45));
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            scorer.ProcessAll(allRides, competitors, calendar);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderRoadBikeWomenDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertRoadBikeWomenRideMatchesExpected(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, new[]
            {
                (ClubNumber: 1041, Name: "Charlotte Nash",     Position: 1, Points: 60.0), // 850s FirstClaim IsSenior Female Road
                (ClubNumber: 3041, Name: "Ella Norris",        Position: 2, Points: 55), // 875s Honorary IsSenior Female Road
                (ClubNumber: 1001, Name: "Mia Bates",          Position: 3, Points: 51), // 900s FirstClaim IsJuvenile Female Road
                (ClubNumber: 3001, Name: "Tia Bennett",        Position: 4, Points: 48), // 915s Honorary IsJuvenile Female Road
                (ClubNumber: 3021, Name: "Zara Hayes",         Position: 5, Points: 46), // 940s Honorary IsJunior Female Road
                (ClubNumber: 3061, Name: "Diana Thompson",     Position: 6, Points: 44), // 985s Honorary IsVeteran Female Road
            });

            AssertExpectedForEvent(2, new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position: 1, Points: 60.0), // 840s FirstClaim IsSenior Female Road
                (ClubNumber: 3042, Name: "Katie Olsen",        Position: 2, Points: 55), // 855s Honorary IsSenior Female Road
                (ClubNumber: 1022, Name: "Sophie Irwin",       Position: 3, Points: 51), // 900s FirstClaim IsJunior Female Road
                (ClubNumber: 3022, Name: "Megan Irving",       Position: 4, Points: 48), // 925s Honorary IsJunior Female Road
                (ClubNumber: 3002, Name: "Nina Chapman",       Position: 5, Points: 46), // 930s Honorary IsJuvenile Female Road
                (ClubNumber: 1003, Name: "Zoe Dennison",       Position: 6, Points: 44), // 940s FirstClaim IsJuvenile Female Road
                (ClubNumber: 1061, Name: "Helen Turner",       Position: 7, Points: 42), // 980s FirstClaim IsVeteran Female Road
                (ClubNumber: 3062, Name: "Fiona Ursula",       Position: 8, Points: 40), // 995s Honorary IsVeteran Female Road
            });

            AssertExpectedForEvent(3, new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position: 1, Points: 60.0), // 860s FirstClaim IsSenior Female Road
                (ClubNumber: 3043, Name: "Georgia Parker",     Position: 2, Points: 55),   // 865s Honorary IsSenior Female Road
                (ClubNumber: 3023, Name: "Amber Jennings",     Position: 3, Points: 51),   // 920s Honorary IsJunior Female Road
                (ClubNumber: 3003, Name: "Leah Davies",        Position: 4, Points: 48),   // 940s Honorary IsJuvenile Female Road
                (ClubNumber: 3063, Name: "Paula Valentine",    Position: 5, Points: 46),   // 1025s Honorary IsVeteran Female Road
            });
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForVeterans_ScoresAllEligibleVeteransRidersUsingHandicaps(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate());

            // Competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            // Build grouping and debug output
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderVeteransDebugOutput(validRides, competitorVersions, new[] { 4 });
            _ = debug; // breakpoint-friendly

            // Expected data for Veterans competition in Event 4
            // NOTE: Fill in expected positions/points once your handicap algorithm is defined.
            // For now, scaffold with ClubNumber + Name only, positions/points TBD.
            var expectedEvent4 = new[]
            {
                // Example entries (replace with actual expected handicapped positions/points)
                (ClubNumber: 5001, Name: "Mark Anderson", Position: 1, Points: 60.0),
                (ClubNumber: 5002, Name: "Simon Bennett", Position: 2, Points: 55.0),
                (ClubNumber: 5101, Name: "Alice Kendall", Position: 3, Points: 51.0),
                (ClubNumber: 5102, Name: "Sophie Lawrence", Position: 4, Points: 48.0),
                // … continue for all 80 Veterans plus the 6 non-Veterans you added
            };

            // Local assert runner
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertVeteranRideMatchesExpected(ridesForEvent, exp);
            }

            // Assert for Event 4 only
            AssertExpectedForEvent(4, expectedEvent4);
        }

    }
}
