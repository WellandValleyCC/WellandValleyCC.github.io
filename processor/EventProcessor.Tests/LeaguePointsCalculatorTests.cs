using AutoFixture.Xunit2;
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
        [InlineAutoData(AgeGroup.IsSenior, false, false, 830.0)] // senior male non-roadbike
        [InlineAutoData(AgeGroup.IsSenior, true, true, 835.0)]  // senior female roadbike
        [InlineAutoData(AgeGroup.IsJuvenile, false, false, 900.0)]
        [EventAutoData]
        public void AddSingleRide_LeagueCalculation_ScoresNewRideCorrectly(
            List<Competitor> competitors,
            List<Ride> allRides,
            int eventNumber,
            AgeGroup ageGroup,
            bool isFemale,
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
                                        .Select(CloneRide)
                                        .ToList();

            // Find or create a competitor that matches the requested AgeGroup and sex and is not already in the event
            bool MatchesGroup(Competitor c)
            {
                return (ageGroup == AgeGroup.IsJuvenile && c.IsJuvenile)
                    || (ageGroup == AgeGroup.IsJunior && c.IsJunior)
                    || (ageGroup == AgeGroup.IsSenior && c.IsSenior)
                    || (ageGroup == AgeGroup.IsVeteran && c.IsVeteran);
            }

            var usedClubNumbers = new HashSet<int>(existingRides.Where(r => r.ClubNumber.HasValue).Select(r => r.ClubNumber!.Value));

            var candidate = competitorsCopy
                .Where(c => MatchesGroup(c) && c.IsFemale == isFemale && !usedClubNumbers.Contains(c.ClubNumber))
                .OrderBy(c => c.ClubNumber)
                .FirstOrDefault();

            if (candidate == null)
            {
                // create a synthetic competitor with a new club number that won't collide
                int nextClub = competitorsCopy.Max(c => c.ClubNumber) + 1;
                candidate = new Competitor
                {
                    ClubNumber = nextClub,
                    GivenName = $"Test{nextClub}",
                    Surname = $"User{nextClub}",
                    ClaimStatus = ClaimStatus.FirstClaim,
                    IsFemale = isFemale,
                    IsJuvenile = ageGroup == AgeGroup.IsJuvenile,
                    IsJunior = ageGroup == AgeGroup.IsJunior,
                    IsSenior = ageGroup == AgeGroup.IsSenior,
                    IsVeteran = ageGroup == AgeGroup.IsVeteran,
                    CreatedUtc = DateTime.UtcNow,
                    LastUpdatedUtc = DateTime.UtcNow
                };
                competitorsCopy.Add(candidate);
            }

            // Build the new ride for that competitor
            var numberOrName = candidate.ClubNumber.ToString();
            var safeSeconds = double.IsNaN(totalSeconds) || double.IsInfinity(totalSeconds) || totalSeconds < 0 ? 900.0 : totalSeconds;
            var newRide = RideFactory.Create(
                eventNumber: eventNumber,
                numberOrName: numberOrName,
                name: null,
                totalSeconds: safeSeconds,
                isRoadBike: isRoadBike,
                eligibility: RideEligibility.Valid,
                actualTime: TimeSpan.FromSeconds(safeSeconds).ToString(@"hh\:mm\:ss"));

            existingRides.Add(newRide);

            // Act
            // TO DO LeaguePointsCalculator.Calculate(existingRides, competitorsCopy, PointsForPosition);

            // Find the updated ride
            var updatedRide = existingRides.Single(r => r.EventNumber == eventNumber && r.ClubNumber == candidate.ClubNumber);

            // Determine which category fields should be asserted
            bool expectJuvenile = ageGroup == AgeGroup.IsJuvenile;
            bool expectJunior = ageGroup == AgeGroup.IsJunior;
            bool expectSenior = ageGroup == AgeGroup.IsSenior;
            bool expectVeteran = ageGroup == AgeGroup.IsVeteran;
            bool expectWomen = isFemale;
            bool expectRoad = isRoadBike;

            // Helper that asserts when there's at least one eligible rider in category; otherwise asserts null/0 as appropriate
            void AssertCategory(Func<Ride, int?> positionSelector, Func<Ride, int?> pointsSelector, Func<Ride, bool> membershipPredicate, bool expect)
            {
                var categoryRides = existingRides
                    .Where(r => r.EventNumber == eventNumber && r.Eligibility == RideEligibility.Valid && r.ClubNumber.HasValue)
                    .Where(r =>
                    {
                        var rc = competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber!.Value);
                        return membershipPredicate(r);
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (expect && categoryRides.Count > 0)
                {
                    Assert.True(positionSelector(updatedRide).HasValue, "Expected position to be set for category");
                    int pos = positionSelector(updatedRide)!.Value;
                    int pts = pointsSelector(updatedRide) ?? 0;
                    Assert.Equal(PointsForPosition(pos), pts);
                }
                else
                {
                    Assert.False(positionSelector(updatedRide).HasValue, "Expected position to be null when category has no eligible riders or not expected");
                    Assert.Equal(0, pointsSelector(updatedRide) ?? 0);
                }
            }

            AssertCategory(
                r => r.JuvenilesPosition, 
                r => r.JuvenilesPoints,
                r => r.ClubNumber.HasValue 
                    && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsJuvenile, 
                expectJuvenile);

            AssertCategory(r => r.JuniorsPosition, r => r.JuniorsPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsJunior, expectJunior);

            AssertCategory(r => r.SeniorsPosition, r => r.SeniorsPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsSenior && !r.IsRoadBike, expectSenior && !expectRoad);

            AssertCategory(r => r.VeteransPosition, r => r.VeteransPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsVeteran, expectVeteran);

            AssertCategory(r => r.WomenPosition, r => r.WomenPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsFemale, expectWomen);

            AssertCategory(r => r.RoadBikeMenPosition, r => r.RoadBikeMenPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsSenior && r.IsRoadBike && !competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsFemale,
                expectSenior && expectRoad && !expectWomen);

            AssertCategory(r => r.RoadBikeWomenPosition, r => r.RoadBikeWomenPoints,
                r => r.ClubNumber.HasValue && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsSenior && r.IsRoadBike && competitorsCopy.Single(c => c.ClubNumber == r.ClubNumber.Value).IsFemale,
                expectSenior && expectRoad && expectWomen);
        }

        private static Ride CloneRide(Ride r) => new Ride { EventNumber = r.EventNumber, ClubNumber = r.ClubNumber, Name = r.Name, ActualTime = r.ActualTime, TotalSeconds = r.TotalSeconds, IsRoadBike = r.IsRoadBike, Eligibility = r.Eligibility, AvgSpeed = r.AvgSpeed, EventPosition = r.EventPosition };
    }
}