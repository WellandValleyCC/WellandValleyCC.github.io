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

        public override string EligibilityStatement => string.Empty;

        public override string ScoringStatement
        {
            get
            {
                var rules = CompetitionRules
                    ?? throw new InvalidOperationException("CompetitionRules must be set before accessing ScoringStatement.");

                var openCount = rules.RoundRobin.Team.OpenCount;
                var womenCount = rules.RoundRobin.Team.WomenCount;

                string RiderPhrase(int n) =>
                    n == 1 ? "top rider's" : $"top {n} riders'";

                return
                    $"Each club's score at an event is the sum of their {RiderPhrase(openCount)} points " +
                    $"in the open competition plus the {RiderPhrase(womenCount)} points in the women's competition.";
            }
        }

        public override string AdditionalComments => string.Empty;

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