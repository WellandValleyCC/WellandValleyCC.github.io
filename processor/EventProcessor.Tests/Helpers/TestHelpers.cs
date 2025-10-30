using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EventProcessor.Tests.Helpers
{
    internal static class TestHelpers
    {
        // Build a lookup of competitor versions per club number, versions ordered ascending by CreatedUtc
        public static IReadOnlyDictionary<int, List<Competitor>> CreateCompetitorVersionsLookup(IEnumerable<Competitor> competitors)
        {
            return competitors
                .Where(c => c != null && c.ClubNumber != 0)
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(c => c.CreatedUtc).ToList());
        }

        // Return the latest competitor version where CreatedUtc <= eventDateUtc, or null if none
        public static Competitor? GetLatestCompetitorForEvent(IReadOnlyList<Competitor>? versions, DateTime eventDateUtc)
        {
            if (versions == null || versions.Count == 0) return null;

            var eventUtc = DateTime.SpecifyKind(eventDateUtc, DateTimeKind.Utc);

            return versions
                .Where(v => v.CreatedUtc <= eventUtc)
                .OrderByDescending(v => v.CreatedUtc)
                .FirstOrDefault();
        }

        // Group rides by event number (only valid rides with club numbers by default)
        public static Dictionary<int, List<Ride>> BuildRidesByEvent(IEnumerable<Ride> rides, bool onlyValidWithClub = true)
        {
            var q = rides ?? Enumerable.Empty<Ride>();
            if (onlyValidWithClub)
            {
                q = q.Where(r => r.Eligibility == RideEligibility.Valid && r.ClubNumber.HasValue);
            }

            return q.GroupBy(r => r.EventNumber)
                    .ToDictionary(g => g.Key, g => g.ToList());
        }

        // Render debug output: uses the same resolution logic as assertions
        public static string RenderJuvenileDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClub,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();

            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClub: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no juvenile rides (or none eligible)");
                    continue;
                }

                var juveniles = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClub.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        if (latest == null) return false;

                        return latest.IsJuvenile && latest.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!juveniles.Any())
                {
                    sb.AppendLine($"// Event {evt}: no juvenile rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual juvenile results:");
                foreach (var ride in juveniles)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.JuvenilesPosition.HasValue ? ride.JuvenilesPosition.Value.ToString() : "null";
                    var pts = ride.JuvenilesPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            return sb.ToString();
        }
    }
}
