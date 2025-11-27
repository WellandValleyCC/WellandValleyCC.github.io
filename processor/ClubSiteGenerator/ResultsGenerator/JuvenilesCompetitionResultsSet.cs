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
        public override string CompetitionCode => "JUV";


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


        public static JuvenilesCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> calendar)
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

            var juvenileRides = allRides
                .Where(r =>
                    r.Competitor != null &&
                    r.Competitor.IsJuvenile &&
                    r.Status == RideStatus.Valid);

            var groups = juvenileRides
                .GroupBy(r => r.Competitor!)
                .ToList();

            var results = groups
                .Select(group => CompetitionResultsCalculator.BuildCompetitorResult(group, calendar))
                .OrderByDescending(r => r.Scoring11.HasValue)               // tier 1: Scoring11 present first
                .ThenByDescending(r => r.Scoring11)                         // within tier 1, sort by score
                .ThenByDescending(r => r.Best8TenMile.HasValue)             // tier 2: Best8TenMile present next
                .ThenByDescending(r => r.Best8TenMile)                      // within tier 2, sort by score
                .ThenBy(r => ExtractSurname(r.Competitor.FullName))         // tier 3: surname
                .ThenBy(r => ExtractGivenName(r.Competitor.FullName))       // then given name
                .ToList();

            int currentRank = 1;
            double? lastScore = null;

            for (int i = 0; i < results.Count; i++)
            {
                var score = results[i].Scoring11;

                if (score == null)
                {
                    // once we hit a competitor with no valid score, stop assigning ranks
                    results[i].Rank = null;
                    continue;
                }

                if (lastScore != null && score == lastScore)
                {
                    // tie --> same rank as previous competitor
                    results[i].Rank = results[i - 1].Rank;
                }
                else
                {
                    // new score --> assign current rank
                    results[i].Rank = currentRank;
                }

                lastScore = score;
                currentRank++;
            }

            return new JuvenilesCompetitionResultsSet(calendar, results);
        }

        private static string ExtractSurname(string fullName)
        {
            var parts = (fullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2 ? parts[^1] : fullName ?? string.Empty;
        }

        private static string ExtractGivenName(string fullName)
        {
            var parts = (fullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2 ? string.Join(' ', parts.Take(parts.Length - 1)) : fullName ?? string.Empty;
        }
    }
}
