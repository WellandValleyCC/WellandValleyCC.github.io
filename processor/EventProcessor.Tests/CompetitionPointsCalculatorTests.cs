using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubProcessor.Orchestration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;

namespace EventProcessor.Tests
{
    public class CompetitionPointsCalculatorTests
    {
        public static List<RoundRobinRider> CreateSampleRiders() =>
             new()
             {
                    new RoundRobinRider
                    {
                        Name = "Alex Morton",
                        RoundRobinClub = "Aerologic",
                        IsFemale = false
                    },
                    new RoundRobinRider
                    {
                        Name = "Riley Thompson",
                        RoundRobinClub = "Ashby ICC",
                        IsFemale = false
                    },
                    new RoundRobinRider
                    {
                        Name = "Sophie Langford",
                        RoundRobinClub = "RFW",
                        IsFemale = true
                    },
                    new RoundRobinRider
                    {
                        Name = "Marcus Hale",
                        RoundRobinClub = "RFW",
                        IsFemale = false
                    },
                    new RoundRobinRider
                    {
                        Name = "Elliot Fraser",
                        RoundRobinClub = "LFCC",
                        IsFemale = false
                    },
                    new RoundRobinRider
                    {
                        Name = "Hannah Keating",
                        RoundRobinClub = "LFCC",
                        IsFemale = true
                    },
                    new RoundRobinRider
                    {
                        Name = "Jordan Pike",
                        RoundRobinClub = "LFCC",
                        IsFemale = false
                    }
             };

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            var competitorsByClubNumber = competitors.ToDictionary(c => c.ClubNumber);

            var allEventRides = allRides
                .Where(r => r.Status == RideStatus.Valid)
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

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            // Build Competitor snapshots grouped by club number for resolution by ride date
            var competitorsByClubNumber = competitors
                .Where(c => c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.CreatedUtc).ToList());

            // Select event rides where the rider exists in our snapshots
            var allEventRides = allRides
                .Where(r => r.Status == RideStatus.Valid)
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

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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

            int competitionYear = ridesUsingFutureCompetitors.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            // Build snapshot dictionary
            var competitorsByClubNumber = competitors
                .Where(c => c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.CreatedUtc).ToList());

            // Filter rides for Event 2
            var eventRides = rides
                .Where(r => r.EventNumber == eventNumber && r.Status == RideStatus.Valid)
                .Where(r => r.ClubNumber.HasValue)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act & Assert
            var act = () => scorer.ProcessAll(rides, competitors, calendar, roundRobinRiders);

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            // Competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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

            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            var competitors = baseCompetitors.Concat(futuresA).Concat(futuresB).Concat(futuresC).ToList();

            // Build competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Include all eligible rides (club members and guests)
            var validRides = allRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
        public void EligibleRidersEventRank_IsAssignedAccordingToTotalTime(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Include all eligible rides (club members only)
            var validRides = allRides
                .Where(r => r.Competitor?.IsEligible() == true &&
                            r.Status == RideStatus.Valid)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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

                    ride.EventEligibleRidersRank.Should().Be(expectedRank, because: $"overall position amongst club members for Event {evtNumber} rider {ride.Name}");

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Include all eligible rides - i.e. not DNS, DNF, DQ
            var validRides = allRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
        public void EventEligibleRoadBikeRank_IsAssignedAmongRoadBikeRidersOnly(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Include all eligible rides (club members only)
            var validRides = allRides
                .Where(r => r.Competitor?.IsEligible() == true &&
                            r.Status == RideStatus.Valid)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
        public void EligibleRidersRoadBikeRank_only_counts_eligible_roadbike_riders_and_matches_EventRank_where_applicable(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r =>
                    r.Competitor?.IsEligible() == true &&
                    r.Status == RideStatus.Valid)
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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

                    ride.EventEligibleRoadBikeRidersRank.Should().Be(expectedRoadRank,
                        because: "EventEligibleRoadBikeRidersRank should reflect sequential position among eligible road-bike riders using the same overall order");

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();

            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);
            var competitors = baseCompetitors.Concat(futures).ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders();
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);
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
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            // Competitor versions lookup (ordered by CreatedUtc ascending)
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            // Filter the rides we will assert on - just those for club members
            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var roundRobinRiders = CreateSampleRiders(); 
            
            // Act
            scorer.ProcessAll(allRides, competitors, calendar, roundRobinRiders);

            // Build grouping and debug output
            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);
            var debug = TestHelpers.RenderVeteransDebugOutput(validRides, competitorVersions, new[] { 4 });
            _ = debug; // breakpoint-friendly

            // Expected data for Veterans competition in Event 4
            // NOTE: Fill in expected positions/points once your handicap algorithm is defined.
            // For now, scaffold with ClubNumber + Name only, positions/points TBD.
            var expectedEvent4 = new[]
            {
                // Event 4 actual results:
                (ClubNumber: 5140, Name: "Theresa Adams",         Position: 1   , Points: 60.0  ), // 1829s VetsBucket:40   AAT:793  HandicapTotalSeconds:1036 FirstClaim Veteran  Female Road
                (ClubNumber: 5139, Name: "Frances Zane",          Position: 2   , Points: 55   ), // 1828s VetsBucket:39   AAT:762  HandicapTotalSeconds:1066 FirstClaim Veteran  Female TT
                (ClubNumber: 5138, Name: "Joanne White",          Position: 3   , Points: 51   ), // 1827s VetsBucket:38   AAT:732  HandicapTotalSeconds:1095 FirstClaim Veteran  Female Road
                (ClubNumber: 5137, Name: "Helen Vincent",         Position: 4   , Points: 48   ), // 1826s VetsBucket:37   AAT:704  HandicapTotalSeconds:1122 FirstClaim Veteran  Female TT
                (ClubNumber: 5136, Name: "Margaret Upton",        Position: 5   , Points: 46   ), // 1825s VetsBucket:36   AAT:677  HandicapTotalSeconds:1148 FirstClaim Veteran  Female Road
                (ClubNumber: 5135, Name: "Denise Taylor",         Position: 6   , Points: 44   ), // 1824s VetsBucket:35   AAT:652  HandicapTotalSeconds:1172 FirstClaim Veteran  Female TT
                (ClubNumber: 5134, Name: "Janice Simpson",        Position: 7   , Points: 42   ), // 1823s VetsBucket:34   AAT:628  HandicapTotalSeconds:1195 FirstClaim Veteran  Female Road
                (ClubNumber: 5133, Name: "Deborah Roberts",       Position: 8   , Points: 40   ), // 1822s VetsBucket:33   AAT:604  HandicapTotalSeconds:1218 FirstClaim Veteran  Female TT
                (ClubNumber: 5132, Name: "Louise Patel",          Position: 9   , Points: 39   ), // 1821s VetsBucket:32   AAT:582  HandicapTotalSeconds:1239 FirstClaim Veteran  Female Road
                (ClubNumber: 5040, Name: "Edward Nelson",         Position: 10  , Points: 38   ), // 1809s VetsBucket:40   AAT:561  HandicapTotalSeconds:1248 FirstClaim Veteran  Male   TT
                (ClubNumber: 5131, Name: "Catherine O'Brien",     Position: 11  , Points: 37   ), // 1820s VetsBucket:31   AAT:561  HandicapTotalSeconds:1259 FirstClaim Veteran  Female TT
                (ClubNumber: 5039, Name: "Geoffrey Matthews",     Position: 12  , Points: 36   ), // 1808s VetsBucket:39   AAT:534  HandicapTotalSeconds:1274 FirstClaim Veteran  Male   Road
                (ClubNumber: 5130, Name: "Audrey Nelson",         Position: 13  , Points: 35   ), // 1819s VetsBucket:30   AAT:541  HandicapTotalSeconds:1278 FirstClaim Veteran  Female Road
                (ClubNumber: 5129, Name: "Eleanor Matthews",      Position: 14  , Points: 34   ), // 1818s VetsBucket:29   AAT:521  HandicapTotalSeconds:1297 FirstClaim Veteran  Female TT
                (ClubNumber: 5038, Name: "Norman Lewis",          Position: 15  , Points: 33   ), // 1807s VetsBucket:38   AAT:508  HandicapTotalSeconds:1299 FirstClaim Veteran  Male   TT
                (ClubNumber: 5128, Name: "Monica Lewis",          Position: 16  , Points: 32   ), // 1817s VetsBucket:28   AAT:502  HandicapTotalSeconds:1315 FirstClaim Veteran  Female Road
                (ClubNumber: 5037, Name: "Leslie Kerr",           Position: 17  , Points: 31   ), // 1806s VetsBucket:37   AAT:483  HandicapTotalSeconds:1323 FirstClaim Veteran  Male   Road
                (ClubNumber: 5127, Name: "Vanessa Kerr",          Position: 18  , Points: 30   ), // 1816s VetsBucket:27   AAT:484  HandicapTotalSeconds:1332 FirstClaim Veteran  Female TT
                (ClubNumber: 5036, Name: "Eric Johnson",          Position: 19  , Points: 29   ), // 1805s VetsBucket:36   AAT:459  HandicapTotalSeconds:1346 FirstClaim Veteran  Male   TT
                (ClubNumber: 5126, Name: "Patricia Johnson",      Position: 20  , Points: 28   ), // 1815s VetsBucket:26   AAT:467  HandicapTotalSeconds:1348 FirstClaim Veteran  Female Road
                (ClubNumber: 5125, Name: "Joanna Irving",         Position: 21  , Points: 27   ), // 1814s VetsBucket:25   AAT:450  HandicapTotalSeconds:1364 FirstClaim Veteran  Female TT
                (ClubNumber: 5035, Name: "Rodney Irving",         Position: 22  , Points: 26   ), // 1804s VetsBucket:35   AAT:437  HandicapTotalSeconds:1367 FirstClaim Veteran  Male   Road
                (ClubNumber: 5124, Name: "Melissa Harrison",      Position: 23  , Points: 25   ), // 1813s VetsBucket:24   AAT:434  HandicapTotalSeconds:1379 FirstClaim Veteran  Female Road
                (ClubNumber: 5034, Name: "Keith Harrison",        Position: 24  , Points: 24   ), // 1803s VetsBucket:34   AAT:415  HandicapTotalSeconds:1388 FirstClaim Veteran  Male   TT
                (ClubNumber: 5123, Name: "Angela Gibson",         Position: 25  , Points: 23   ), // 1812s VetsBucket:23   AAT:419  HandicapTotalSeconds:1393 FirstClaim Veteran  Female TT
                (ClubNumber: 5033, Name: "Howard Gibson",         Position: 26  , Points: 21.5 ), // 1802s VetsBucket:33   AAT:395  HandicapTotalSeconds:1407 FirstClaim Veteran  Male   Road
                (ClubNumber: 5122, Name: "Rebecca Fletcher",      Position: 26  , Points: 21.5 ), // 1811s VetsBucket:22   AAT:404  HandicapTotalSeconds:1407 FirstClaim Veteran  Female Road
                (ClubNumber: 5121, Name: "Natalie Edwards",       Position: 28  , Points: 20   ), // 1810s VetsBucket:21   AAT:390  HandicapTotalSeconds:1420 FirstClaim Veteran  Female TT
                (ClubNumber: 5032, Name: "Barry Fletcher",        Position: 29  , Points: 19   ), // 1801s VetsBucket:32   AAT:375  HandicapTotalSeconds:1426 FirstClaim Veteran  Male   TT
                (ClubNumber: 5031, Name: "Alan Edwards",          Position: 30  , Points: 18   ), // 1800s VetsBucket:31   AAT:357  HandicapTotalSeconds:1443 FirstClaim Veteran  Male   Road
                (ClubNumber: 5120, Name: "Samantha Dixon",        Position: 31  , Points: 17   ), // 1809s VetsBucket:20   AAT:365  HandicapTotalSeconds:1444 FirstClaim Veteran  Female Road
                (ClubNumber: 5030, Name: "Clive Dixon",           Position: 32  , Points: 16   ), // 1799s VetsBucket:30   AAT:339  HandicapTotalSeconds:1460 FirstClaim Veteran  Male   TT
                (ClubNumber: 5119, Name: "Julia Carter",          Position: 33  , Points: 15   ), // 1808s VetsBucket:19   AAT:342  HandicapTotalSeconds:1466 FirstClaim Veteran  Female TT
                (ClubNumber: 5029, Name: "Russell Carter",        Position: 34  , Points: 14   ), // 1798s VetsBucket:29   AAT:321  HandicapTotalSeconds:1477 FirstClaim Veteran  Male   Road
                (ClubNumber: 5118, Name: "Eliza Barker",          Position: 35  , Points: 13   ), // 1807s VetsBucket:18   AAT:322  HandicapTotalSeconds:1485 FirstClaim Veteran  Female Road
                (ClubNumber: 5028, Name: "Nigel Barker",          Position: 36  , Points: 12   ), // 1797s VetsBucket:28   AAT:305  HandicapTotalSeconds:1492 FirstClaim Veteran  Male   TT
                (ClubNumber: 5117, Name: "Lydia Abbott",          Position: 37  , Points: 11   ), // 1806s VetsBucket:17   AAT:303  HandicapTotalSeconds:1503 FirstClaim Veteran  Female TT
                (ClubNumber: 5027, Name: "Trevor Abbott",         Position: 38  , Points: 10   ), // 1796s VetsBucket:27   AAT:289  HandicapTotalSeconds:1507 FirstClaim Veteran  Male   Road
                (ClubNumber: 5116, Name: "Harriet Zimmer",        Position: 39  , Points: 9    ), // 1805s VetsBucket:16   AAT:286  HandicapTotalSeconds:1519 FirstClaim Veteran  Female Road
                (ClubNumber: 5026, Name: "Harvey Zimmer",         Position: 40  , Points: 8    ), // 1795s VetsBucket:26   AAT:274  HandicapTotalSeconds:1521 FirstClaim Veteran  Male   TT
                (ClubNumber: 5115, Name: "Claudia Young",         Position: 41  , Points: 7    ), // 1804s VetsBucket:15   AAT:271  HandicapTotalSeconds:1533 FirstClaim Veteran  Female TT
                (ClubNumber: 5025, Name: "Malcolm Young",         Position: 42  , Points: 6    ), // 1794s VetsBucket:25   AAT:259  HandicapTotalSeconds:1535 FirstClaim Veteran  Male   Road
                (ClubNumber: 5114, Name: "Phoebe Xavier",         Position: 43  , Points: 5    ), // 1803s VetsBucket:14   AAT:257  HandicapTotalSeconds:1546 FirstClaim Veteran  Female Road
                (ClubNumber: 5024, Name: "Leon Xavier",           Position: 44  , Points: 4    ), // 1793s VetsBucket:24   AAT:245  HandicapTotalSeconds:1548 FirstClaim Veteran  Male   TT
                (ClubNumber: 5113, Name: "Naomi Walker",          Position: 45  , Points: 3    ), // 1802s VetsBucket:13   AAT:244  HandicapTotalSeconds:1558 FirstClaim Veteran  Female TT
                (ClubNumber: 5023, Name: "Dean Walker",           Position: 46  , Points: 2    ), // 1792s VetsBucket:23   AAT:231  HandicapTotalSeconds:1561 FirstClaim Veteran  Male   Road
                (ClubNumber: 5112, Name: "Isabel Vaughn",         Position: 47  , Points: 1    ), // 1801s VetsBucket:12   AAT:232  HandicapTotalSeconds:1569 FirstClaim Veteran  Female Road
                (ClubNumber: 5022, Name: "Stuart Vaughn",         Position: 48  , Points: 1    ), // 1791s VetsBucket:22   AAT:218  HandicapTotalSeconds:1573 FirstClaim Veteran  Male   TT
                (ClubNumber: 5111, Name: "Megan Underwood",       Position: 49  , Points: 1    ), // 1800s VetsBucket:11   AAT:222  HandicapTotalSeconds:1578 FirstClaim Veteran  Female TT
                (ClubNumber: 5021, Name: "Gavin Underwood",       Position: 50  , Points: 1    ), // 1790s VetsBucket:21   AAT:206  HandicapTotalSeconds:1584 FirstClaim Veteran  Male   Road
                (ClubNumber: 5110, Name: "Bethany Turner",        Position: 51  , Points: 1    ), // 1799s VetsBucket:10   AAT:213  HandicapTotalSeconds:1586 FirstClaim Veteran  Female Road
                (ClubNumber: 5109, Name: "Grace Stevens",         Position: 52  , Points: 1    ), // 1798s VetsBucket:9    AAT:205  HandicapTotalSeconds:1593 FirstClaim Veteran  Female TT
                (ClubNumber: 5020, Name: "Paul Turner",           Position: 53  , Points: 1    ), // 1789s VetsBucket:20   AAT:194  HandicapTotalSeconds:1595 FirstClaim Veteran  Male   TT
                (ClubNumber: 5108, Name: "Charlotte Reeves",      Position: 54  , Points: 1    ), // 1797s VetsBucket:8    AAT:198  HandicapTotalSeconds:1599 FirstClaim Veteran  Female Road
                (ClubNumber: 5107, Name: "Laura Quinn",           Position: 55  , Points: 1    ), // 1796s VetsBucket:7    AAT:191  HandicapTotalSeconds:1605 FirstClaim Veteran  Female TT
                (ClubNumber: 5019, Name: "Martin Stevens",        Position: 56  , Points: 1    ), // 1788s VetsBucket:19   AAT:182  HandicapTotalSeconds:1606 FirstClaim Veteran  Male   Road
                (ClubNumber: 5106, Name: "Hannah Peters",         Position: 57  , Points: 1    ), // 1795s VetsBucket:6    AAT:185  HandicapTotalSeconds:1610 FirstClaim Veteran  Female Road
                (ClubNumber: 5105, Name: "Rachel Osborne",        Position: 58  , Points: 1    ), // 1794s VetsBucket:5    AAT:180  HandicapTotalSeconds:1614 FirstClaim Veteran  Female TT
                (ClubNumber: 5018, Name: "Anthony Reid",          Position: 59  , Points: 1    ), // 1787s VetsBucket:18   AAT:171  HandicapTotalSeconds:1616 FirstClaim Veteran  Male   TT
                (ClubNumber: 5104, Name: "Emily Norris",          Position: 60  , Points: 1    ), // 1793s VetsBucket:4    AAT:176  HandicapTotalSeconds:1617 FirstClaim Veteran  Female Road
                (ClubNumber: 5103, Name: "Clara Mitchell",        Position: 61  , Points: 1    ), // 1792s VetsBucket:3    AAT:173  HandicapTotalSeconds:1619 FirstClaim Veteran  Female TT
                (ClubNumber: 5102, Name: "Sophie Lawrence",       Position: 62  , Points: 1    ), // 1791s VetsBucket:2    AAT:169  HandicapTotalSeconds:1622 FirstClaim Veteran  Female Road
                (ClubNumber: 5101, Name: "Alice Kendall",         Position: 63  , Points: 1    ), // 1790s VetsBucket:1    AAT:167  HandicapTotalSeconds:1623 FirstClaim Veteran  Female TT
                (ClubNumber: 5017, Name: "Jason Quinn",           Position: 64  , Points: 1    ), // 1786s VetsBucket:17   AAT:160  HandicapTotalSeconds:1626 FirstClaim Veteran  Male   Road
                (ClubNumber: 5016, Name: "Neil Parker",           Position: 65  , Points: 1    ), // 1785s VetsBucket:16   AAT:150  HandicapTotalSeconds:1635 FirstClaim Veteran  Male   TT
                (ClubNumber: 5015, Name: "Darren Owens",          Position: 66  , Points: 1    ), // 1784s VetsBucket:15   AAT:140  HandicapTotalSeconds:1644 FirstClaim Veteran  Male   Road
                (ClubNumber: 5014, Name: "Craig Norton",          Position: 67  , Points: 1    ), // 1783s VetsBucket:14   AAT:130  HandicapTotalSeconds:1653 FirstClaim Veteran  Male   TT
                (ClubNumber: 5013, Name: "Gareth Murray",         Position: 68  , Points: 1    ), // 1782s VetsBucket:13   AAT:121  HandicapTotalSeconds:1661 FirstClaim Veteran  Male   Road
                (ClubNumber: 5012, Name: "Patrick Lawson",        Position: 69  , Points: 1    ), // 1781s VetsBucket:12   AAT:112  HandicapTotalSeconds:1669 FirstClaim Veteran  Male   TT
                (ClubNumber: 5011, Name: "Stephen Kirk",          Position: 70  , Points: 1    ), // 1780s VetsBucket:11   AAT:104  HandicapTotalSeconds:1676 FirstClaim Veteran  Male   Road
                (ClubNumber: 5010, Name: "Colin Jennings",        Position: 71  , Points: 1    ), // 1779s VetsBucket:10   AAT:96   HandicapTotalSeconds:1683 FirstClaim Veteran  Male   TT
                (ClubNumber: 5009, Name: "Douglas Ingram",        Position: 72  , Points: 1    ), // 1778s VetsBucket:9    AAT:88   HandicapTotalSeconds:1690 FirstClaim Veteran  Male   Road
                (ClubNumber: 5008, Name: "Adrian Holt",           Position: 73  , Points: 1    ), // 1777s VetsBucket:8    AAT:80   HandicapTotalSeconds:1697 FirstClaim Veteran  Male   TT
                (ClubNumber: 5007, Name: "Philip Grayson",        Position: 74  , Points: 1    ), // 1776s VetsBucket:7    AAT:73   HandicapTotalSeconds:1703 FirstClaim Veteran  Male   Road
                (ClubNumber: 5006, Name: "Ian Foster",            Position: 75  , Points: 1    ), // 1775s VetsBucket:6    AAT:66   HandicapTotalSeconds:1709 FirstClaim Veteran  Male   TT
                (ClubNumber: 5005, Name: "George Ellison",        Position: 76  , Points: 1    ), // 1774s VetsBucket:5    AAT:59   HandicapTotalSeconds:1715 FirstClaim Veteran  Male   Road
                (ClubNumber: 5004, Name: "Hugh Dalton",           Position: 77  , Points: 1    ), // 1773s VetsBucket:4    AAT:53   HandicapTotalSeconds:1720 FirstClaim Veteran  Male   TT
                (ClubNumber: 5003, Name: "Victor Chapman",        Position: 78  , Points: 1    ), // 1772s VetsBucket:3    AAT:47   HandicapTotalSeconds:1725 FirstClaim Veteran  Male   Road
                (ClubNumber: 5002, Name: "Simon Bennett",         Position: 79  , Points: 1    ), // 1771s VetsBucket:2    AAT:42   HandicapTotalSeconds:1729 FirstClaim Veteran  Male   TT
                (ClubNumber: 5001, Name: "Mark Anderson",         Position: 80  , Points: 1    ), // 1770s VetsBucket:1    AAT:36   HandicapTotalSeconds:1734 FirstClaim Veteran  Male   Road
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
