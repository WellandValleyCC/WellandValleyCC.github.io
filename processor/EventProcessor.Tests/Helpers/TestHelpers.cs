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
        public static Dictionary<int, List<Ride>> BuildRidesByEvent(IEnumerable<Ride> rides, bool onlyValidWithClubNumber = true)
        {
            var q = rides ?? Enumerable.Empty<Ride>();
            if (onlyValidWithClubNumber)
            {
                q = q.Where(r => r.Eligibility == RideEligibility.Valid && r.ClubNumber.HasValue);
            }

            return q.GroupBy(r => r.EventNumber)
                    .ToDictionary(g => g.Key, g => g.ToList());
        }

        // Render debug output: uses the same resolution logic as assertions
        public static string RenderJuvenileDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();

            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

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
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

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

        public static string RenderJuniorsDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();
            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                var eligible = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        return latest?.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!eligible.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual juniors results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.JuniorsPosition.HasValue ? ride.JuniorsPosition.Value.ToString() : "null";
                    var pts = ride.JuniorsPoints;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup}");
                }
            }

            return sb.ToString();
        }

        // Render debug output: uses the same resolution logic as assertions
        public static string RenderSeniorsDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();

            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                var allAgeGroups = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        if (latest == null) return false;

                        return latest.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!allAgeGroups.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual seniors results:");
                foreach (var ride in allAgeGroups)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.SeniorsPosition.HasValue ? ride.SeniorsPosition.Value.ToString() : "null";
                    var pts = ride.SeniorsPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            return sb.ToString();
        }

        public static string RenderWomenDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();
            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                var eligible = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        return latest?.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!eligible.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual women results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.WomenPosition.HasValue ? ride.WomenPosition.Value.ToString() : "null";
                    var pts = ride.WomenPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            return sb.ToString();
        }

        public static string RenderRoadBikeMenDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();
            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                var eligible = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        return latest?.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!eligible.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual road bike men results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.RoadBikeMenPosition.HasValue ? ride.RoadBikeMenPosition.Value.ToString() : "null";
                    var pts = ride.RoadBikeMenPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            return sb.ToString();
        }

        public static string RenderRoadBikeWomenDebugOutput(
            IEnumerable<Ride> allRides,
            IReadOnlyDictionary<int, List<Competitor>> competitorVersionsByClubNumber,
            IEnumerable<int> eventNumbers)
        {
            var sb = new StringBuilder();
            var ridesByEvent = BuildRidesByEvent(allRides, onlyValidWithClubNumber: true);

            foreach (var evt in eventNumbers.OrderBy(n => n))
            {
                if (!ridesByEvent.TryGetValue(evt, out var ridesForEvent) || !ridesForEvent.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                var eligible = ridesForEvent
                    .Where(r =>
                    {
                        if (!r.ClubNumber.HasValue) return false;
                        if (!competitorVersionsByClubNumber.TryGetValue(r.ClubNumber.Value, out var versions)) return false;

                        var eventDateUtc = DateTime.SpecifyKind(r.CalendarEvent?.EventDate ?? DateTime.MinValue, DateTimeKind.Utc);
                        var latest = GetLatestCompetitorForEvent(versions, eventDateUtc);
                        return latest?.ClaimStatus != ClaimStatus.SecondClaim;
                    })
                    .OrderBy(r => r.TotalSeconds)
                    .ToList();

                if (!eligible.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual road bike women results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.RoadBikeWomenPosition.HasValue ? ride.RoadBikeWomenPosition.Value.ToString() : "null";
                    var pts = ride.RoadBikeWomenPoints;
                    sb.AppendLine($"(ClubNumber: {club}, Name: \"{name}\", Position: {pos}, Points: {pts}),");
                }
            }

            return sb.ToString();
        }
    }
}
