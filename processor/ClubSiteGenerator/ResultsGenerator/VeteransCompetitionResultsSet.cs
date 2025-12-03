using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
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
        public override string GenericName => "Veterans";
        public override string CompetitionType => "Veterans";

        public override string EligibilityStatement => "All first claim veteran members of the club are eligible for this championship.";

        public static VeteransCompetitionResultsSet CreateFrom(
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

            // filter veteran rides
            var veteranRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsVeteran &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = veteranRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.VeteransPoints))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new VeteransCompetitionResultsSet(calendar, results);
        }
    }
}

