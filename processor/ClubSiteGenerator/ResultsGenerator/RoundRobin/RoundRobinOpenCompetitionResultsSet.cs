using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinOpenCompetitionResultsSet
        : RoundRobinCompetitionResultsSet<RoundRobinRiderResult>
    {
        private RoundRobinOpenCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinRiderResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series";
        public override string FileName => $"{Year}-rr-open";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Open";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Open;

        public override string EligibilityStatement =>
            "All members of the participating clubs are eligible for this competition.";

        public override string ScoringStatement
        {
            get
            {
                if (CompetitionRules is null)
                    throw new InvalidOperationException("CompetitionRules must be set before accessing ScoringStatement.");

                return $"Your competition score is the total of the points from your {CompetitionRules.RoundRobin.Count} highest scoring events.";
            }
        }

        public override string AdditionalComments =>
            "Riders should confirm which Round Robin club they are representing at sign‑on.";

        public static RoundRobinOpenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            var results = RoundRobinResultsCalculator.BuildOpenResults(
                allRides, rrCalendar, rules);

            return new RoundRobinOpenCompetitionResultsSet(rrCalendar, results)
            {
                CompetitionRules = rules,
                CssFile = "rr.css"
            };
        }
    }
}