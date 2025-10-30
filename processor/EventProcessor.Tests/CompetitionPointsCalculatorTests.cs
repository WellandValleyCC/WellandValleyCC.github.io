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

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_RanksJuvenileRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            List<CalendarEvent> calendar,
            int chosenEventNumber)
        {
            // Arrange
            var calculators = new List<ICompetitionScoreCalculator>
            {
                new JuvenilesScoreCalculator()
            };

            var scorer = new CompetitionPointsCalculator(calculators);

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

            // Assert

            // Build a single string for debugger inspection
            var sb = new StringBuilder();

            for (int evt = 1; evt <= 3; evt++)
            {
                var juvenilesForEvent = allEventRides
                    .Where(r => r.CalendarEvent!.EventNumber == evt)
                    .Where(r =>
                    {
                        var c = competitorsByClubNumber[r.ClubNumber!.Value];
                        return c.IsJuvenile && c.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!juvenilesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no juvenile rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual juvenile results:");
                foreach (var ride in juvenilesForEvent)
                {
                    var club = ride.ClubNumber!.Value;
                    string name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.JuvenilesPosition.HasValue ? ride.JuvenilesPosition.Value.ToString() : "null";
                    var pts = ride.JuvenilesPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            // After: scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Group valid rides by event number from allRides
            var ridesByEvent = allRides
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .GroupBy(r => r.EventNumber)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Helper to assert a single event
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, int Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();

                foreach (var exp in expected)
                {
                    var ride = ridesForEvent
                        .SingleOrDefault(r => r.ClubNumber == exp.ClubNumber && r.CalendarEvent!.EventNumber == evtNumber);

                    ride.Should().NotBeNull($"expected a ride for club {exp.ClubNumber} ({exp.Name}) in event {evtNumber}");

                    ride!.JuvenilesPosition.Should().Be(exp.Position, $"club {exp.ClubNumber} ({exp.Name}) expected position {exp.Position}");
                    ride.JuvenilesPoints.Should().Be(exp.Points, $"club {exp.ClubNumber} ({exp.Name}) expected points {exp.Points}");
                }
            }

            // Assert for events 1..3
            AssertExpectedForEvent(1, expectedEvent1);
            AssertExpectedForEvent(2, expectedEvent2);
            AssertExpectedForEvent(3, expectedEvent3);
        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_ConsidersCompetitorClaimStatusHistoryUsingTestCompetitors(
            List<Ride> allRides,
            List<CalendarEvent> calendar,
            int chosenEventNumber)
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

            // Combine stable competitors with the futures (futures are additional rows for same ClubNumber)
            var competitors = baseCompetitors
                .Concat(miaBatesFutures)
                .Concat(islaCarsonFutures)
                .ToList();

            // Scorer setup
            var calculators = new List<ICompetitionScoreCalculator> { new JuvenilesScoreCalculator() };
            var scorer = new CompetitionPointsCalculator(calculators);

            Func<int, int> points = PointsForPosition;

            // Build Competitor snapshots grouped by club number for resolution by ride date
            var competitorsByClubNumber = competitors
                .Where(c => c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(s => s.LastUpdatedUtc).ToList());

            // Select event rides where the rider exists in our snapshots
            var eventRides = allRides
                .Where(r => r.EventNumber == chosenEventNumber && r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // determine expected juvenile rides using the snapshot effective at each ride's date
            var expectedJuvenileRides = eventRides
                .Where(r =>
                {
                    var clubNumber = r.ClubNumber!.Value;
                    var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent!.EventDate, DateTimeKind.Utc);
                    var snapshot = CompetitorSnapshotResolver.ResolveForEvent(competitorsByClubNumber, clubNumber, eventDateUtc);
                    if (snapshot == null) return false;
                    return snapshot.IsJuvenile && snapshot.ClaimStatus != ClaimStatus.SecondClaim;
                })
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            var expectedEvent1 = new[]
            {
                (ClubNumber: 1011, Name: "Liam Evans", Position: 1, Points: 60),
                (ClubNumber: 3011, Name: "Jay Ellis", Position: 2, Points: 53),
                (ClubNumber: 1001, Name: "Mia Bates", Position: 2, Points: 53),
                (ClubNumber: 1002, Name: "Isla Carson", Position: 4, Points: 48),
            };

            var expectedEvent2 = new[]
            {
                (ClubNumber: 3012, Name: "Max Franklin", Position: 1, Points: 60),
                (ClubNumber: 3002, Name: "Nina Chapman", Position: 2, Points: 55),
                (ClubNumber: 1003, Name: "Zoe Dennison", Position: 3, Points: 51),
                (ClubNumber: 1012, Name: "Noah Fletcher", Position: 4, Points: 48),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 5, Points: 46),
            };

            var expectedEvent3 = new[]
            {
                (ClubNumber: 3013, Name: "Ben Gibson", Position: 1, Points: 60),
                (ClubNumber: 3003, Name: "Leah Davies", Position: 2, Points: 55),
                (ClubNumber: 1013, Name: "Ethan Graham", Position: 3, Points: 51),
            };


            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Assert - scored juveniles are ranked and points assigned according to pointsForPosition

            foreach (var exp in expectedEvent1)
            {
                var ride = eventRides
                    .SingleOrDefault(r => r.ClubNumber == exp.ClubNumber && r.CalendarEvent!.EventNumber == 1);

                ride.Should().NotBeNull($"expected a ride for club {exp.ClubNumber} ({exp.Name}) in event 1");

                ride!.JuvenilesPosition.Should().Be(exp.Position, $"club {exp.ClubNumber} ({exp.Name}) expected position {exp.Position}");
                ride.JuvenilesPoints.Should().Be(exp.Points, $"club {exp.ClubNumber} ({exp.Name}) expected points {exp.Points}");
            }

            foreach (var exp in expectedEvent2)
            {
                var ride = eventRides
                    .SingleOrDefault(r => r.ClubNumber == exp.ClubNumber && r.CalendarEvent!.EventNumber == chosenEventNumber);

                ride.Should().NotBeNull($"expected a ride for club {exp.ClubNumber} ({exp.Name}) in event {chosenEventNumber}");

                ride!.JuvenilesPosition.Should().Be(exp.Position, $"club {exp.ClubNumber} ({exp.Name}) expected position {exp.Position}");
                ride.JuvenilesPoints.Should().Be(exp.Points, $"club {exp.ClubNumber} ({exp.Name}) expected points {exp.Points}");
            }

            foreach (var exp in expectedEvent3)
            {
                var ride = eventRides
                    .SingleOrDefault(r => r.ClubNumber == exp.ClubNumber && r.CalendarEvent!.EventNumber == 3);

                ride.Should().NotBeNull($"expected a ride for club {exp.ClubNumber} ({exp.Name}) in event 3");

                ride!.JuvenilesPosition.Should().Be(exp.Position, $"club {exp.ClubNumber} ({exp.Name}) expected position {exp.Position}");
                ride.JuvenilesPoints.Should().Be(exp.Points, $"club {exp.ClubNumber} ({exp.Name}) expected points {exp.Points}");
            }
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

            var calculators = new List<ICompetitionScoreCalculator> { new JuvenilesScoreCalculator() };
            var scorer = new CompetitionPointsCalculator(calculators);

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

    }
}