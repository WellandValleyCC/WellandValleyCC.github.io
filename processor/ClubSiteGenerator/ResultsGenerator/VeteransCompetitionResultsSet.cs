using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class VeteransCompetitionResultsSet : CompetitionResultsSet
    {
        private VeteransCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship - Veterans";
        public override string FileName => $"{Year}-veterans";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Veterans";
        public override CompetitionType CompetitionType => CompetitionType.Veterans;

        public override string EligibilityStatement => "All first claim veteran members of the club are eligible for this competition.";

        public static VeteransCompetitionResultsSet CreateFrom(
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

            // filter veteran rides
            var veteranRides = allRides
                .Where(r =>
                    r.Competitor is { IsVeteran: true } c &&
                    c.IsEligible() &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = veteranRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    championshipCalendar, 
                    r => r.VeteransPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new VeteransCompetitionResultsSet(championshipCalendar, results);
        }
    }
}

