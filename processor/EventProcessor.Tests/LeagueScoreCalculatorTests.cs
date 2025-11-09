using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Orchestration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace EventProcessor.Tests
{
    public class LeagueScoreCalculatorIntegrationTests
    {
        private readonly Func<int, int> pointsForPosition = pos => 10 - (pos - 1);

        [Theory]
        [EventAutoData]
        public void EventScoring_ForLeagues_AssignsLeaguePositionsAndPoints(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var baseCompetitors = TestCompetitors.All.ToList();
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;

            // Create scorer with all calculators (including LeagueScoreCalculator once implemented)
            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            var competitors = baseCompetitors.ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);

            // Debug string you can inspect in Watch/Immediate window
            var debug = TestHelpers.RenderLeaguesDebugOutput(validRides, competitorVersions, new[] { 4 });
            _ = debug;

            // Assert
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertLeagueRideMatchesExpected(ridesForEvent, exp);
            }

            // Example expectations for Event 4 (Veterans league spread)
            AssertExpectedForEvent(4, new[]
            {
                (ClubNumber: 5001, Name: "Mark Anderson", Position: 1, Points: 60.0),
                (ClubNumber: 5002, Name: "Simon Bennett", Position: 2, Points: 55),
                (ClubNumber: 5101, Name: "Alice Kendall", Position: 1, Points: 60.0), // female league
                (ClubNumber: 5102, Name: "Sophie Lawrence", Position: 2, Points: 55),
            });
        }

        private static void AssertLeagueRideMatchesExpected(List<Ride> ridesForEvent, (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.LeaguePosition.Should().Be(expected.Position, $"expected LeaguePosition {expected.Position} for {context}");
            ride.LeaguePoints.Should().Be(expected.Points,     $"expected LeaguePoints {expected.Points} for {context}");
        }
    }
}
