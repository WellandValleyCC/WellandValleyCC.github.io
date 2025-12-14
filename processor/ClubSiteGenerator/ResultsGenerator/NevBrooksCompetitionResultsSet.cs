using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class NevBrooksCompetitionResultsSet : CompetitionResultsSet
    {
        private NevBrooksCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship -  Nev Brooks Handicap";
        public override string FileName => $"{Year}-nev-brooks";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Nev Brooks";
        public override CompetitionType CompetitionType => CompetitionType.NevBrooks;

        public override string EligibilityStatement => "All first claim members of the club are eligible for this competition - any age group.";

        public static NevBrooksCompetitionResultsSet CreateFrom(
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

            // build the competition-specific calendar: all 10-mile evening events after the first
            var nevBrooksCalendar = championshipCalendar
                .Where(e => e.IsEvening10)
                .OrderBy(e => e.EventNumber)
                .Skip(1) // skip the first 10-mile TT
                .ToList();

            var relevantTenMileEventNumbers = nevBrooksCalendar
                .Select(e => e.EventNumber)
                .ToHashSet();

            // filter rides: must be Valid, any ageGroup, and event is one of the tight calendar events
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Status == RideStatus.Valid &&
                    r.NevBrooksPoints != null &&
                    r.CalendarEvent != null &&
                    relevantTenMileEventNumbers.Contains(r.CalendarEvent.EventNumber));

            // group by ClubNumber
            var groups = championshipRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(),
                    nevBrooksCalendar, // pass the tight calendar here
                    r => r.NevBrooksPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new NevBrooksCompetitionResultsSet(nevBrooksCalendar, results);
        }
    }
}