using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    internal class LeagueCompetitionResultsSet : CompetitionResultsSet
    {
        public League League { get; }
        public string? Sponsor { get; }

        private LeagueCompetitionResultsSet(
            League league,
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<CompetitorResult> scoredRides,
            string? sponsor)
            : base(calendar, scoredRides)
        {
            League = league;
            Sponsor = sponsor;
        }

        public override string DisplayName =>
            $"{(string.IsNullOrWhiteSpace(Sponsor) ? "WVCC" : Sponsor)} League - {League.GetDisplayName()}";
        public override string FileName => $"{Year}-league-{League.ToCsvValue()}";
        public override string SubFolderName => "competitions";
        public override string LinkText => League.GetDisplayName();

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
            IEnumerable<CalendarEvent> championshipCalendar,
            ICompetitionRules rules
            )
        {
            if (HasMissingCompetitors(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with Competitors.", nameof(allRides));

            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            if (HasNonChampionshipEvents(championshipCalendar))
                throw new ArgumentException($"{nameof(championshipCalendar)} must contain only championship events.", nameof(championshipCalendar));

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
                    championshipCalendar, 
                    r => r.LeaguePoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new LeagueCompetitionResultsSet(league, championshipCalendar, results, rules.LeagueSponsor);
        }
    }
}
