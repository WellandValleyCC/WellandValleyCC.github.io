using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class RoadBikeWomenCompetitionResultsSet : CompetitionResultsSet
    {
        private RoadBikeWomenCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Club Championship - Road Bike Women";
        public override string FileName => $"{Year}-road-bike-women";
        public override string SubFolderName => "competitions";
        public override string LinkText => "Road Bike Women";
        public override CompetitionType CompetitionType => CompetitionType.RoadBikeWomen;

        public override string EligibilityStatement => "All first claim female members of the club riding road bikes are eligible for this competition.";
            
        public static RoadBikeWomenCompetitionResultsSet CreateFrom(
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

            // filter women rides on a road bike
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor is { } c &&
                    c.IsEligible() &&
                    r.Competitor.IsFemale == true &&
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
                    r => r.RoadBikeWomenPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new RoadBikeWomenCompetitionResultsSet(championshipCalendar, results);
        }
    }
}

