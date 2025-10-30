using AutoFixture.Xunit2;
using ClubProcessor.Calculators;
using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Orchestration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;

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
            int eventNumber)
        {
            // Arrange
            var calculators = new List<ICompetitionScoreCalculator>
            {
                new JuvenilesScoreCalculator()
            };

            var scorer = new CompetitionPointsCalculator(calculators);

            // Func<int, int> pointsForPosition = pos => pos <= 20 ? 21 - pos : 0;

            var competitorsByClubNumber = competitors.ToDictionary(c => c.ClubNumber);

            var eventRides = allRides
                .Where(r => r.EventNumber == eventNumber && r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var juveniles = eventRides
                .Where(r =>
                {
                    var c = competitorsByClubNumber[r.ClubNumber!.Value];
                    return c.IsJuvenile && c.ClaimStatus != ClaimStatus.SecondClaim;
                })
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Assert
            for (int i = 0; i < juveniles.Count; i++)
            {
                var ride = juveniles[i];
                int expectedPosition = i + 1;
                int expectedPoints = PointsForPosition(expectedPosition);

                ride.JuvenilesPosition.Should().Be(expectedPosition,
                    $"ride at index {i} should be ranked {expectedPosition}");

                ride.JuvenilesPoints.Should().Be(expectedPoints,
                    $"ride at index {i} should receive {expectedPoints} points");
            }

            var nonJuveniles = eventRides.Except(juveniles);
            foreach (var ride in nonJuveniles)
            {
                ride.JuvenilesPosition.Should().BeNull("non-juveniles should not be assigned a JuvenilesPosition");
                ride.JuvenilesPoints.Should().Be(0, "non-juveniles should not receive JuvenilesPoints");
            }
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

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, calendar, PointsForPosition);

            // Assert - scored juveniles are ranked and points assigned according to pointsForPosition
            for (int i = 0; i < expectedJuvenileRides.Count; i++)
            {
                var ride = expectedJuvenileRides[i];
                int expectedPosition = i + 1;
                int expectedPoints = points(expectedPosition);

                ride.JuvenilesPosition.Should().Be(expectedPosition,
                    $"ride at index {i} should be ranked {expectedPosition}");
                ride.JuvenilesPoints.Should().Be(expectedPoints,
                    $"ride at index {i} should receive {expectedPoints} points");
            }

            // Assert - other event rides should not have juvenile scoring
            var nonJuvenileAndSecondClaimJuveniles = eventRides.Except(expectedJuvenileRides).ToList();
            foreach (var ride in nonJuvenileAndSecondClaimJuveniles)
            {
                ride.JuvenilesPosition.Should().BeNull("non-juveniles, or juveniles who are second claim should not be assigned a JuvenilesPosition");
                ride.JuvenilesPoints.Should().Be(0, "non-juveniles, or juveniles who are second claim non-juveniles should not receive JuvenilesPoints");
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