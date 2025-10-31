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
        private static readonly IReadOnlyDictionary<int, int> PointsMap = BuildPointsMap();

        private static IReadOnlyDictionary<int, int> BuildPointsMap()
        {
            // primary positions with explicit values
            int[] top = {
                    60,55,51,48,46,44,42,40,39,38,37,36,35,34,33,32,31,30,29,28,27,26,25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1
                };

            // fill remaining positions that are all 1 from 48..100
            int maxPosition = 100;
            var map = new Dictionary<int, int>(capacity: maxPosition);
            for (int i = 0; i < top.Length; i++)
            {
                map[i + 1] = top[i];
            }

            for (int pos = top.Length + 1; pos <= maxPosition; pos++)
            {
                map[pos] = 1;
            }

            return map;
        }

        public static int PointsForPosition(int position)
        {
            return PointsMap.TryGetValue(position, out var pts) ? pts : 0;
        }

        private static void AssertRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, int Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");
            ride!.JuvenilesPosition.Should().Be(expected.Position, $"club {expected.ClubNumber} ({expected.Name}) expected position {expected.Position}");
            ride.JuvenilesPoints.Should().Be(expected.Points, $"club {expected.ClubNumber} ({expected.Name}) expected points {expected.Points}");
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_RanksJuvenileRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var scorer = CompetitionPointsCalculatorFactory.Create();

            var competitorsByClubNumber = competitors.ToDictionary(c => c.ClubNumber);

            var allEventRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var expectedEvent1 = new[]
            {
                // Event 1 actual juvenile results:
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60),
                (ClubNumber: 1001, Name: "Mia Bates", Position: 2, Points: 53),
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 53),
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 4, Points: 48),
                (ClubNumber: 1002, Name: "Isla Carson", Position: 5, Points: 46)
            };

            var expectedEvent2 = new[]
            {
                // Event 2 actual juvenile results:
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60),
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55),
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 51),
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 4, Points: 48),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 5, Points: 46)
            };

            var expectedEvent3 = new[]
            {
                // Event 3 actual juvenile results:
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60),
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51)
            };

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Arrange helper structures
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(allRides);

            // Optional debug output (set breakpoint here or write to ITestOutputHelper)
            var debug = TestHelpers.RenderJuvenileDebugOutput(allRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // place breakpoint here to inspect

            // Assert via helper (use ridesByEvent, not allEventRides)
            void AssertExpectedForEventSimple(int evtNumber, (int ClubNumber, string Name, int Position, int Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertRideMatchesExpected(ridesForEvent, exp);
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
            var scorer = CompetitionPointsCalculatorFactory.Create();

            Func<int, int> points = PointsForPosition;

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
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60),
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 55),
                (ClubNumber: 3001, Name: "Tia Bennett", Position: 3, Points: 51),
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60),
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55),
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 50),  // Order is: 60,55,51,48,46,44,42,40,39,38,37,36,35
                (ClubNumber: 2012, Name: "Oscar Edwards", Position: 3, Points: 50), // 51 + 48 = 99 points.  Shared between two, is 49.5 each.  Rounded up = 50
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 5, Points: 46),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 6, Points: 44),
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60),
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51),
            };

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Arrange helper structures
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);
            var ridesByEvent = TestHelpers.BuildRidesByEvent(allRides);

            // Optional debug output (this render uses the same resolution rules as the assertions)
            var debug = TestHelpers.RenderJuvenileDebugOutput(allRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // breakpoint-friendly

            // Assert using the same assertion helper
            void AssertExpectedForEventWithHistory(int evtNumber, (int ClubNumber, string Name, int Position, int Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertRideMatchesExpected(ridesForEvent, exp);
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

            var scorer = CompetitionPointsCalculatorFactory.Create();

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
            var act = () => scorer.ScoreAllCompetitions(rides, competitors, calendar, PointsForPosition);

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
            var scorer = CompetitionPointsCalculatorFactory.Create();

            // Competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter rides to only those we can score (valid, club numbers present in competitor set)
            var validRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Build grouping and debug output
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClub: true);
            var debug = TestHelpers.RenderJuvenileDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug; // breakpoint-friendly

            // Helper that asserts SeniorsPosition/Points on a ride
            void AssertSeniorMatch(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, int Points) expected)
            {
                var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
                ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");
                ride!.SeniorsPosition.Should().Be(expected.Position, $"club {expected.ClubNumber} ({expected.Name}) expected seniors position {expected.Position}");
                ride.SeniorsPoints.Should().Be(expected.Points, $"club {expected.ClubNumber} ({expected.Name}) expected seniors points {expected.Points}");
            }

            // Example expected data (replace these arrays with the authoritative expected results for Seniors)
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn", Position: 1, Points: 60),
                (ClubNumber: 1041, Name: "Charlotte Nash", Position: 2, Points: 55),
                (ClubNumber: 1052, Name: "Thomas Reid", Position: 3, Points: 51),
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price", Position: 1, Points: 60),
                (ClubNumber: 1053, Name: "Daniel Shaw", Position: 2, Points: 55),
                (ClubNumber: 1052, Name: "Thomas Reid", Position: 3, Points: 51),
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price", Position: 1, Points: 60),
                (ClubNumber: 3053, Name: "Joseph Stevens", Position: 2, Points: 55),
                (ClubNumber: 3051, Name: "Aaron Quincy", Position: 3, Points: 51),
            };

            // Local assert runner
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, int Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertSeniorMatch(ridesForEvent, exp);
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

            var scorer = CompetitionPointsCalculatorFactory.Create();

            var competitors = baseCompetitors.Concat(futuresA).Concat(futuresB).ToList();

            // Build competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on
            var validRides = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClub: true);
            var debug = TestHelpers.RenderJuvenileDebugOutput(validRides, competitorVersions, new[] { 1, 2, 3 });
            _ = debug;

            // Assertion helper (same as above)
            void AssertSeniorMatch(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
            {
                var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
                ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");
                ride!.SeniorsPosition.Should().Be(expected.Position);
                ride.SeniorsPoints.Should().Be(expected.Points);
            }

            // Expected results adjusted for history semantics (replace with authoritative values)
            var expectedEvent1 = new[]
            {
                (ClubNumber: 1051, Name: "James Quinn",     Position:  1, Points: 60),// time: 830
                (ClubNumber: 1052, Name: "Thomas Reid",     Position:  2, Points: 55),// time: 845
                (ClubNumber: 1041, Name: "Charlotte Nash",  Position:  3, Points: 51),// time: 850
                (ClubNumber: 1042, Name: "Emily Owens",     Position:  4, Points: 47.0),// time: 860
                (ClubNumber: 2051, Name: "Samuel Patel",    Position:  4, Points: 47.0),// time: 860
                (ClubNumber: 3051, Name: "Aaron Quincy",    Position:  6, Points: 44),// time: 865
                (ClubNumber: 2041, Name: "Eva Matthews",    Position:  7, Points: 42),// time: 870
                (ClubNumber: 3041, Name: "Ella Norris",     Position:  8, Points: 40),// time: 875
                (ClubNumber: 1031, Name: "Oliver King",     Position:  9, Points: 39),// time: 880
                (ClubNumber: 2031, Name: "Freddie Johnson", Position: 10, Points: 38), // time: 885
                (ClubNumber: 1011, Name: "Liam Evans",      Position: 11, Points: 37.5), // time: 890
                (ClubNumber: 3031, Name: "Reece Kirk",      Position: 11, Points: 37.5), // time: 890
                (ClubNumber: 1032, Name: "Harry Lewis",     Position: 13, Points: 36), // time: 895
                (ClubNumber: 1001, Name: "Mia Bates",       Position: 14, Points: 33.5), // time: 900
                (ClubNumber: 3011, Name: "Jay Ellis",       Position: 14, Points: 33.5), // time: 900
                (ClubNumber: 2011, Name: "Leo Dixon",       Position: 16, Points: 32), // time: 905
                (ClubNumber: 2001, Name: "Maya Abbott",     Position: 17, Points: 31), // time: 910
                (ClubNumber: 3001, Name: "Tia Bennett",     Position: 18, Points: 30), // time: 915
                (ClubNumber: 1002, Name: "Isla Carson",     Position: 19, Points: 29), // time: 920
                (ClubNumber: 1071, Name: "Peter Walker",    Position: 20, Points: 28), // time: 930
                (ClubNumber: 2021, Name: "Chloe Griffin",   Position: 21, Points: 27), // time: 935
                (ClubNumber: 1021, Name: "Amelia Hughes",   Position: 22, Points: 25.5), // time: 940
                (ClubNumber: 3021, Name: "Zara Hayes",      Position: 22, Points: 25.5), // time: 940
                (ClubNumber: 3071, Name: "Graham Watson",   Position: 24, Points: 24), // time: 970
                (ClubNumber: 3061, Name: "Diana Thompson",  Position: 25, Points: 23), // time: 985
                (ClubNumber: 2061, Name: "Beth Taylor",     Position: 26, Points: 22),// time: 990

            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  1, Points: 60), // time: 840
                (ClubNumber: 3052, Name: "Charlie Robinson",   Position:  2, Points: 55), // time: 845
                (ClubNumber: 2052, Name: "Jacob Roberts",      Position:  3, Points: 51), // time: 850
                (ClubNumber: 1053, Name: "Daniel Shaw",        Position:  4, Points: 47.0), // time: 855
                (ClubNumber: 3042, Name: "Katie Olsen",        Position:  4, Points: 47.0), // time: 855
                (ClubNumber: 2042, Name: "Rosie Nelson",       Position:  6, Points: 44), // time: 860
                (ClubNumber: 2032, Name: "Archie Kerr",        Position:  7, Points: 42), // time: 890
                (ClubNumber: 3032, Name: "Tyler Lloyd",        Position:  8, Points: 40), // time: 895
                (ClubNumber: 1022, Name: "Sophie Irwin",       Position:  9, Points: 39), // time: 900
                (ClubNumber: 1023, Name: "Grace Jackson",      Position: 10, Points: 37.5), // time: 915
                (ClubNumber: 3012, Name: "Max Franklin",       Position: 10, Points: 37.5), // time: 915
                (ClubNumber: 2022, Name: "Millie Harrison",    Position: 12, Points: 36), // time: 920
                (ClubNumber: 2002, Name: "Ella Barker",        Position: 13, Points: 33.5), // time: 925
                (ClubNumber: 3022, Name: "Megan Irving",       Position: 13, Points: 33.5), // time: 925
                (ClubNumber: 3002, Name: "Nina Chapman",       Position: 15, Points: 32), // time: 930
                (ClubNumber: 1003, Name: "Zoe Dennison",       Position: 16, Points: 29.5), // time: 940
                (ClubNumber: 2012, Name: "Oscar Edwards",      Position: 16, Points: 29.5), // time: 940
                (ClubNumber: 1012, Name: "Noah Fletcher",      Position: 18, Points: 28), // time: 955
                (ClubNumber: 1072, Name: "Martin Xavier",      Position: 19, Points: 27), // time: 965
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 20, Points: 26), // time: 970
                (ClubNumber: 3072, Name: "Trevor York",        Position: 21, Points: 25), // time: 975
                (ClubNumber: 1061, Name: "Helen Turner",       Position: 22, Points: 25.5), // time: 980
                (ClubNumber: 2072, Name: "Nathan Zane",        Position: 22, Points: 25.5), // time: 980
                (ClubNumber: 1062, Name: "Alison Underwood",   Position: 24, Points: 23.5), // time: 995
                (ClubNumber: 3062, Name: "Fiona Ursula",       Position: 24, Points: 23.5), // time: 995
                (ClubNumber: 2062, Name: "Rachel Upton",       Position: 26, Points: 22), // time: 1000
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3053, Name: "Joseph Stevens",     Position:  1, Points: 60), // time: 855
                (ClubNumber: 1043, Name: "Lucy Price",         Position:  2, Points: 53.0), // time: 860
                (ClubNumber: 2053, Name: "Logan Simpson",      Position:  2, Points: 53.0), // time: 860
                (ClubNumber: 3043, Name: "Georgia Parker",     Position:  4, Points: 48), // time: 865
                (ClubNumber: 1041, Name: "Charlotte Nash",     Position:  5, Points: 45.0), // time: 870
                (ClubNumber: 2043, Name: "Hannah O'Brien",     Position:  5, Points: 45.0), // time: 870
                (ClubNumber: 2033, Name: "Henry Lawson",       Position:  7, Points: 42), // time: 880
                (ClubNumber: 3033, Name: "Ryan Mitchell",      Position:  8, Points: 40), // time: 890
                (ClubNumber: 1031, Name: "Oliver King",        Position:  9, Points: 39), // time: 905
                (ClubNumber: 1033, Name: "Jack Mason",         Position: 10, Points: 37.5), // time: 910
                (ClubNumber: 2023, Name: "Lily Ingram",        Position: 10, Points: 37.5), // time: 910
                (ClubNumber: 3023, Name: "Amber Jennings",     Position: 12, Points: 36), // time: 920
                (ClubNumber: 3013, Name: "Ben Gibson",         Position: 13, Points: 35), // time: 925
                (ClubNumber: 2003, Name: "Ruby Carter",        Position: 14, Points: 33.5), // time: 935
                (ClubNumber: 3003, Name: "Leah Davies",        Position: 15, Points: 33.5), // time: 940
                (ClubNumber: 2013, Name: "George Foster",      Position: 16, Points: 32), // time: 950
                (ClubNumber: 1013, Name: "Ethan Graham",       Position: 17, Points: 31), // time: 980
                (ClubNumber: 1071, Name: "Peter Walker",       Position: 18, Points: 30), // time: 990
                (ClubNumber: 2073, Name: "Luke Adams",         Position: 19, Points: 29), // time: 995
                (ClubNumber: 1073, Name: "Colin Young",        Position: 20, Points: 28), // time: 1005
                (ClubNumber: 2063, Name: "Claire Vincent",     Position: 21, Points: 27), // time: 1010
                (ClubNumber: 1063, Name: "Janet Vaughn",       Position: 22, Points: 26), // time: 1020
                (ClubNumber: 3063, Name: "Paula Valentine",    Position: 23, Points: 25), // time: 1025
            };

            // Local runner
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                    AssertSeniorMatch(ridesForEvent, exp);
            }

            AssertExpectedForEvent(1, expectedEvent1);
            AssertExpectedForEvent(2, expectedEvent2);
            AssertExpectedForEvent(3, expectedEvent3);
        }
    }
}