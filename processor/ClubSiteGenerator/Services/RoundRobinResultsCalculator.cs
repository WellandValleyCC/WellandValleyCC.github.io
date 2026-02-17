using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.Services
{
    public static class RoundRobinResultsCalculator
    {
        // ------------------------------------------------------------
        // Identity helper (WVCC Competitor OR RoundRobinRider)
        // ------------------------------------------------------------
        private static string GetRiderIdentity(Ride r) =>
            r.Competitor != null
                ? $"C:{r.Competitor.Id}"
                : $"R:{r.RoundRobinRider!.Id}";

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

            var firstId = GetRiderIdentity(rides[0]);

            if (rides.Any(r => GetRiderIdentity(r) != firstId))
                throw new ArgumentException(
                    "All rides must belong to the same competitor or Round Robin rider.",
                    nameof(rides));
        }

        // ------------------------------------------------------------
        // Build INDIVIDUAL result (Open, Women)
        // ------------------------------------------------------------
        public static CompetitorResult BuildIndividualResult(
            IReadOnlyList<Ride> rrRides,
            IEnumerable<CalendarEvent> rrCalendar,
            Func<Ride, double?> pointsSelector,
            ICompetitionRules rules)
        {
            ValidateIdentity(rrRides);
            ValidateCalendar(rrCalendar);

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

            // Determine identity for display
            var competitor = rrRides.Last().Competitor;
            var rrRider = rrRides.Last().RoundRobinRider;

            var result = new CompetitorResult
            {
                Rides = rrRides,
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,

                AllEvents = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = scoringRides
                },

                TenMileCompetition = new CompetitionScore(),

                FullCompetition = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = scoringRides
                }
            };

            // Only set Competitor if present
            if (competitor != null)
                result.Competitor = competitor;

            // Only set RR rider if present
            if (rrRider != null)
                result.RoundRobinRider = rrRider;

            return result;
        }

        // ------------------------------------------------------------
        // Build TEAM result (best 4 + best 1)
        // ------------------------------------------------------------
        public static CompetitorResult BuildTeamResult(
            IReadOnlyList<Ride> rrRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            ValidateCalendar(rrCalendar);

            int openCount = rules.RoundRobin.Team.OpenCount;   // e.g. 4
            int womenCount = rules.RoundRobin.Team.WomenCount; // e.g. 1

            // Group all rides for this club by RR event number
            var eventGroups = rrRides
                .GroupBy(r => r.CalendarEvent!.RoundRobinEventNumber)
                .ToList();

            var eventPoints = new Dictionary<int, double?>();
            var eventStatuses = new Dictionary<int, RideStatus>();

            foreach (var ev in eventGroups)
            {
                int eventNumber = ev.Key; // RR event number
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

            // Total = sum of all event scores (no best-N across events)
            double? totalPoints =
                eventPoints.Values.Any(v => v.HasValue)
                    ? eventPoints.Values.Where(v => v.HasValue).Sum(v => v!.Value)
                    : (double?)null;

            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            var clubName = rrRides.First().RoundRobinClub!;

            return new CompetitorResult
            {
                RoundRobinClubName = clubName,

                Rides = rrRides,
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,

                AllEvents = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = validRides
                },

                TenMileCompetition = new CompetitionScore(),

                FullCompetition = new CompetitionScore
                {
                    Points = totalPoints,
                    Rides = validRides
                }
            };
        }

        // ------------------------------------------------------------
        // Sorting (RR-specific)
        // ------------------------------------------------------------
        public static IList<CompetitorResult> SortResults(IList<CompetitorResult> results)
        {
            return results
                .OrderByDescending(r => r.FullCompetition.Points.HasValue)
                .ThenByDescending(r => r.FullCompetition.Points)
                .ThenBy(r => GetSortName(r))
                .ToList();
        }

        private static string GetSortName(CompetitorResult r)
        {
            // WVCC competitor
            if (r.Competitor != null)
            {
                var (surname, given) = NameParts.Split($"{r.Competitor.GivenName} {r.Competitor.Surname}");
                return $"{surname} {given}";
            }

            // Non-WVCC individual rider
            if (r.RoundRobinRider != null)
            {
                var (surname, given) = NameParts.Split(r.RoundRobinRider.Name);
                return $"{surname} {given}";
            }

            // Team competition
            if (r.RoundRobinClubName != null)
                return r.RoundRobinClubName;

            return "ZZZ"; // fallback
        }
    }
}