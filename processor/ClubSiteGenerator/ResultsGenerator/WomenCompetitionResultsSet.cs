using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
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
        public override string LinkText => "Women";
        public override CompetitionType CompetitionType => CompetitionType.Women;

        public override string EligibilityStatement => "All first claim female members of the club are eligible for this competition.";

        public static WomenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> championshipCalendar,
            ICompetitionRules rules)
        {
            if (HasMissingCompetitors(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with Competitors.", nameof(allRides));

            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            if (HasNonChampionshipEvents(championshipCalendar))
                throw new ArgumentException($"{nameof(championshipCalendar)} must contain only championship events.", nameof(championshipCalendar));

            // filter women rides
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor is { } c &&
                    c.IsEligible() &&
                    r.Competitor.IsFemale == true &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = championshipRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => WvccCompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    championshipCalendar, 
                    r => r.WomenPoints,
                    rules))
                .ToList();

            results = WvccCompetitionResultsCalculator.SortResults(results).ToList();

            return new WomenCompetitionResultsSet(championshipCalendar, results);
        }
    }
}

