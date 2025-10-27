using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventProcessor.Tests.Helpers
{
    public class EventAutoDataAttribute : AutoDataAttribute
    {
        public EventAutoDataAttribute() : base(CreateFixture) { }

        private static IFixture CreateFixture()
        {
            var fixture = new Fixture();

            // Create local clones of canonical pools so we can inspect them to derive other registrations
            var competitors = TestCompetitors.All.Select(c => new Competitor
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

            var allRides = TestRides.All.Select(r => new Ride
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
            }).ToList();

            // Register the canonical pools so tests can receive them as parameters
            fixture.Register(() => competitors);
            fixture.Register(() => allRides);

            // Register PointsForPosition
            fixture.Register<Func<int, int>>(() => (pos) => LeaguePointsCalculatorTests.PointsForPosition(pos));

            // Register an eventNumber chosen from the existing event numbers in allRides
            var distinctEventNumbers = allRides.Select(r => r.EventNumber).Distinct().OrderBy(n => n).ToArray();
            // choose deterministic index (you could pick random seeded index instead)
            int chosenEventIndex = 0;
            int chosenEventNumber = distinctEventNumbers.Length > 0 ? distinctEventNumbers[chosenEventIndex] : 1;
            fixture.Register(() => chosenEventNumber);

            // Register a clubNumber that is in competitors but not present in the chosen event's rides
            fixture.Register<int?>(() =>
            {
                // Collect club numbers already used in the chosen event (exclude guests which are represented on Ride not Competitor)
                var usedClubNumbers = new HashSet<int>(allRides
                    .Where(r => r.EventNumber == chosenEventNumber && r.ClubNumber.HasValue)
                    .Select(r => r.ClubNumber!.Value));

                // Candidates: competitors with ClubNumber not present in the event
                var candidates = competitors
                    .Select(c => c.ClubNumber)          // ClubNumber is non-nullable int
                    .Where(n => !usedClubNumbers.Contains(n))
                    .Distinct()
                    .OrderBy(n => n)
                    .ToArray();

                if (candidates.Length == 0) return (int?)null; // fall back to guest

                return (int?)candidates[0]; // deterministic choice; swap for seeded random if desired
            });

            // Optional: register RideFactory helper so AutoFixture can create Ride if requested
            fixture.Register<Func<int, string?, string?, double, bool, RideEligibility, string?, Ride>>(
                () => (eventNumber, numberOrName, name, totalSeconds, isRoadBike, eligibility, actualTime) =>
                    RideFactory.Create(eventNumber, numberOrName, name, totalSeconds, isRoadBike, eligibility, actualTime)
            );

            return fixture;
        }
    }
}
