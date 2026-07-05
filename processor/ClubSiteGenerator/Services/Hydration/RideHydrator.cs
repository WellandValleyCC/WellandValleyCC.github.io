using ClubCore.Models;

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
            IEnumerable<RoundRobinRider> rrRiders,
            int competitionYear)
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
                    // NEW: Skip Guest riders ONLY for 2025 — they will be attached synthetically later
                    if (competitionYear == 2025 &&
                        string.Equals(ride.RoundRobinClub, "Guest", StringComparison.OrdinalIgnoreCase))
                        continue;

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
            IEnumerable<Competitor> competitors,
            IEnumerable<RoundRobinRider> rrRiders)
        {
            var synthetic = BuildSyntheticWvccRiders(competitors);
            var wvccSecondClaim = BuildWvccSecondClaimLookup(rrRiders);

            foreach (var ride in rides)
            {
                if (!IsWvccRide(ride))
                    continue;

                if (ride.Competitor == null)
                    throw new InvalidOperationException(
                        $"WVCC ride {ride.Id} has no Competitor object.");

                var rr = ResolveWvccRoundRobinRider(ride, synthetic, wvccSecondClaim);

                ride.RoundRobinRider = rr;
                ride.Name = rr.Name; // decorated or synthetic
            }
        }

        private static Dictionary<int, RoundRobinRider> BuildSyntheticWvccRiders(
            IEnumerable<Competitor> competitors)
        {
            // First‑claim WVCC riders never appear in the RR CSV,
            // so we synthesise RoundRobinRider objects for them.
            return competitors.ToDictionary(
                c => -c.Id,
                c => new RoundRobinRider
                {
                    Id = -c.Id,
                    Name = $"{c.GivenName} {c.Surname}",
                    RoundRobinClub = "WVCC",
                    IsFemale = c.IsFemale
                });
        }

        private static Dictionary<string, RoundRobinRider> BuildWvccSecondClaimLookup(
            IEnumerable<RoundRobinRider> rrRiders)
        {
            // Second‑claim WVCC riders may appear in the RR CSV.
            // When they do, their CSV row contains a decorated name
            // including their first‑claim club. We index them by base
            // name ("GivenName Surname") so we can match them to Competitor objects.
            return rrRiders
                .Where(rr => string.Equals(rr.RoundRobinClub, "WVCC", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(
                    rr => GetBaseName(rr.Name),
                    rr => rr,
                    StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsWvccRide(Ride ride) =>
            string.Equals(ride.RoundRobinClub, "WVCC", StringComparison.OrdinalIgnoreCase);

        private static RoundRobinRider ResolveWvccRoundRobinRider(
            Ride ride,
            Dictionary<int, RoundRobinRider> synthetic,
            Dictionary<string, RoundRobinRider> wvccSecondClaim)
        {
            var baseName = $"{ride.Competitor.GivenName} {ride.Competitor.Surname}";

            // If this WVCC rider appears in the RR CSV (only true for second‑claim members),
            // use the CSV RoundRobinRider so their decorated name appears in results.
            if (wvccSecondClaim.TryGetValue(baseName, out var csvRider))
            {
                // Ensure gender is correct even if CSV loader missed it
                csvRider.IsFemale = ride.Competitor.IsFemale;
                return csvRider;
            }

            // Otherwise fall back to the synthetic WVCC rider.
            return synthetic[-ride.Competitor.Id];
        }

        private static string GetBaseName(string name)
        {
            // e.g. "Leah Cuthbertson (G Fox)" -> "Leah Cuthbertson"
            //      "Leah Cuthbertson (G Fox) (WVCC)" -> "Leah Cuthbertson"
            var idx = name.IndexOf(" (", StringComparison.Ordinal);
            return idx >= 0 ? name.Substring(0, idx) : name;
        }

        public static void AttachSyntheticGuestRoundRobinRiders(
            IEnumerable<Ride> rides,
            int competitionYear)
        {
            if (competitionYear != 2025)
                return; // Only applies to the 2025 open competition

            // Synthetic Guest IDs start at -10000 and count down
            int nextSyntheticId = -10000;

            // Cache synthetic riders by name to avoid duplicates
            var syntheticByName = new Dictionary<string, RoundRobinRider>(
                StringComparer.OrdinalIgnoreCase);

            foreach (var ride in rides)
            {
                // Only apply to Guest RR rides
                if (!string.Equals(ride.RoundRobinClub, "Guest", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (string.IsNullOrWhiteSpace(ride.Name))
                {
                    throw new InvalidOperationException(
                        $"Guest RR ride {ride.Id} has no Name.");
                }

                var key = ride.Name.Trim();

                // Reuse synthetic rider if already created
                if (!syntheticByName.TryGetValue(key, out var rr))
                {
                    var isFemale = key.Contains("(W)", StringComparison.OrdinalIgnoreCase);

                    rr = new RoundRobinRider
                    {
                        Id = nextSyntheticId--,
                        Name = key,
                        RoundRobinClub = "Guest",
                        IsFemale = isFemale
                    };

                    syntheticByName[key] = rr;
                }

                ride.RoundRobinRider = rr;
            }
        }
    }
}
