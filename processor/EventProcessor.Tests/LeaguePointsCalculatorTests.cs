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
            // Arrange: clone injected pools to avoid mutating shared fixtures
            var competitorsCopy = competitors.Select(c => new Competitor
            {
                ClubNumber = c.ClubNumber,
                Surname = c.Surname,
                GivenName = c.GivenName,
                ClaimStatus = c.ClaimStatus,
                IsFemale = c.IsFemale,
                IsJuvenile = c.IsJuvenile,
                IsJunior = c.IsJunior,
                IsSenior = c.IsSenior,
                IsVeteran = c.IsVeteran,
                CreatedUtc = c.CreatedUtc,
                LastUpdatedUtc = c.LastUpdatedUtc
            }).ToList();

            var existingRides = allRides.Where(r => r.EventNumber == eventNumber)
                                        .Select(r => new Ride
                                        {
                                            EventNumber = r.EventNumber,
                                            ClubNumber = r.ClubNumber,
                                            Name = r.Name,
                                            ActualTime = r.ActualTime,
                                            TotalSeconds = r.TotalSeconds,
                                            IsRoadBike = r.IsRoadBike,
                                            Eligibility = r.Eligibility,
                                            AvgSpeed = r.AvgSpeed,
                                            EventPosition = r.EventPosition
                                        })
                                        .ToList();

            // Build the new ride. If clubNumber is provided ensure it's a real competitor
            if (clubNumber.HasValue && !competitorsCopy.Any(c => c.ClubNumber == clubNumber.Value))
            {
                throw new InvalidOperationException($"Fixture supplied clubNumber {clubNumber.Value} that is not present in competitors");
            }

            var numberOrName = clubNumber?.ToString() ?? $"Guest_{eventNumber}_Auto";
            var newRide = RideFactory.Create(
                eventNumber: eventNumber,
                numberOrName: numberOrName,
                name: clubNumber == null ? numberOrName : null,
                totalSeconds: totalSeconds,
                isRoadBike: isRoadBike,
                eligibility: RideEligibility.Valid,
                actualTime: TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\\:mm\\:ss"));

            existingRides.Add(newRide);

            // Act: call the calculator (use the PointsForPosition helper)
            // TODO: LeaguePointsCalculator.Calculate(existingRides, competitorsCopy, PointsForPosition);

            // Find the updated ride
            var updatedRide = newRide.ClubNumber != null
                ? existingRides.Single(r => r.EventNumber == eventNumber && r.ClubNumber == newRide.ClubNumber)
                : existingRides.Single(r => r.EventNumber == eventNumber && r.Name == newRide.Name && Math.Abs(r.TotalSeconds - newRide.TotalSeconds) < 0.001);

            // Assert: when competitor exists and eligibility is valid, verify positions/points use the table
            if (updatedRide.Eligibility == RideEligibility.Valid && updatedRide.ClubNumber.HasValue)
            {
                var comp = competitorsCopy.Single(c => c.ClubNumber == updatedRide.ClubNumber.Value);

                if (comp.IsJuvenile)
                {
                    Assert.NotNull(updatedRide.JuvenilesPosition);
                    Assert.Equal(PointsForPosition(updatedRide.JuvenilesPosition ?? 0), updatedRide.JuvenilesPoints);
                }

                if (comp.IsJunior)
                {
                    Assert.NotNull(updatedRide.JuniorsPosition);
                    Assert.Equal(PointsForPosition(updatedRide.JuniorsPosition ?? 0), updatedRide.JuniorsPoints);
                }

                if (comp.IsSenior)
                {
                    Assert.NotNull(updatedRide.SeniorsPosition);
                    Assert.Equal(PointsForPosition(updatedRide.SeniorsPosition ?? 0), updatedRide.SeniorsPoints);
                }

                if (comp.IsVeteran)
                {
                    Assert.NotNull(updatedRide.VeteransPosition);
                    Assert.Equal(PointsForPosition(updatedRide.VeteransPosition ?? 0), updatedRide.VeteransPoints);
                }

                if (comp.IsFemale)
                {
                    Assert.NotNull(updatedRide.WomenPosition);
                    Assert.Equal(PointsForPosition(updatedRide.WomenPosition ?? 0), updatedRide.WomenPoints);
                }

                if (updatedRide.IsRoadBike && !comp.IsFemale)
                {
                    Assert.NotNull(updatedRide.RoadBikeMenPosition);
                    Assert.Equal(PointsForPosition(updatedRide.RoadBikeMenPosition ?? 0), updatedRide.RoadBikeMenPoints);
                }

                if (updatedRide.IsRoadBike && comp.IsFemale)
                {
                    Assert.NotNull(updatedRide.RoadBikeWomenPosition);
                    Assert.Equal(PointsForPosition(updatedRide.RoadBikeWomenPosition ?? 0), updatedRide.RoadBikeWomenPoints);
                }
            }
            else
            {
                // For guests or non-valid eligibility you can assert fields are null or zero as appropriate
                Assert.True(true); // no-op placeholder: replace with your guest/DNS expectations if desired
            }
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