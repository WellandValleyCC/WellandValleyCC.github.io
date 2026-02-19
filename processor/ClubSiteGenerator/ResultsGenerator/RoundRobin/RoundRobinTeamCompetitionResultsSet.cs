using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinTeamCompetitionResultsSet
        : RoundRobinCompetitionResultsSet<RoundRobinTeamResult>
    {
        private RoundRobinTeamCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinTeamResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Inter‑Club Round Robin TT Series – Team";
        public override string FileName => $"{Year}-rr-team";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Team";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Team;

        public override string EligibilityStatement =>
            "All participating clubs are eligible for the team competition.";

        public static RoundRobinTeamCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            var results = RoundRobinResultsCalculator.BuildTeamResults(
                allRides, rrCalendar, rules);

            return new RoundRobinTeamCompetitionResultsSet(rrCalendar, results)
            {
                CompetitionRules = rules,
                CssFile = "rr.css"
            };
        }
    }
}