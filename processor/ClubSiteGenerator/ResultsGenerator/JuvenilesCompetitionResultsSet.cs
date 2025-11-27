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

        public override string DisplayName => "Juveniles Competition";

        public override string FileName => $"{Year}-juveniles";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Juvenile;
        public override string CompetitionType => "Juveniles";


        //// Project event numbers, dates, and names from the calendar
        //var eventNumbers = Calendar
        //    .Select(e => e.EventNumber.ToString())
        //    .ToList();

        //var eventDates = Calendar
        //    .Select(e => e.EventDate.ToString("ddd d MMM", CultureInfo.InvariantCulture))
        //    .ToList();

        //var eventNames = Calendar
        //    .Select(e => e.EventName)
        //    .ToList();

        //var headers = new List<HtmlHeaderRow>
        //{
        //    // Row 1: event numbers
        //    new(new List<string>
        //    {
        //        "", "", "", "", "" // blanks for the fixed columns
        //    }.Concat(eventNumbers).ToList()),

        //    // Row 2: event dates
        //    new(new List<string>
        //    {
        //        "", "", "", "", "" // blanks for the fixed columns
        //    }.Concat(eventDates).ToList()),

        //    // Row 3: labels (fixed columns + optional event names)
        //    new(new List<string>
        //    {
        //        "Name",
        //        "Current rank",
        //        "Events completed",
        //        "10-mile TTs Best 8",
        //        "Scoring 11"
        //    }.Concat(eventNames).ToList())
        //};


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

            CompetitionResultsCalculator.AssignRanks(results);

            return new JuvenilesCompetitionResultsSet(calendar, results);
        }

    }
}
