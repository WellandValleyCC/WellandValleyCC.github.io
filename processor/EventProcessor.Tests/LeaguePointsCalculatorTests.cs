using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using EventProcessor.Tests.Helpers;

namespace EventProcessor.Tests
{
    public class LeaguePointsCalculatorTests
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
        public void AddSingleRide_AutoData_UsesStandardPreconditions(
            // injected by EventAutoData: standard pools
            List<Competitor> competitors,
            List<Ride> allRides,

            // auto-created values for the new ride (AutoFixture will supply sensible values),
            // you can change types to control what AutoFixture generates (e.g. int eventNumber)
            int eventNumber,
            int? clubNumber,
            bool isRoadBike,
            double totalSeconds)
        {
            // Arrange: build event-specific existing rides and add the new ride
            var existing = allRides.Where(r => r.EventNumber == eventNumber).Select(CloneRide).ToList();

            var numberOrName = clubNumber?.ToString() ?? $"Guest_{eventNumber}_Auto";
            var newRide = RideFactory.Create(
                eventNumber: eventNumber,
                numberOrName: numberOrName,
                name: clubNumber == null ? numberOrName : null,
                totalSeconds: totalSeconds,
                isRoadBike: isRoadBike,
                eligibility: RideEligibility.Valid,
                actualTime: TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss"));

            existing.Add(newRide);

            // Act: call the calculator (use PointsForPosition internally)
            // TO DO LeaguePointsCalculator.Calculate(existing, competitors, PointsForPosition);

            // Find the updated ride
            var updated = FindUpdatedRide(existing, newRide);

            // Assert: Ensure positions/points are set for the added rider.
            // For an AutoData test you might assert they are not null when the rider is eligible in that league.
            // Example: ensure at least the SeniorsPosition and SeniorsPoints have been set to something
            // when the competitor is a senior; otherwise, adapt asserts to the expected league membership.
            var comp = competitors.FirstOrDefault(c => c.ClubNumber == updated.ClubNumber);
            if (comp != null && comp.IsSenior && updated.Eligibility == RideEligibility.Valid && !updated.IsRoadBike)
            {
                Assert.NotNull(updated.SeniorsPosition);
                Assert.NotNull(updated.SeniorsPoints);
                Assert.Equal(PointsForPosition(updated.SeniorsPosition ?? 0), updated.SeniorsPoints);
            }

            // Additional assertions for Women, RoadBikeMen, etc. could follow the same pattern
            // using comp.IsFemale, updated.IsRoadBike and age flags.
        }

        private static Ride CloneRide(Ride r) => new Ride { EventNumber = r.EventNumber, ClubNumber = r.ClubNumber, Name = r.Name, ActualTime = r.ActualTime, TotalSeconds = r.TotalSeconds, IsRoadBike = r.IsRoadBike, Eligibility = r.Eligibility, AvgSpeed = r.AvgSpeed, EventPosition = r.EventPosition };

        private static Ride FindUpdatedRide(List<Ride> rides, Ride newRide)
        {
            if (newRide.ClubNumber != null)
                return rides.Single(r => r.ClubNumber == newRide.ClubNumber && r.EventNumber == newRide.EventNumber);

            return rides.Single(r => r.Name == newRide.Name && Math.Abs(r.TotalSeconds - newRide.TotalSeconds) < 0.001 && r.EventNumber == newRide.EventNumber);
        }
    }
}