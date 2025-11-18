using AutoFixture;
using AutoFixture.Xunit2;
using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubCore.Models.Enums;
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

            // 1) Build deterministic copies of canonical pools
            var competitors = TestCompetitors.All.Select(c => new Competitor
            {
                ClubNumber = c.ClubNumber,
                Surname = c.Surname,
                GivenName = c.GivenName,
                ClaimStatus = c.ClaimStatus,
                IsFemale = c.IsFemale,
                AgeGroup = c.AgeGroup,
                CreatedUtc = c.CreatedUtc,
                LastUpdatedUtc = c.LastUpdatedUtc,
                VetsBucket = c.VetsBucket
            }).ToList();

            var allRides = TestRides.All.Select(r => new Ride
            {
                EventNumber = r.EventNumber,
                ClubNumber = r.ClubNumber,
                Name = r.Name,
                TotalSeconds = r.TotalSeconds,
                IsRoadBike = r.IsRoadBike,
                Status = r.Status,
                AvgSpeed = r.AvgSpeed,
                EventRank = r.EventRank
            }).ToList();

            // 2) Build CalendarEvents that align to the event numbers in allRides
            // Use a recent rolling start so 30‑day histories span several events
            var firstEventDateUtc = DateTime.UtcNow.Date.AddDays(-35); // midnight UTC 35 days ago

            var evening10Events = new HashSet<int> { 5, 8 }; // standard 10m TTs
            var milesByEvent = new Dictionary<int, double>
            {
                { 6, 9.5 }, // non‑standard
                { 7, 25.0 } // non‑standard
            };

            var calendarEvents = TestCalendarEvents.CreateLookupForRides(
                allRides,
                firstEventDateUtc,
                interval: TimeSpan.FromDays(30),
                milesByEvent: milesByEvent,
                evening10Events: evening10Events);

            var calendar = calendarEvents.Values.ToList();

            // 3) Ensure every Ride has CalendarEvent populated so we have a date
            foreach (var r in allRides)
            {
                if (calendarEvents.TryGetValue(r.EventNumber, out var ev))
                {
                    r.CalendarEvent = ev;
                }
                else
                {
                    // guard for unexpected event numbers used by EventAutoData
                    throw new InvalidOperationException($"No CalendarEvent for EventNumber {r.EventNumber}");
                }
            }

            // 4) Register the canonical pools and helpers on the fixture
            // Register collections so tests can accept List<Competitor>, List<Ride>, List<CalendarEvent>
            fixture.Register(() => competitors);
            fixture.Register(() => allRides);
            fixture.Register(() => calendarEvents);
            fixture.Register(() => calendar);

            // Register an eventNumber chosen from the existing event numbers in allRides
            var distinctEventNumbers = allRides.Select(r => r.EventNumber).Distinct().OrderBy(n => n).ToArray();
            int chosenEventIndex = 0;
            int chosenEventNumber = distinctEventNumbers.Length > 0 ? distinctEventNumbers[chosenEventIndex] : 1;
            fixture.Register(() => chosenEventNumber);

            // Register a clubNumber that is in competitors but not present in the chosen event's rides
            fixture.Register<int?>(() =>
            {
                var usedClubNumbers = new HashSet<int>(allRides
                    .Where(r => r.EventNumber == chosenEventNumber && r.ClubNumber.HasValue)
                    .Select(r => r.ClubNumber!.Value));

                var candidates = competitors
                    .Select(c => c.ClubNumber)
                    .Where(n => !usedClubNumbers.Contains(n))
                    .Distinct()
                    .OrderBy(n => n)
                    .ToArray();

                if (candidates.Length == 0) return (int?)null;
                return (int?)candidates[0];
            });

            // Register RideFactory helper so AutoFixture can create Ride if requested
            fixture.Register<Func<int, string?, string?, double, bool, RideStatus, string?, Ride>>(
                () => (eventNumber, numberOrName, name, totalSeconds, isRoadBike, eligibility, actualTime) =>
                    RideFactory.Create(eventNumber, numberOrName, name, totalSeconds, isRoadBike, eligibility, actualTime)
            );

            return fixture;
        }
    }
}
