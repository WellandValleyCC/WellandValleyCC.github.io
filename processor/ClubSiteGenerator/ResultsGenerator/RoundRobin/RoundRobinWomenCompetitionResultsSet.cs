using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinWomenCompetitionResultsSet : RoundRobinCompetitionResultsSet
    {
        private RoundRobinWomenCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series – Women";
        public override string FileName => $"{Year}-rr-women";
        public override string SubFolderName => "competitions";
        public override string LinkText => "RR Women";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Women;

        public override string EligibilityStatement =>
            "All female members of participating clubs are eligible for this competition.";

        public static RoundRobinWomenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            if (HasNonRoundRobinEvents(rrCalendar))
                throw new ArgumentException($"{nameof(rrCalendar)} must contain only Round Robin events.", nameof(rrCalendar));

            //
            // Filter rides:
            // 1. Must belong to a Round Robin club
            // 2. Must belong to an event in the RR calendar
            // 3. Must be female
            //

            // ------------------------------------------------------------
            // Step 1: Filter by RR club + RR event only
            // ------------------------------------------------------------
            var rrEventNumbers = rrCalendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            var rrRides = allRides
                .Where(r =>
                    r.RoundRobinClub != null &&
                    rrEventNumbers.Contains(r.EventNumber))
                .ToList();

            // ------------------------------------------------------------
            // Step 2: Hydration validation
            // ------------------------------------------------------------
            foreach (var ride in rrRides)
            {
                if (ride.RoundRobinClub == "WVCC")
                {
                    if (ride.Competitor == null)
                        throw new ArgumentException(
                            $"Ride {ride.Id} belongs to WVCC but Competitor is not hydrated.",
                            nameof(allRides));
                }
                else
                {
                    if (ride.RoundRobinRider == null)
                        throw new ArgumentException(
                            $"Ride {ride.Id} belongs to {ride.RoundRobinClub} but RoundRobinRider is not hydrated.",
                            nameof(allRides));
                }
            }

            // ------------------------------------------------------------
            // Step 3: Now filter to women only
            // ------------------------------------------------------------
            rrRides = rrRides
                .Where(r =>
                    (r.Competitor != null && r.Competitor.IsFemale) ||
                    (r.RoundRobinRider != null && r.RoundRobinRider.IsFemale))
                .ToList();

            //
            // Only valid rides contribute to scoring
            //
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            //
            // Group by identity (WVCC competitor OR RR rider)
            //
            var groups = validRides
                .GroupBy(r =>
                    r.Competitor != null
                        ? $"C:{r.Competitor.Id}"
                        : $"R:{r.RoundRobinRider!.Id}")
                .ToList();

            //
            // Build individual results using RoundRobinWomenPoints
            //
            var results = groups
                .Select(group => RoundRobinResultsCalculator.BuildIndividualResult(
                    group.ToList(),
                    rrCalendar,
                    r => r.RoundRobinWomenPoints,
                    rules))
                .ToList();

            //
            // Sort using RR-specific ordering
            //
            results = RoundRobinResultsCalculator.SortResults(results).ToList();

            return new RoundRobinWomenCompetitionResultsSet(rrCalendar, results);
        }
    }
}