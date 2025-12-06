using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    internal class LeagueCompetitionResultsSet : CompetitionResultsSet
    {
        public League League { get; }

        private LeagueCompetitionResultsSet(
            League league,
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
            League = league;
        }

        private const string Sponsor = "George Halls Cycles";
        public override string DisplayName => $"{Sponsor} League - {League.GetDisplayName()}";
        public override string FileName => $"{Year}-league-{League.ToCsvValue()}";
        public override string SubFolderName => "competitions";
        public override string GenericName => League.GetDisplayName();

        public override CompetitionType CompetitionType => League switch
        {
            League.Premier => CompetitionType.PremierLeague,
            League.League1 => CompetitionType.League1,
            League.League2 => CompetitionType.League2,
            League.League3 => CompetitionType.League3,
            League.League4 => CompetitionType.League4,
            _ => CompetitionType.Undefined
        };

        public override string EligibilityStatement => $"This competition is for club members assigned to {League.GetDisplayName()}.";

        public static LeagueCompetitionResultsSet CreateFrom(
            League league,
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> calendar,
            ICompetitionRulesProvider rulesProvider)
        {
            if (allRides.Any(r => r.ClubNumber != null && r.Competitor is null))
            {
                throw new ArgumentException(
                    $"{nameof(allRides)} collection must be hydrated with Competitors.",
                    nameof(allRides));
            }

            if (allRides.Any(r => r.CalendarEvent is null))
            {
                throw new ArgumentException(
                    $"{nameof(allRides)} collection must be hydrated with CalendarEvents.",
                    nameof(allRides));
            }

            // resolve rules 
            var year = calendar.First().EventDate.Year;
            var tenMileRule = rulesProvider.GetRule(year, CompetitionRuleScope.TenMile);
            var fullCompetitionRule = rulesProvider.GetRule(year, CompetitionRuleScope.Full);

            // filter rides must be Valid, Competitor muist be in this league
            var Championship = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.League == league &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = Championship
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    calendar, 
                    r => r.LeaguePoints,
                    tenMileRule,
                    fullCompetitionRule))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new LeagueCompetitionResultsSet(league, calendar, results);
        }
    }
}
