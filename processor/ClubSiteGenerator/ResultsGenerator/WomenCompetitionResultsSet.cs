using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class WomenCompetitionResultsSet : CompetitionResultsSet
    {
        private WomenCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship - Women";
        public override string FileName => $"{Year}-women";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Women";
        public override CompetitionType CompetitionType => CompetitionType.Women;

        public override string EligibilityStatement => "All first claim female members of the club are eligible for this championship.";

        public static WomenCompetitionResultsSet CreateFrom(
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
            var fullCompetitionRule = (IMixedCompetitionRule)rulesProvider.GetRule(2025, CompetitionRuleScope.Full);

            // filter women rides
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsFemale == true &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = championshipRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    calendar, 
                    r => r.WomenPoints,
                    tenMileRule,
                    fullCompetitionRule))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new WomenCompetitionResultsSet(calendar, results);
        }
    }
}

