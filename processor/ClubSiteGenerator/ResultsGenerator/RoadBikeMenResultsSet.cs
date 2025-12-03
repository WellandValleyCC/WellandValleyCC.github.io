using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
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
        public override string GenericName => "Road Bike Men";
        public override string CompetitionType => "Road Bike Men";

        public override string EligibilityStatement => "All first claim male members of the club riding road bikes are eligible for this championship.";

        public static RoadBikeMenCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides,
            IEnumerable<CalendarEvent> calendar)
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
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.WomenPoints))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new RoadBikeMenCompetitionResultsSet(calendar, results);
        }
    }
}

