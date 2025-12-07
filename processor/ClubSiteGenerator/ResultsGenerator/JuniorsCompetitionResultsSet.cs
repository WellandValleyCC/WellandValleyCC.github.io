using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Rules;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class JuniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private JuniorsCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides) 
        { 
        }

        public override string DisplayName => "Club Championship - Juniors";
        public override string FileName => $"{Year}-juniors";
        public override string SubFolderName => "competitions";

        public override string GenericName => "Juniors";
        public override CompetitionType CompetitionType => CompetitionType.Juniors;

        public override string EligibilityStatement => "All first claim junior members of the club are eligible for this competition.";

        public static JuniorsCompetitionResultsSet CreateFrom(
            IEnumerable<Ride> allRides, 
            IEnumerable<CalendarEvent> calendar,
            ICompetitionRules rules)
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

            // filter junior rides
            var juniorRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsJunior &&
                    r.Status == RideStatus.Valid);

            // group by ClubNumber
            var groups = juniorRides
                .GroupBy(r => r.Competitor!.ClubNumber)
                .ToList();

            // build results
            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(
                    group.ToList(), 
                    calendar, 
                    r => r.JuniorsPoints,
                    rules))
                .ToList();

            results = CompetitionResultsCalculator.SortResults(results).ToList();

            return new JuniorsCompetitionResultsSet(calendar, results);
        }
    }
}
