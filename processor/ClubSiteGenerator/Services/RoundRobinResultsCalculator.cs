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
                    continue;
                }

                var bestOpen = valid
                    .Select(r => r.RoundRobinPoints)
                    .Where(p => p.HasValue)
                    .Select(p => p!.Value)
                    .OrderByDescending(p => p)
                    .Take(openCount)
                    .ToList();

                var bestWomen = valid
                    .Select(r => r.RoundRobinWomenPoints)
                    .Where(p => p.HasValue)
                    .Select(p => p!.Value)
                    .OrderByDescending(p => p)
                    .Take(womenCount)
                    .ToList();

                double eventScore = bestOpen.Sum() + bestWomen.Sum();

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
                }
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
    }
}
