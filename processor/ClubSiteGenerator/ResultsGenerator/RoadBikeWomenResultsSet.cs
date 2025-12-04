using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
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
        public override string GenericName => "Road Bike Women";
        public override CompetitionType CompetitionType => CompetitionType.RoadBikeWomen;

        public override string EligibilityStatement => "All first claim female members of the club riding road bikes are eligible for this championship.";
            
        public static RoadBikeWomenCompetitionResultsSet CreateFrom(
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

            // filter women rides on a road bike
            var championshipRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsFemale == true &&
                    r.IsRoadBike &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = championshipRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.RoadBikeWomenPoints))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new RoadBikeWomenCompetitionResultsSet(calendar, results);
        }
    }
}

