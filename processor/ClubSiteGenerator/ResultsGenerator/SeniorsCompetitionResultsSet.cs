using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class SeniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private SeniorsCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship - Seniors";
        public override string FileName => $"{Year}-seniors";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Seniors";
        public override CompetitionType CompetitionType => CompetitionType.Seniors;

        public override string EligibilityStatement => "All first claim members of the club are eligible for this competition - any age group.";

        public static SeniorsCompetitionResultsSet CreateFrom(
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

            // filter rides must be Valid, but any ageGroup
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = championshipRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    championshipCalendar, 
                    r => r.SeniorsPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new SeniorsCompetitionResultsSet(championshipCalendar, results);
        }
    }
}

