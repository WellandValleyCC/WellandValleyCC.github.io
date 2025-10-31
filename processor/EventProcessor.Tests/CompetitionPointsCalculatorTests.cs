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

        void AssertSeniorRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.SeniorsPosition.Should().Be(expected.Position, $"expected SeniorsPosition {expected.Position} for {context}");
            ride.SeniorsPoints.Should().Be(expected.Points, $"expected SeniorsPoints {expected.Points} for {context}");
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
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60.0),
                (ClubNumber: 1001, Name: "Mia Bates", Position: 2, Points: 53),
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 53),
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 4, Points: 48),
                (ClubNumber: 1002, Name: "Isla Carson", Position: 5, Points: 46)
            };

            var expectedEvent2 = new[]
            {
                // Event 2 actual juvenile results:
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60.0),
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55),
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 51),
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 4, Points: 48),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 5, Points: 46)
            };

            var expectedEvent3 = new[]
            {
                // Event 3 actual juvenile results:
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60.0),
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51)
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
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60.0),
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 55),
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 3, Points: 51),
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60),
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55),
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 49.5),  // Order is: 60,55,51,48,46,44,42,40,39,38,37,36,35
                (ClubNumber: 2012, Name: "Oscar Edwards", Position: 3, Points: 49.5), // 51 + 48 = 99 points.  Shared between two, is 49.5 each.
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 5, Points: 46),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 6, Points: 44),
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60.0),
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51),
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
                CompetitorFactory.Create(4001, "Harper", "Sylvie", ClaimStatus.FirstClaim, true, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4002, "Cross", "Damon", ClaimStatus.FirstClaim, false, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4003, "Langford", "Tessa", ClaimStatus.FirstClaim, true, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4004, "Blake", "Ronan", ClaimStatus.FirstClaim, false, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4005, "Frost", "Imogen", ClaimStatus.FirstClaim, true, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4006, "Drake", "Callum", ClaimStatus.FirstClaim, false, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4007, "Winslow", "Freya", ClaimStatus.FirstClaim, true, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),
                CompetitorFactory.Create(4008, "Thorne", "Jasper", ClaimStatus.FirstClaim, false, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),
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
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClub: true);
            var debug = TestHelpers.RenderSeniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // breakpoint-friendly

            // Expected data for Seniors competition
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn",     Position:  1, Points: 60.0),// time: 830
                (ClubNumber: 1052, Name: "Thomas Reid",     Position:  2, Points: 55),// time: 845
                (ClubNumber: 1041, Name: "Charlotte Nash",  Position:  3, Points: 51),// time: 850
                (ClubNumber: 1042, Name: "Emily Owens",     Position:  4, Points: 48),// time: 860
                (ClubNumber: 3051, Name: "Aaron Quincy",    Position:  5, Points: 46),// time: 865
                (ClubNumber: 3041, Name: "Ella Norris",     Position:  6, Points: 44),// time: 875
                (ClubNumber: 1031, Name: "Oliver King",     Position:  7, Points: 42),// time: 880
                (ClubNumber: 1011, Name: "Liam Evans",      Position:  8, Points: 39.5), // time: 890
                (ClubNumber: 3031, Name: "Reece Kirk",      Position:  8, Points: 39.5), // time: 890
                (ClubNumber: 1032, Name: "Harry Lewis",     Position: 10, Points: 38), // time: 895
                (ClubNumber: 1001, Name: "Mia Bates",       Position: 11, Points: 36.5), // time: 900
                (ClubNumber: 3011, Name: "Jay Ellis",       Position: 11, Points: 36.5), // time: 900
                (ClubNumber: 3001, Name: "Tia Bennett",     Position: 13, Points: 35), // time: 915
                (ClubNumber: 1002, Name: "Isla Carson",     Position: 14, Points: 34), // time: 920
                (ClubNumber: 1071, Name: "Peter Walker",    Position: 15, Points: 33), // time: 930
                (ClubNumber: 1021, Name: "Amelia Hughes",   Position: 16, Points: 31.5), // time: 940
                (ClubNumber: 3021, Name: "Zara Hayes",      Position: 16, Points: 31.5), // time: 940
                (ClubNumber: 3071, Name: "Graham Watson",   Position: 18, Points: 30), // time: 970
                (ClubNumber: 3061, Name: "Diana Thompson",  Position: 19, Points: 29) // time: 985
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  1, Points: 60), // time: 840
                (ClubNumber: 3052, Name: "Charlie Robinson",   Position:  2, Points: 55), // time: 845
                (ClubNumber: 1053, Name: "Daniel Shaw",        Position:  3, Points: 49.5), // time: 855
                (ClubNumber: 3042, Name: "Katie Olsen",        Position:  3, Points: 49.5), // time: 855
                (ClubNumber: 3032, Name: "Tyler Lloyd",        Position:  5, Points: 46), // time: 895
                (ClubNumber: 1022, Name: "Sophie Irwin",       Position:  6, Points: 44), // time: 900
                (ClubNumber: 1023, Name: "Grace Jackson",      Position:  7, Points: 41), // time: 915
                (ClubNumber: 3012, Name: "Max Franklin",       Position:  7, Points: 41), // time: 915
                (ClubNumber: 3022, Name: "Megan Irving",       Position:  9, Points: 39), // time: 925
                (ClubNumber: 3002, Name: "Nina Chapman",       Position: 10, Points: 38), // time: 930
                (ClubNumber: 1003, Name: "Zoe Dennison",       Position: 11, Points: 37), // time: 940
                (ClubNumber: 1012, Name: "Noah Fletcher",      Position: 12, Points: 36), // time: 955
                (ClubNumber: 1072, Name: "Martin Xavier",      Position: 13, Points: 35), // time: 965
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 14, Points: 34), // time: 970
                (ClubNumber: 3072, Name: "Trevor York",        Position: 15, Points: 33), // time: 975
                (ClubNumber: 1061, Name: "Helen Turner",       Position: 16, Points: 32), // time: 980
                (ClubNumber: 1062, Name: "Alison Underwood",   Position: 17, Points: 30.5), // time: 995
                (ClubNumber: 3062, Name: "Fiona Ursula",       Position: 17, Points: 30.5) // time: 995
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3053, Name: "Joseph Stevens",     Position:  1, Points: 60.0), // time: 855
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  2, Points: 55), // time: 860
                (ClubNumber: 3043, Name: "Georgia Parker",     Position:  3, Points: 51), // time: 865
                (ClubNumber: 1041, Name: "Charlotte Nash",     Position:  4, Points: 48), // time: 870
                (ClubNumber: 3033, Name: "Ryan Mitchell",      Position:  5, Points: 46), // time: 890
                (ClubNumber: 1031, Name: "Oliver King",        Position:  6, Points: 44), // time: 905
                (ClubNumber: 1033, Name: "Jack Mason",         Position:  7, Points: 42), // time: 910
                (ClubNumber: 3023, Name: "Amber Jennings",     Position:  8, Points: 40), // time: 920
                (ClubNumber: 3013, Name: "Ben Gibson",         Position:  9, Points: 39), // time: 925
                (ClubNumber: 3003, Name: "Leah Davies",        Position: 10, Points: 38), // time: 940
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 11, Points: 37), // time: 980
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 12, Points: 36), // time: 990
                (ClubNumber: 3073, Name: "Derek Zimmer",       Position: 13, Points: 35), // time: 1000
                (ClubNumber: 1073, Name: "Colin Young",        Position: 14, Points: 34), // time: 1005
                (ClubNumber: 1063, Name: "Janet Vaughn",       Position: 15, Points: 33), // time: 1020
                (ClubNumber: 3063, Name: "Paula Valentine",    Position: 16, Points: 32) // time: 1025
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

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClub: true);
            var debug = TestHelpers.RenderSeniorsDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            // Expected data for Seniors competition
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn",     Position:  1, Points: 60.0),// time: 830
                (ClubNumber: 1052, Name: "Thomas Reid",     Position:  2, Points: 55),// time: 845
// excluded by Futures changing ClaimStatus         (ClubNumber: 1041, Name: "Charlotte Nash",  Position:  3, Points: 51),// time: 850
                (ClubNumber: 1042, Name: "Emily Owens",     Position:  3, Points: 51),// time: 860
                (ClubNumber: 3051, Name: "Aaron Quincy",    Position:  4, Points: 48),// time: 865
                (ClubNumber: 3041, Name: "Ella Norris",     Position:  5, Points: 46),// time: 875
                (ClubNumber: 1031, Name: "Oliver King",     Position:  6, Points: 44),// time: 880
                (ClubNumber: 1011, Name: "Liam Evans",      Position:  7, Points: 41.0), // time: 890
                (ClubNumber: 3031, Name: "Reece Kirk",      Position:  7, Points: 41.0), // time: 890
                (ClubNumber: 1032, Name: "Harry Lewis",     Position:  9, Points: 39), // time: 895
                (ClubNumber: 1001, Name: "Mia Bates",       Position: 10, Points: 37.5), // time: 900
                (ClubNumber: 3011, Name: "Jay Ellis",       Position: 10, Points: 37.5), // time: 900
                (ClubNumber: 3001, Name: "Tia Bennett",     Position: 12, Points: 36), // time: 915
                (ClubNumber: 1002, Name: "Isla Carson",     Position: 13, Points: 35), // time: 920
                (ClubNumber: 1071, Name: "Peter Walker",    Position: 14, Points: 34), // time: 930
                (ClubNumber: 1021, Name: "Amelia Hughes",   Position: 15, Points: 32.5), // time: 940
                (ClubNumber: 3021, Name: "Zara Hayes",      Position: 15, Points: 32.5), // time: 940
                (ClubNumber: 3071, Name: "Graham Watson",   Position: 17, Points: 31), // time: 970
                (ClubNumber: 3061, Name: "Diana Thompson",  Position: 18, Points: 30), // time: 985
                (ClubNumber: 1062, Name: "Alison Underwood",   Position: 19, Points: 29) // time: 995
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  1, Points: 60), // time: 840
                (ClubNumber: 3052, Name: "Charlie Robinson",   Position:  2, Points: 55), // time: 845
                (ClubNumber: 1053, Name: "Daniel Shaw",        Position:  3, Points: 49.5), // time: 855
                (ClubNumber: 3042, Name: "Katie Olsen",        Position:  3, Points: 49.5), // time: 855
                (ClubNumber: 3032, Name: "Tyler Lloyd",        Position:  5, Points: 46), // time: 895
                (ClubNumber: 1022, Name: "Sophie Irwin",       Position:  6, Points: 44), // time: 900
                (ClubNumber: 1023, Name: "Grace Jackson",      Position:  7, Points: 41.0), // time: 915
                (ClubNumber: 3012, Name: "Max Franklin",       Position:  7, Points: 41.0), // time: 915
                (ClubNumber: 3022, Name: "Megan Irving",       Position:  9, Points: 39), // time: 925
                (ClubNumber: 3002, Name: "Nina Chapman",       Position: 10, Points: 38), // time: 930
                (ClubNumber: 1003, Name: "Zoe Dennison",       Position: 11, Points: 37), // time: 940
                (ClubNumber: 1012, Name: "Noah Fletcher",      Position: 12, Points: 36), // time: 955
                (ClubNumber: 1072, Name: "Martin Xavier",      Position: 13, Points: 35), // time: 965
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 14, Points: 34), // time: 970
                (ClubNumber: 3072, Name: "Trevor York",        Position: 15, Points: 33), // time: 975
                (ClubNumber: 1061, Name: "Helen Turner",       Position: 16, Points: 32), // time: 980
// excluded by Futures changing ClaimStatus        (ClubNumber: 1062, Name: "Alison Underwood",   Position: 17, Points: 30.5), // time: 995
                (ClubNumber: 3062, Name: "Fiona Ursula",       Position: 17, Points: 31), // time: 995
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3053, Name: "Joseph Stevens",     Position:  1, Points: 60.0), // time: 855
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  2, Points: 55), // time: 860
                (ClubNumber: 3043, Name: "Georgia Parker",     Position:  3, Points: 51), // time: 865
                (ClubNumber: 1041, Name: "Charlotte Nash",     Position:  4, Points: 48), // time: 870
                (ClubNumber: 3033, Name: "Ryan Mitchell",      Position:  5, Points: 46), // time: 890
                (ClubNumber: 1031, Name: "Oliver King",        Position:  6, Points: 44), // time: 905
                (ClubNumber: 1033, Name: "Jack Mason",         Position:  7, Points: 42), // time: 910
                (ClubNumber: 3023, Name: "Amber Jennings",     Position:  8, Points: 40), // time: 920
                (ClubNumber: 3013, Name: "Ben Gibson",         Position:  9, Points: 39), // time: 925
                (ClubNumber: 3003, Name: "Leah Davies",        Position: 10, Points: 38), // time: 940
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 11, Points: 37), // time: 980
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 12, Points: 36), // time: 990
                (ClubNumber: 3073, Name: "Derek Zimmer",       Position: 13, Points: 35), // time: 1000
                (ClubNumber: 1073, Name: "Colin Young",        Position: 14, Points: 34), // time: 1005
                (ClubNumber: 1063, Name: "Janet Vaughn",       Position: 15, Points: 33), // time: 1020
                (ClubNumber: 3063, Name: "Paula Valentine",    Position: 16, Points: 32), // time: 1025
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
    }
}