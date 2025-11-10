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
            var debug = TestHelpers.RenderNevBrooksDebugOutput(validRides, competitorVersions, new[] { 5, 8 });
            _ = debug;

            // Assert
            void AssertExpectedForEvent(int evtNumber, (int ClubNumber, string Name, int Position, double Points)[] expected)
            {
                ridesByEvent.TryGetValue(evtNumber, out var ridesForEvent);
                ridesForEvent = ridesForEvent ?? new List<Ride>();
                foreach (var exp in expected)
                    AssertNevBrooksRideMatchesExpected(ridesForEvent, exp);
            }

            foreach (var evt in ridesByEvent.Keys.Where(n => n != 5 && n != 8))
            {
                foreach (var ride in ridesByEvent[evt])
                {
                    var context = $"[Club: {ride.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

                    bool eligibleTen = ride.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary }
                                       && ride.Eligibility == RideEligibility.Valid
                                       && ride.CalendarEvent?.Miles == 10.0;

                    if (eligibleTen)
                    {
                        // Baseline only: Generated set, others null
                        ride.NevBrooksSecondsGenerated.Should().NotBeNull($"Generated must be set for eligible 10m TT {context}");
                        ride.NevBrooksSecondsApplied.Should().BeNull($"Applied should be null for first/unsuccessful 10m TT {context}");
                        ride.NevBrooksSecondsAdjustedTime.Should().BeNull($"AdjustedTime should be null for baseline {context}");
                        ride.NevBrooksPosition.Should().BeNull($"Position should be null for baseline {context}");
                        ride.NevBrooksPoints.Should().BeNull($"Points should be null for baseline {context}");
                    }
                    else
                    {
                        // Ineligible or non?10m: all null
                        ride.NevBrooksSecondsGenerated.Should().BeNull($"Generated should be null for ineligible/non?10m ride {context}");
                        ride.NevBrooksSecondsApplied.Should().BeNull($"Applied should be null for ineligible/non?10m ride {context}");
                        ride.NevBrooksSecondsAdjustedTime.Should().BeNull($"AdjustedTime should be null for ineligible/non?10m ride {context}");
                        ride.NevBrooksPosition.Should().BeNull($"Position should be null for ineligible/non?10m ride {context}");
                        ride.NevBrooksPoints.Should().BeNull($"Points should be null for ineligible/non?10m ride {context}");
                    }
                }
            }

            // Example expectations for Event 8 (second 10m TT)
            AssertExpectedForEvent(8, new[]
            {
                (ClubNumber: 6001, Name: "Tom Harris", Position: 1, Points: 60.0),
                (ClubNumber: 6002, Name: "Emma Lewis", Position: 2, Points: 53.0),
                (ClubNumber: 6007, Name: "Luke Quinn", Position: 2, Points: 53.0),
                (ClubNumber: 6008, Name: "Hannah Reed", Position: 4, Points: 48.0),
            });

            // Assert negative cases: all other events should have NevBrooks properties null
            foreach (var evt in ridesByEvent.Keys.Where(n => n != 5 && n != 8))
            {
                foreach (var ride in ridesByEvent[evt])
                {
                    var context = $"[Club: {ride.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";
                    AssertNevBrooksRideIsNull(ride, context);
                }
            }

            foreach (var evt in new[] { 5, 8 })
            {
                if (ridesByEvent.TryGetValue(evt, out var ridesForEvent))
                {
                    foreach (var ride in ridesForEvent)
                    {
                        var claimStatus = ride.Competitor?.ClaimStatus;
                        if (claimStatus == ClaimStatus.SecondClaim || claimStatus == null)
                        {
                            var context = $"[Club: {ride.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";
                            AssertNevBrooksRideIsNull(ride, context);
                        }
                    }
                }
            }
        }

        private static void AssertNevBrooksRideMatchesExpected(
            List<Ride> ridesForEvent,
            (int ClubNumber, string Name, int Position, double Points) expected)
        {
            var ride = ridesForEvent.SingleOrDefault(r => r.ClubNumber == expected.ClubNumber);
            ride.Should().NotBeNull($"expected a ride for club {expected.ClubNumber} ({expected.Name})");

            var context = $"[Club: {ride!.ClubNumber}, Name: {ride.Name}, Event: {ride.EventNumber}]";

            ride.NevBrooksPosition.Should().Be(expected.Position, $"expected NevBrooksPosition {expected.Position} for {context}");
            ride.NevBrooksPoints.Should().Be(expected.Points, $"expected NevBrooksPoints {expected.Points} for {context}");
        }

        private static void AssertNevBrooksRideIsNull(Ride ride, string context)
        {
            ride.NevBrooksPosition.Should().BeNull($"NevBrooksPosition should be null for {context}");
            ride.NevBrooksPoints.Should().BeNull($"NevBrooksPoints should be null for {context}");
            ride.NevBrooksSecondsGenerated.Should().BeNull($"NevBrooksSecondsGenerated should be null for {context}");
            ride.NevBrooksSecondsApplied.Should().BeNull($"NevBrooksSecondsApplied should be null for {context}");
            ride.NevBrooksSecondsAdjustedTime.Should().BeNull($"NevBrooksSecondsAdjustedTime should be null for {context}");
        }
    }
}
