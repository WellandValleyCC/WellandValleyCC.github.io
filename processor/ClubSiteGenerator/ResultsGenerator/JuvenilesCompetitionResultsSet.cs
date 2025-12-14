using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        private JuvenilesCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides) { }

        public override string DisplayName => "Club Championship - Juveniles";
        public override string FileName => $"{Year}-juveniles";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Juveniles";
        public override CompetitionType CompetitionType => CompetitionType.Juveniles;

        public override string EligibilityStatement => "All first claim juvenile members of the club are eligible for this competition.";

        public static JuvenilesCompetitionResultsSet CreateFrom(
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

            // filter juvenile rides
            var juvenileRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsJuvenile &&
                    r.Status == RideStatus.Valid);
 
            // group by ClubNumber
            var groups = juvenileRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    championshipCalendar, 
                    r => r.JuvenilesPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new JuvenilesCompetitionResultsSet(championshipCalendar, results);
        }
    }
}
