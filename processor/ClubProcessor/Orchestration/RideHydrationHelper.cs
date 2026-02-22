using ClubCore.Models;

namespace ClubProcessor.Orchestration
{
    public static class RideHydrationHelper
    {
        public static void HydrateCompetitors(
            IEnumerable<Ride> rides, 
            IEnumerable<Competitor> competitors)
        {
            if (rides == null) throw new ArgumentNullException(nameof(rides));
            if (competitors == null) throw new ArgumentNullException(nameof(competitors));

            // Group competitors by ClubNumber and pre-sort each group's snapshots by CreatedUtc ascending
            var competitorsByClubNumber = competitors
                .GroupBy(c => c.ClubNumber)
                .ToDictionary(g => g.Key, g => g.OrderBy(c => DateTime.SpecifyKind(c.CreatedUtc, DateTimeKind.Utc)).ToList());

            var missing = new List<int>();

            foreach (var ride in rides)
            {
                if (!ride.ClubNumber.HasValue) continue;

                var clubNumber = ride.ClubNumber.Value;

                if (!competitorsByClubNumber.TryGetValue(clubNumber, out var snapshots) || snapshots.Count == 0)
                {
                    missing.Add(clubNumber);
                    continue;
                }

                if (ride.CalendarEvent == null)
                    throw new InvalidOperationException($"Ride '{ride.Name}' in Event {ride.EventNumber} is missing its CalendarEvent");

                var eventDateUtc = DateTime.SpecifyKind(ride.CalendarEvent.EventDate, DateTimeKind.Utc);

                // Find the latest snapshot whose CreatedUtc <= eventDateUtc
                // snapshots are sorted ascending, so scan from the end
                Competitor? chosen = null;
                for (int i = snapshots.Count - 1; i >= 0; i--)
                {
                    var candidate = snapshots[i];
                    var createdUtc = DateTime.SpecifyKind(candidate.CreatedUtc, DateTimeKind.Utc);
                    if (createdUtc <= eventDateUtc)
                    {
                        chosen = candidate;
                        break;
                    }
                }

                // If none found that are <= event date, treat as missing (the Competitor was not a member of the club
                // at the time of the event)
                if (chosen == null)
                {
                    // Option A: treat as missing
                    missing.Add(clubNumber);
                    continue;
                }

                ride.Competitor = chosen;
            }

            if (missing.Any())
            {
                Console.WriteLine("[ERROR] The following ClubNumbers were found in rides but missing from the competitor list:");
                foreach (var clubNumber in missing.Distinct())
                {
                    Console.WriteLine($"  - ClubNumber {clubNumber}");
                }

                throw new InvalidOperationException("Scoring aborted: missing competitors detected. Please check the membership list.");
            }
        }

        public static void HydrateRoundRobinRiders(
            IEnumerable<Ride> rides,
            IEnumerable<RoundRobinRider> rrRiders,
            int competitionYear)
        {
            if (rides == null) throw new ArgumentNullException(nameof(rides));
            if (rrRiders == null) throw new ArgumentNullException(nameof(rrRiders));

            var rrByName = rrRiders
                .GroupBy(r => r.DecoratedName.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            var missing = new List<(string Name, string Club)>();

            // Synthetic ID generator for Guest riders
            int nextSyntheticId = -10000;

            foreach (var ride in rides)
            {
                if (ride.ClubNumber != null) continue;
                if (string.IsNullOrWhiteSpace(ride.RoundRobinClub)) continue;
                if (string.Equals(ride.RoundRobinClub, "WVCC", StringComparison.OrdinalIgnoreCase)) continue;

                if (string.IsNullOrWhiteSpace(ride.Name))
                    throw new InvalidOperationException(
                        $"Ride in Event {ride.EventNumber} has no rider Name but is marked as RoundRobin.");

                var key = ride.Name.Trim();

                if (!rrByName.TryGetValue(key, out var rr))
                {
                    // 2025-only synthetic Guest rider
                    if (competitionYear == 2025 &&
                        string.Equals(ride.RoundRobinClub, "Guest", StringComparison.OrdinalIgnoreCase))
                    {
                        var isFemale = key.Contains("(W)", StringComparison.OrdinalIgnoreCase);

                        rr = new RoundRobinRider
                        {
                            Id = nextSyntheticId--,   // unique negative ID
                            Name = key,
                            RoundRobinClub = "Guest",
                            IsFemale = isFemale
                        };

                        rrByName[key] = rr;
                        ride.RoundRobinRider = rr;
                        continue;
                    }

                    // Otherwise: real missing rider
                    missing.Add((ride.Name, ride.RoundRobinClub));
                    continue;
                }

                ride.RoundRobinRider = rr;
            }

            if (missing.Any())
            {
                Console.WriteLine("[ERROR] Some Round Robin riders could not be matched:");
                foreach (var m in missing.Distinct())
                    Console.WriteLine($"  - Name '{m.Name}' (Club '{m.Club}') not found in RoundRobinRiders list");

                throw new InvalidOperationException(
                    "Scoring aborted: missing RoundRobin riders detected. Please check the RoundRobinRiders sheet.");
            }
        }

        public static void HydrateCalendarEvents(
            IEnumerable<Ride> rides, 
            IEnumerable<CalendarEvent> calendarEvents)
        {
            if (rides == null) throw new ArgumentNullException(nameof(rides));
            if (calendarEvents == null) throw new ArgumentNullException(nameof(calendarEvents));

            // Ensure there are no duplicate EventNumber entries; if there are, fail with a clear message
            var grouped = calendarEvents.GroupBy(e => e.EventNumber).ToList();
            var duplicates = grouped.Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                var dupInfo = string.Join(", ", duplicates.Select(g => $"{g.Key} (count={g.Count()})"));
                Console.WriteLine("[ERROR] Duplicate CalendarEvent EventNumbers detected: " + dupInfo);
                throw new InvalidOperationException($"Duplicate CalendarEvent.EventNumber values found: {dupInfo}");
            }

            // Safe to build a dictionary now (each key is unique)
            var eventsByNumber = grouped.ToDictionary(g => g.Key, g => g.Single());

            var missing = new List<int>();

            foreach (var ride in rides)
            {
                var eventNumber = ride.EventNumber;
                if (eventsByNumber.TryGetValue(eventNumber, out var ev))
                {
                    ev.EventDate = DateTime.SpecifyKind(ev.EventDate, DateTimeKind.Utc);
                    ride.CalendarEvent = ev;
                }
                else
                {
                    missing.Add(eventNumber);
                }
            }

            if (missing.Any())
            {
                Console.WriteLine("[ERROR] The following EventNumbers were found in rides but missing from the calendar event list:");
                foreach (var eventNumber in missing.Distinct().OrderBy(n => n))
                {
                    Console.WriteLine($"  - EventNumber {eventNumber}");
                }

                throw new InvalidOperationException("Scoring aborted: missing calendar events detected. Please check the calendar event list.");
            }
        }
    }
}
