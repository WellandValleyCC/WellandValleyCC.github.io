using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinWomenCompetitionResultsSet
        : RoundRobinCompetitionResultsSet<RoundRobinRiderResult>
    {
        private RoundRobinWomenCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinRiderResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series – Women";
        public override string FileName => $"{Year}-rr-women";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Women";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Women;

        public override string EligibilityStatement =>
            "All female members of the participating clubs are eligible for this competition.";

        public override string ScoringStatement
        {
            get
            {
                var rules = CompetitionRules
                    ?? throw new InvalidOperationException("CompetitionRules must be set before accessing ScoringStatement.");

                return $"Your competition score is the total of the points from your {rules.RoundRobin.Count} highest scoring events.";
            }
        }

        public override string AdditionalComments =>
            "Riders should confirm which Round Robin club they are representing at sign‑on.";

        public static RoundRobinWomenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            var results = RoundRobinResultsCalculator.BuildWomenResults(
                allRides, rrCalendar, rules);

            return new RoundRobinWomenCompetitionResultsSet(rrCalendar, results)
            {
                CompetitionRules = rules,
                CssFile = "rr.css"
            };
        }
    }
}