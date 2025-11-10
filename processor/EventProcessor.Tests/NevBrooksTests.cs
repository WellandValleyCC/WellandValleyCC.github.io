using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Orchestration;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using Xunit;

namespace EventProcessor.Tests
{
    public class NevBrooksIntegrationTests
    {
        [Theory]
        [EventAutoData]
        public void EventScoring_ForNevBrooks_AssignsPositionsAndPoints(
            List<Ride> allRides,
            List<CalendarEvent> calendar)
        {
            // Arrange
            var baseCompetitors = TestCompetitors.All.ToList();
            int competitionYear = allRides.FirstOrDefault()?.CalendarEvent?.EventDate.Year ?? DateTime.Now.Year;

            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), competitionYear);

            var competitors = baseCompetitors.ToList();
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);

            var validRides = allRides
                .Where(r => r.ClubNumber.HasValue && competitorVersions.ContainsKey(r.ClubNumber.Value))
                .ToList();

            // Act
            scorer.ProcessAll(allRides, competitors, calendar);

            var ridesByEvent = TestHelpers.BuildRidesByEvent(validRides, onlyValidWithClubNumber: true);

            // Debug string for inspection
            var debug = TestHelpers.RenderNevBrooksDebugOutput(validRides, competitorVersions, ridesByEvent.Keys);
            _ = debug;

            // Assert
            void AssertExpectedForEvent(
                int evtNumber,
                (int ClubNumber, string Name, int? Position, double? Points,
                 double? Generated, double? Applied, double? Adjusted)[] expected)
            {
                var ridesForEvent = ridesByEvent[evtNumber];
                foreach (var exp in expected)
                {
                    var ride = ridesForEvent.Single(r => r.ClubNumber == exp.ClubNumber);
                    var context = $"[Club: {ride.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

                    ride.NevBrooksPosition.Should().Be(exp.Position, $"expected NevBrooksPosition {exp.Position} for {context}");
                    ride.NevBrooksPoints.Should().Be(exp.Points, $"expected NevBrooksPoints {exp.Points} for {context}");
                    ride.NevBrooksSecondsGenerated.Should().Be(exp.Generated, $"expected NevBrooksSecondsGenerated {exp.Generated} for {context}");
                    ride.NevBrooksSecondsApplied.Should().Be(exp.Applied, $"expected NevBrooksSecondsApplied {exp.Applied} for {context}");
                    ride.NevBrooksSecondsAdjustedTime.Should().Be(exp.Adjusted, $"expected NevBrooksSecondsAdjustedTime {exp.Adjusted} for {context}");
                }
            }

            // Event 5: baseline only, no scoring
            AssertExpectedForEvent(5, new[]
            {
                (6001, "Tom Harris",  (int?)null, (double?)null, (double?)505.0, (double?)null, (double?)null),
                (6002, "Emma Lewis",  null, null, 525.0, null, null),
                (6007, "Luke Quinn",  null, null, 605.0, null, null),
                (6008, "Hannah Reed", null, null, 485.0, null, null),
            });

            // Event 6 and 7: same 6000+ riders, but non-10m TTs --> all Nev Brooks fields null
            foreach (var evt in new[] { 6, 7 })
            {
                foreach (var ride in ridesByEvent[evt])
                {
                    var context = $"[Club: {ride.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

                    ride.NevBrooksPosition.Should().BeNull($"NevBrooksPosition should be null for non-10m TT {context}");
                    ride.NevBrooksPoints.Should().BeNull($"NevBrooksPoints should be null for non-10m TT {context}");
                    ride.NevBrooksSecondsGenerated.Should().BeNull($"NevBrooksSecondsGenerated should be null for non-10m TT {context}");
                    ride.NevBrooksSecondsApplied.Should().BeNull($"NevBrooksSecondsApplied should be null for non-10m TT {context}");
                    ride.NevBrooksSecondsAdjustedTime.Should().BeNull($"NevBrooksSecondsAdjustedTime should be null for non-10m TT {context}");
                }
            }

            // Event 8: scoring expectations (global points table: 60, 53, 48…)
            AssertExpectedForEvent(8, new[]
            {
                (6001, "Tom Harris", (int?)1, (double?)60.0, (double?)475.0, (double?)505.0, (double?)965.0),
                (6002, "Emma Lewis", 2, 53.0, 515.0, 525.0, 985.0),
                (6007, "Luke Quinn", 2, 53.0, 595.0, 605.0, 985.0),
                (6008, "Hannah Reed", 4, 48.0, 480.0, 485.0, 990.0),
            });
        }
    }
}
