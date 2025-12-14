using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class RoadBikeMenCompetitionResultsSet : CompetitionResultsSet
    {
        private RoadBikeMenCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship - Road Bike Men";
        public override string FileName => $"{Year}-road-bike-men";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Road Bike Men";
        public override CompetitionType CompetitionType => CompetitionType.RoadBikeMen;

        public override string EligibilityStatement => "All first claim male members of the club riding road bikes are eligible for this competition.";

        public static RoadBikeMenCompetitionResultsSet CreateFrom(
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

            // filter men rides on a road bike
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsFemale == false &&
                    r.IsRoadBike &&
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
                    r => r.RoadBikeMenPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new RoadBikeMenCompetitionResultsSet(championshipCalendar, results);
        }
    }
}

