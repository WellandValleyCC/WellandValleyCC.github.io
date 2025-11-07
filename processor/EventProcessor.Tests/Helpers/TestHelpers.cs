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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.JuvenilesPosition.HasValue ? ride.JuvenilesPosition.Value.ToString() : "null";
                    var pts = ride.JuvenilesPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.JuniorsPosition.HasValue ? ride.JuniorsPosition.Value.ToString() : "null";
                    var pts = ride.JuniorsPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.SeniorsPosition.HasValue ? ride.SeniorsPosition.Value.ToString() : "null";
                    var pts = ride.SeniorsPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.WomenPosition.HasValue ? ride.WomenPosition.Value.ToString() : "null";
                    var pts = ride.WomenPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.RoadBikeMenPosition.HasValue ? ride.RoadBikeMenPosition.Value.ToString() : "null";
                    var pts = ride.RoadBikeMenPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
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

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.RoadBikeWomenPosition.HasValue ? ride.RoadBikeWomenPosition.Value.ToString() : "null";
                    var pts = ride.RoadBikeWomenPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var line = $"(ClubNumber: {club}, Name: \"{name}\",";
                    line = line.PadRight(46); // ensures "Position" starts at column 42
                    sb.AppendLine($"{line} Position: {pos}, Points: {pts}), // {totalSeconds}s {claimStatus} {ageGroup} {gender} {bikeType}");
                }
            }

            return sb.ToString();
        }

        public static string RenderVeteransDebugOutput(
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
                    .OrderBy(r => r.HandicapTotalSeconds)
                    .ToList();

                if (!eligible.Any())
                {
                    sb.AppendLine($"// Event {evt}: no rides (or none eligible)");
                    continue;
                }

                sb.AppendLine($"// Event {evt} actual results:");
                foreach (var ride in eligible)
                {
                    var club = ride.ClubNumber!.Value;
                    var name = (ride.Name ?? string.Empty).Replace("\"", "\\\"");
                    var pos = ride.VeteransPosition.HasValue ? ride.VeteransPosition.Value.ToString() : "null";
                    var pts = ride.VeteransPoints;
                    var gender = ride.Gender;
                    var bikeType = ride.RoadBikeIndicator;
                    var ageGroup = ride.AgeGroupDisplay;
                    var claimStatus = ride.ClaimStatusDisplay;
                    var totalSeconds = ride.TotalSeconds;
                    var vetsBucket = ride.Competitor?.VetsBucket.HasValue == true ? ride.Competitor.VetsBucket.Value.ToString() : "n/a";
                    var handicapSeconds = ride.HandicapSeconds?.ToString() ?? "n/a";
                    var handicapTotalSeconds = ride.HandicapTotalSeconds?.ToString() ?? "n/a";

                    // Align the name column outside the quotes
                    string line = string.Format(
                        "(ClubNumber: {0,-4}, Name: \"{1}\",{2}Position: {3,-4}, Points: {4,-5})",
                        club,
                        name,
                        new string(' ', Math.Max(1, 22 - name.Length)), // pad spaces after the closing quote
                        pos,
                        pts);

                    sb.AppendLine(
                        $"{line} // {totalSeconds,4}s VetsBucket:{vetsBucket,-4} AAT:{handicapSeconds,-4} HandicapTotalSeconds:{handicapTotalSeconds,-4} {claimStatus,-10} {ageGroup,-8} {gender,-6} {bikeType}");
                }
            }

            return sb.ToString();
        }

    }
}
