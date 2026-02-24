using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinClubCompetitionResultsSet
        : RoundRobinCompetitionResultsSet<RoundRobinTeamResult>
    {
        private RoundRobinClubCompetitionResultsSet(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinTeamResult> results)
            : base(calendar, results)
        {
        }

        public override string DisplayName => "Round Robin TT Series – Club";
        public override string FileName => $"{Year}-rr-club";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Club";
        public override RoundRobinCompetitionType CompetitionType => RoundRobinCompetitionType.Club;

        public override string EligibilityStatement => string.Empty;

        public override string ScoringStatement
        {
            get
            {
                var rules = CompetitionRules
                    ?? throw new InvalidOperationException("CompetitionRules must be set before accessing ScoringStatement.");

                var openCount = rules.RoundRobin.Club.OpenCount;
                var womenCount = rules.RoundRobin.Club.WomenCount;

                string RiderPhrase(int n) =>
                    n == 1 ? "top rider's" : $"top {n} riders'";

                return
                    $"Each club's score at an event is the sum of their {RiderPhrase(openCount)} points " +
                    $"in the open competition plus the {RiderPhrase(womenCount)} points in the women's competition.";
            }
        }

        public override string AdditionalComments => string.Empty;

        public static RoundRobinClubCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> rrCalendar,
            ICompetitionRules rules)
        {
            var results = RoundRobinResultsCalculator.BuildTeamResults(
                allRides, rrCalendar, rules);

            return new RoundRobinClubCompetitionResultsSet(rrCalendar, results)
            {
                CompetitionRules = rules,
                CssFile = "rr.css"
            };
        }
    }
}