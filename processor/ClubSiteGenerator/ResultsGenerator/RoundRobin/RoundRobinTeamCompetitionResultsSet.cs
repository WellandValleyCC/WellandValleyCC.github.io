using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinTeamCompetitionResultsSet : RoundRobinCompetitionResultsSet
    {
        private RoundRobinTeamCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series – Team";
        public override string FileName => $"{Year}-rr-team";
        public override string SubFolderName => "competitions";
        public override string LinkText => "RR Team";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Team;

        public override string EligibilityStatement =>
            "All participating clubs are eligible for the team competition.";

        public static RoundRobinTeamCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            if (HasNonRoundRobinEvents(rrCalendar))
                throw new ArgumentException($"{nameof(rrCalendar)} must contain only Round Robin events.", nameof(rrCalendar));

            //
            // Step 1: Filter by RR club + RR event only
            //
            var rrEventNumbers = rrCalendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            var rrRides = allRides
                .Where(r =>
                    r.RoundRobinClub != null &&
                    rrEventNumbers.Contains(r.EventNumber))
                .ToList();

            //
            // Step 2: Hydration validation
            //
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

            //
            // Step 3: Only valid rides contribute to scoring
            //
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            //
            // Step 4: Group by club name (team identity)
            //
            var groups = validRides
                .GroupBy(r => r.RoundRobinClub!)
                .ToList();

            //
            // Step 5: Build team results
            //
            var results = groups
                .Select(group => RoundRobinResultsCalculator.BuildTeamResult(
                    group.ToList(),
                    rrCalendar,
                    rules))
                .ToList();

            //
            // Step 6: Sort using RR-specific ordering
            //
            results = RoundRobinResultsCalculator.SortResults(results).ToList();

            return new RoundRobinTeamCompetitionResultsSet(rrCalendar, results);
        }
    }
}