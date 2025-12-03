using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class SeniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private SeniorsCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides)
        {
        }

        public override string DisplayName => "Seniors Championship";
        public override string FileName => $"{Year}-seniors";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Seniors";
        public override string CompetitionType => "Seniors";

        public override string EligibilityStatement => "All first claim members of the club are eligible for this championship - any age group.";

        public static SeniorsCompetitionResultsSet CreateFrom(
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

            // filter rides m- must be Valid, but any ageGroup
            var Championship = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = Championship
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.SeniorsPoints))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new SeniorsCompetitionResultsSet(calendar, results);
        }
    }
}

