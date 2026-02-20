using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.Services
{
    public static class RoundRobinResultsCalculator
    {
        // ------------------------------------------------------------
        // Identity helper (unified RoundRobinRider domain)
        // ------------------------------------------------------------
        private static int GetRiderId(Ride r) =>
            r.RoundRobinRider!.Id; // WVCC riders have synthetic negative IDs

        // ------------------------------------------------------------
        // Calendar validation
        // ------------------------------------------------------------
        private static void ValidateCalendar(IEnumerable<CalendarEvent> calendar)
        {
            if (calendar == null || calendar.Any(ev => !ev.IsRoundRobinEvent))
                throw new ArgumentException(
                    "Round Robin calendar must not be null and must contain only Round Robin events.",
                    nameof(calendar));
        }

        // ------------------------------------------------------------
        // Identity validation
        // ------------------------------------------------------------
        private static void ValidateIdentity(IReadOnlyList<Ride> rides)
        {
            if (rides == null || rides.Count == 0)
                throw new ArgumentException("Rides collection must not be null or empty.", nameof(rides));

            var firstId = GetRiderId(rides[0]);

            if (rides.Any(r => GetRiderId(r) != firstId))
                throw new ArgumentException(
                    "All rides must belong to the same Round Robin rider.",
                    nameof(rides));
        }

        public static IList<RoundRobinRiderResult> BuildOpenResults(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            ValidateCalendar(rrCalendar);

            var rrEventNumbers = rrCalendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            // Filter to RR rides only
            var rrRides = allRides
                .Where(r => r.RoundRobinClub != null &&
                            rrEventNumbers.Contains(r.EventNumber))
                .ToList();

            // Hydration validation
            foreach (var ride in rrRides)
            {
                if (ride.RoundRobinRider == null)
                    throw new ArgumentException(
                        $"Ride {ride.Id} is missing RoundRobinRider.",
                        nameof(allRides));
            }

            // Only valid rides contribute to scoring
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Group by rider identity
            var groups = validRides
                .GroupBy(r => r.RoundRobinRider!.Id)
                .ToList();

            // Build per‑rider results
            var results = groups
                .Select(g => BuildIndividualResult(
                    g.ToList(),
                    rrCalendar,
                    r => r.RoundRobinPoints,
                    rules))
                .ToList();

            // ------------------------------------------------------------
            // SORT by Best‑N total, then Surname, then Given name
            // ------------------------------------------------------------
            results = results
                .OrderByDescending(r => r.Total.Points ?? 0)
                .ThenBy(r => NameParts.Split(r.Rider.Name).Surname)
                .ThenBy(r => NameParts.Split(r.Rider.Name).GivenNames)
                .ToList();

            // ------------------------------------------------------------
            // ASSIGN RANKS (tie‑aware)
            // ------------------------------------------------------------
            AssignRanks(results);

            return results;
        }

        public static IList<RoundRobinRiderResult> BuildWomenResults(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            ValidateCalendar(rrCalendar);

            var rrEventNumbers = rrCalendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            // Filter to RR rides only
            var rrRides = allRides
                .Where(r => r.RoundRobinClub != null &&
                            rrEventNumbers.Contains(r.EventNumber))
                .ToList();

            // Hydration validation
            foreach (var ride in rrRides)
            {
                if (ride.RoundRobinRider == null)
                    throw new ArgumentException(
                        $"Ride {ride.Id} is missing RoundRobinRider.",
                        nameof(allRides));
            }

            // Filter to women only
            rrRides = rrRides
                .Where(r => r.RoundRobinRider!.IsFemale)
                .ToList();

            // Only valid rides contribute to scoring
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Group by rider identity
            var groups = validRides
                .GroupBy(r => r.RoundRobinRider!.Id)
                .ToList();

            // Build per‑rider results
            var results = groups
                .Select(g => BuildIndividualResult(
                    g.ToList(),
                    rrCalendar,
                    r => r.RoundRobinWomenPoints,
                    rules))
                .ToList();

            // ------------------------------------------------------------
            // SORT by Best‑N total, then Surname, then Given name
            // ------------------------------------------------------------
            results = results
                .OrderByDescending(r => r.Total.Points ?? 0)
                .ThenBy(r => NameParts.Split(r.Rider.Name).Surname)
                .ThenBy(r => NameParts.Split(r.Rider.Name).GivenNames)
                .ToList();

            // ------------------------------------------------------------
            // ASSIGN RANKS (tie‑aware)
            // ------------------------------------------------------------
            AssignRanks(results);

            return results;
        }
        public static IList<RoundRobinTeamResult> BuildTeamResults(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            ValidateCalendar(rrCalendar);

            var rrEventNumbers = rrCalendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            // Filter to RR rides only
            var rrRides = allRides
                .Where(r => r.RoundRobinClub != null &&
                            rrEventNumbers.Contains(r.EventNumber))
                .ToList();

            // Hydration validation
            foreach (var ride in rrRides)
            {
                if (ride.RoundRobinRider == null)
                    throw new ArgumentException(
                        $"Ride {ride.Id} is missing RoundRobinRider.",
                        nameof(allRides));
            }

            // Only valid rides contribute to scoring
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Group by club
            var groups = validRides
                .GroupBy(r => r.RoundRobinClub!)
                .ToList();

            // Build team results
            var results = groups
                .Select(g => BuildTeamResult(
                    g.ToList(),
                    rrCalendar,
                    rules))
                .ToList();

            // ------------------------------------------------------------
            // SORT + ASSIGN RANKS (tie‑aware)
            // ------------------------------------------------------------
            results = results
                .OrderByDescending(r => r.Total.Points ?? 0)
                .ThenBy(r => r.ClubShortName)
                .ToList();

            AssignTeamRanks(results);

            return results;
        }



        // ------------------------------------------------------------
        // Build INDIVIDUAL result (Open, Women)
        // ------------------------------------------------------------
        public static RoundRobinRiderResult BuildIndividualResult(
            IReadOnlyList<Ride> rrRides,
            IEnumerable<CalendarEvent> rrCalendar,
            Func<Ride, double?> pointsSelector,
            ICompetitionRules rules)
        {
            ValidateIdentity(rrRides);
            ValidateCalendar(rrCalendar);

            var rider = rrRides[0].RoundRobinRider!;

            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Sort by points descending
            var sorted = validRides
                .OrderByDescending(r => pointsSelector(r) ?? 0)
                .ToList();

            // Take top N rides according to RR rules
            int scoringCount = rules.RoundRobin.Count;
            var scoringRides = sorted.Take(scoringCount).ToList();

            double? totalPoints = scoringRides.Any()
                ? scoringRides.Sum(r => pointsSelector(r) ?? 0)
                : (double?)null;

            // Event-level dictionaries
            var eventPoints = rrRides
                .GroupBy(r => r.CalendarEvent!.RoundRobinEventNumber)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var ride = g.First();
                        return ride.Status == RideStatus.Valid ? pointsSelector(ride) : null;
                    });

            var eventStatuses = rrRides
                .GroupBy(r => r.CalendarEvent!.RoundRobinEventNumber)
                .ToDictionary(
                    g => g.Key,
                    g => g.Any(r => r.Status == RideStatus.Valid)
                            ? RideStatus.Valid
                            : g.First().Status);

            return new RoundRobinRiderResult
            {
                Rider = rider,
                Rides = rrRides,
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,
                EventsCompleted = validRides.Count,
                Total = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = scoringRides
                }
            };
        }

        // ------------------------------------------------------------
        // Build TEAM result (best 4 open + best 1 women)
        // ------------------------------------------------------------
        public static RoundRobinTeamResult BuildTeamResult(
            IReadOnlyList<Ride> rrRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            ValidateCalendar(rrCalendar);

            int openCount = rules.RoundRobin.Team.OpenCount;   // e.g. 4
            int womenCount = rules.RoundRobin.Team.WomenCount; // e.g. 1

            var clubName = rrRides.First().RoundRobinClub!;

            // Group all rides for this club by RR event number
            var eventGroups = rrRides
                .GroupBy(r => r.CalendarEvent!.RoundRobinEventNumber)
                .ToList();

            var eventPoints = new Dictionary<int, double?>();
            var eventStatuses = new Dictionary<int, RideStatus>();

            // Per‑event contributing riders
            var contributingOpenByEvent = new Dictionary<int, IReadOnlyList<RoundRobinRiderScore>>();
            var contributingWomenByEvent = new Dictionary<int, IReadOnlyList<RoundRobinRiderScore>>();

            foreach (var ev in eventGroups)
            {
                int eventNumber = ev.Key;
                var ridesInEvent = ev.ToList();

                var valid = ridesInEvent
                    .Where(r => r.Status == RideStatus.Valid)
                    .ToList();

                if (!valid.Any())
                {
                    eventPoints[eventNumber] = null;
                    eventStatuses[eventNumber] = ridesInEvent.First().Status;

                    contributingOpenByEvent[eventNumber] = Array.Empty<RoundRobinRiderScore>();
                    contributingWomenByEvent[eventNumber] = Array.Empty<RoundRobinRiderScore>();
                    continue;
                }

                // ---- OPEN scoring ----
                var bestOpenRides = valid
                    .Where(r => r.RoundRobinPoints.HasValue)
                    .OrderByDescending(r => r.RoundRobinPoints!.Value)
                    .Take(openCount)
                    .ToList();

                var bestOpenPoints = bestOpenRides
                    .Select(r => r.RoundRobinPoints!.Value)
                    .ToList();

                contributingOpenByEvent[eventNumber] =
                    bestOpenRides
                        .Select(r => new RoundRobinRiderScore
                        {
                            Rider = r.RoundRobinRider!,
                            Points = r.RoundRobinPoints!.Value
                        })
                        .ToList();

                // ---- WOMEN scoring ----
                var bestWomenRides = valid
                    .Where(r => r.RoundRobinWomenPoints.HasValue)
                    .OrderByDescending(r => r.RoundRobinWomenPoints!.Value)
                    .Take(womenCount)
                    .ToList();

                var bestWomenPoints = bestWomenRides
                    .Select(r => r.RoundRobinWomenPoints!.Value)
                    .ToList();

                contributingWomenByEvent[eventNumber] =
                    bestWomenRides
                        .Select(r => new RoundRobinRiderScore
                        {
                            Rider = r.RoundRobinRider!,
                            Points = r.RoundRobinWomenPoints!.Value
                        })
                        .ToList();

                // ---- Event total ----
                double eventScore = bestOpenPoints.Sum() + bestWomenPoints.Sum();

                eventPoints[eventNumber] = eventScore;
                eventStatuses[eventNumber] = RideStatus.Valid;
            }

            double? totalPoints =
                eventPoints.Values.Any(v => v.HasValue)
                    ? eventPoints.Values.Where(v => v.HasValue).Sum(v => v!.Value)
                    : (double?)null;

            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            return new RoundRobinTeamResult
            {
                ClubShortName = clubName,
                Riders = rrRides.Select(r => r.RoundRobinRider!).Distinct().ToList(),
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,
                EventsCompleted = validRides.Count,
                Total = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = validRides
                },

                // Per‑event breakdowns
                ContributingOpenRidesByEvent = contributingOpenByEvent,
                ContributingWomenRidesByEvent = contributingWomenByEvent
            };
        }

        // ------------------------------------------------------------
        // Sorting (RR-specific)
        // ------------------------------------------------------------
        public static IList<T> SortResults<T>(IList<T> results)
        {
            return results
                .OrderByDescending(r => GetPoints(r))
                .ThenBy(r => GetSortName(r))
                .ToList();
        }

        private static double? GetPoints<T>(T r) =>
            r switch
            {
                RoundRobinRiderResult rr => rr.Total.Points,
                RoundRobinTeamResult tr => tr.Total.Points,
                _ => null
            };

        private static string GetSortName<T>(T r) =>
            r switch
            {
                RoundRobinRiderResult rr =>
                    NameParts.Split(rr.Rider.Name) is var (surname, given)
                        ? $"{surname} {given}"
                        : rr.Rider.Name,

                RoundRobinTeamResult tr => tr.ClubShortName,

                _ => "ZZZ"
            };

        private static void AssignRanks(List<RoundRobinRiderResult> results)
        {
            int currentRank = 1;
            double? lastScore = null;
            int? lastAssignedRank = null;

            for (int i = 0; i < results.Count; i++)
            {
                var score = results[i].Total.Points;

                if (score == null)
                {
                    results[i].Rank = null;
                    continue;
                }

                if (lastScore != null && score == lastScore)
                {
                    // tie
                    results[i].Rank = lastAssignedRank;
                }
                else
                {
                    results[i].Rank = currentRank;
                    lastAssignedRank = currentRank;
                }

                lastScore = score;
                currentRank++;
            }
        }

        private static void AssignTeamRanks(List<RoundRobinTeamResult> results)
        {
            int currentRank = 1;
            double? lastScore = null;
            int? lastAssignedRank = null;

            for (int i = 0; i < results.Count; i++)
            {
                var score = results[i].Total.Points;

                if (score == null)
                {
                    results[i].Rank = null;
                    continue;
                }

                if (lastScore != null && score == lastScore)
                {
                    // tie
                    results[i].Rank = lastAssignedRank;
                }
                else
                {
                    results[i].Rank = currentRank;
                    lastAssignedRank = currentRank;
                }

                lastScore = score;
                currentRank++;
            }
        }
    }
}
