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
        public void EventScoring_ForSeniors_RanksAllAgeGroupRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            int eventNumber)
        {
            // Arrange
            var calculators = new List<ICompetitionScoreCalculator>
            {
                new SeniorsScoreCalculator()
            };

            var scorer = new CompetitionPointsCalculator(calculators);

            Func<int, int> pointsForPosition = pos => pos <= 20 ? 21 - pos : 0;

            var competitorsByClubNumber = competitors.ToDictionary(c => c.ClubNumber);

            var eventRides = allRides
                .Where(r => r.EventNumber == eventNumber && r.Eligibility == RideEligibility.Valid)
                .Where(r => r.ClubNumber.HasValue && competitorsByClubNumber.ContainsKey(r.ClubNumber.Value))
                .ToList();

            var allCompetitionEligibleRides = eventRides
                .Where(r =>
                {
                    var c = competitorsByClubNumber[r.ClubNumber!.Value];
                    return c.ClaimStatus != ClaimStatus.SecondClaim;
                })
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            // Act
            scorer.ScoreAllCompetitions(allRides, competitors, pointsForPosition);

            // Assert
            for (int i = 0; i < allCompetitionEligibleRides.Count; i++)
            {
                var ride = allCompetitionEligibleRides[i];
                int expectedPosition = i + 1;
                int expectedPoints = pointsForPosition(expectedPosition);

                ride.SeniorsPosition.Should().Be(expectedPosition,
                    $"ride at index {i} should be ranked {expectedPosition} in Seniors");

                ride.SeniorsPoints.Should().Be(expectedPoints,
                    $"ride at index {i} should receive {expectedPoints} Seniors points");
            }

        }

        [Theory]
        [EventAutoData]
        public void EventScoring_ForJuveniles_RanksJuvenileRidersWhoAreNotSecondClaim(
            List<Competitor> competitors,
            List<Ride> allRides,
            int eventNumber)
        {
            // Arrange
            var calculators = new List<ICompetitionScoreCalculator>
            {
                new JuvenilesScoreCalculator()
            };

            var scorer = new CompetitionPointsCalculator(calculators);

            Func<int, int> pointsForPosition = pos => pos <= 20 ? 21 - pos : 0;

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
            scorer.ScoreAllCompetitions(allRides, competitors, pointsForPosition);

            // Assert
            for (int i = 0; i < juveniles.Count; i++)
            {
                var ride = juveniles[i];
                int expectedPosition = i + 1;
                int expectedPoints = pointsForPosition(expectedPosition);

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

        private static Ride CloneRide(Ride r) => new Ride { EventNumber = r.EventNumber, ClubNumber = r.ClubNumber, Name = r.Name, TotalSeconds = r.TotalSeconds, IsRoadBike = r.IsRoadBike, Eligibility = r.Eligibility, AvgSpeed = r.AvgSpeed, EventPosition = r.EventPosition };
    }
}