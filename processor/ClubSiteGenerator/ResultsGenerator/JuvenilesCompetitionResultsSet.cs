using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Services;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        private JuvenilesCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides) { }

        public override string DisplayName => "Juveniles Championship";

        public override string FileName => $"{Year}-juveniles";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Juveniles";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Juvenile;
        public override string CompetitionType => "Juveniles";

        public override string EligibilityStatement => "All first claim juvenile members of the club are eligible for this championship.";

        public static JuvenilesCompetitionResultsSet CreateFrom(
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

            // filter juvenile rides
            var juvenileRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsJuvenile &&
                    r.Status == RideStatus.Valid);

            // group by competitor
            var groups = juvenileRides
                .GroupBy(r => r.Competitor!)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group, calendar))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new JuvenilesCompetitionResultsSet(calendar, results);
        }
    }
}
