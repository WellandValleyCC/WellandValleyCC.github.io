using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinOpenCompetitionResultsSet : RoundRobinCompetitionResultsSet
    {
        private RoundRobinOpenCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series";
        public override string FileName => $"{Year}-rr-open";
        public override string SubFolderName => "competitions";
        public override string LinkText => "RR Open";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Open;

        public override string EligibilityStatement =>
            "All members of participating clubs are eligible for this competition.";

        public static RoundRobinOpenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            if (HasNonRoundRobinEvents(rrCalendar))
                throw new ArgumentException($"{nameof(rrCalendar)} must contain only Round Robin events.", nameof(rrCalendar));

            // Only rides that belong to a Round Robin club participate
            var rrRides = allRides
                .Where(r => r.RoundRobinClub != null)
                .ToList();

            // Validate hydration rules
            foreach (var ride in rrRides)
            {
                // WVCC riders must have Competitor hydrated
                if (ride.RoundRobinClub == "WVCC")
                {
                    if (ride.Competitor == null)
                        throw new ArgumentException(
                            $"Ride {ride.Id} belongs to WVCC but Competitor is not hydrated.",
                            nameof(allRides));
                }
                else
                {
                    // Non-WVCC riders must have RoundRobinRider hydrated
                    if (ride.RoundRobinRider == null)
                        throw new ArgumentException(
                            $"Ride {ride.Id} belongs to {ride.RoundRobinClub} but RoundRobinRider is not hydrated.",
                            nameof(allRides));
                }
            }

            // Filter to valid rides
            var validRides = rrRides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Group by individual rider identity
            var groups = validRides
                .GroupBy(r =>
                    r.Competitor != null
                        ? $"C:{r.Competitor.Id}"          // WVCC competitor
                        : $"R:{r.RoundRobinRider!.Id}")   // Non-WVCC RR rider
                .ToList();

            // Build results using RoundRobinPoints
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(),
                    rrCalendar,
                    r => r.RoundRobinPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new RoundRobinOpenCompetitionResultsSet(rrCalendar, results);
        }
    }
}
