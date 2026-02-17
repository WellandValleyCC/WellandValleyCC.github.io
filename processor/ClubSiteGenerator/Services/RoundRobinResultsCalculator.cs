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
            var eventPoints = rrRides.ToDictionary(
                r => r.EventNumber,
                r => r.Status == RideStatus.Valid ? pointsSelector(r) : null);

            var eventStatuses = rrRides.ToDictionary(
                r => r.EventNumber,
                r => r.Status);

            // Determine identity for display
            var competitor = rrRides.Last().Competitor;
            var rrRider = rrRides.Last().RoundRobinRider;

            return new CompetitorResult
            {
                Competitor = competitor,   // null for non-WVCC
                RoundRobinRider = rrRider,

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

            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Team scoring rules from config
            int openCount = rules.RoundRobin.Team.OpenCount;
            int womenCount = rules.RoundRobin.Team.WomenCount;

            // Best N Open points
            var bestOpen = validRides
                .Select(r => r.RoundRobinPoints)
                .Where(p => p.HasValue)
                .Select(p => p!.Value)
                .OrderByDescending(p => p)
                .Take(openCount)
                .ToList();

            // Best M Women points
            var bestWomen = validRides
                .Select(r => r.RoundRobinWomenPoints)
                .Where(p => p.HasValue)
                .Select(p => p!.Value)
                .OrderByDescending(p => p)
                .Take(womenCount)
                .ToList();

            double? totalPoints =
                (bestOpen.Any() || bestWomen.Any())
                    ? bestOpen.Sum() + bestWomen.Sum()
                    : (double?)null;

            // Event-level dictionaries
            var eventPoints = rrRides.ToDictionary(
                r => r.EventNumber,
                r => r.Status == RideStatus.Valid ? r.RoundRobinPoints : null);

            var eventStatuses = rrRides.ToDictionary(
                r => r.EventNumber,
                r => r.Status);

            // Team identity: use the club name from the first ride
            var roundRobinClubName = rrRides.First().RoundRobinClub!;

            return new CompetitorResult
            {
                RoundRobinClubName = roundRobinClubName,
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