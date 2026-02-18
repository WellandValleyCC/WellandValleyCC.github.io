using ClubCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubSiteGenerator.Services.Hydration
{
    public static class RideHydrator
    {
        public static void AttachCalendarEvents(
            IEnumerable<Ride> rides,
            IEnumerable<CalendarEvent> calendar)
        {
            var eventsByNumber = calendar.ToDictionary(e => e.EventNumber);

            foreach (var ride in rides)
            {
                if (!eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                {
                    throw new InvalidOperationException(
                        $"Ride for event number {ride.EventNumber} has no matching CalendarEvent");
                }

                ride.CalendarEvent = ev;
            }
        }

        public static void AttachCompetitors(
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar)
        {
            var competitorsByClub = competitors
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(c => c.CreatedUtc).ToList()
                );

            var eventsByNumber = calendar.ToDictionary(e => e.EventNumber);

            foreach (var ride in rides)
            {
                if (!ride.ClubNumber.HasValue)
                    continue;

                if (!eventsByNumber.TryGetValue(ride.EventNumber, out var ev))
                    throw new InvalidOperationException($"Ride {ride.EventNumber} has no CalendarEvent");

                if (!competitorsByClub.TryGetValue(ride.ClubNumber.Value, out var snapshots))
                {
                    throw new InvalidOperationException(
                        $"Ride '{ride.Name ?? "Name=<null>"}' (Event {ride.EventNumber}, Club {ride.ClubNumber}) has no matching Competitor");
                }

                var eventDateUtc = ev.EventDate;

                Competitor? chosen = snapshots
                    .Where(c => c.CreatedUtc <= eventDateUtc)
                    .LastOrDefault();

                if (chosen == null)
                {
                    throw new InvalidOperationException(
                        $"Ride '{ride.Name ?? "Name=<null>"}' (Event {ride.EventNumber}, Club {ride.ClubNumber}) has no valid Competitor snapshot before {eventDateUtc:u}");
                }

                ride.Competitor = chosen;
            }
        }

        public static void AttachRoundRobinRiders(
            IEnumerable<Ride> rides,
            IEnumerable<RoundRobinRider> rrRiders)
        {
            var rrByName = rrRiders
                .GroupBy(r => r.DecoratedName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var missing = new List<(string Name, string Club)>();

            foreach (var ride in rides)
            {
                if (ride.ClubNumber != null) continue;
                if (string.IsNullOrWhiteSpace(ride.RoundRobinClub)) continue;
                if (string.Equals(ride.RoundRobinClub, "WVCC", StringComparison.OrdinalIgnoreCase)) continue;

                if (string.IsNullOrWhiteSpace(ride.Name))
                {
                    throw new InvalidOperationException(
                        $"Ride in Event {ride.EventNumber} (Club '{ride.RoundRobinClub}') " +
                        "is marked as RoundRobin but has no rider Name.");
                }

                var key = ride.Name.Trim();

                if (!rrByName.TryGetValue(key, out var rr))
                {
                    missing.Add((ride.Name ?? "Name=<null>", ride.RoundRobinClub));
                    continue;
                }

                ride.RoundRobinRider = rr;
            }

            if (missing.Any())
            {
                var details = string.Join(
                    Environment.NewLine,
                    missing
                        .Distinct()
                        .Select(m => $"  - '{m.Name}' ({m.Club})"));

                throw new InvalidOperationException(
                    "Round Robin hydration failed. Missing riders:" +
                    Environment.NewLine +
                    details);
            }
        }

        public static void AttachSyntheticWvccRoundRobinRiders(
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors)
        {
            // Build synthetic riders keyed by negative competitor ID
            var synthetic = competitors
                .ToDictionary(
                    c => -c.Id,
                    c => new RoundRobinRider
                    {
                        Id = -c.Id,
                        Name = $"{c.GivenName} {c.Surname}",
                        RoundRobinClub = "WVCC",
                        IsFemale = c.IsFemale
                    });

            foreach (var ride in rides)
            {
                if (!string.Equals(ride.RoundRobinClub, "WVCC", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (ride.Competitor == null)
                    throw new InvalidOperationException(
                        $"WVCC ride {ride.Id} has no Competitor object.");

                ride.RoundRobinRider = synthetic[-ride.Competitor.Id];
            }
        }
    }
}
