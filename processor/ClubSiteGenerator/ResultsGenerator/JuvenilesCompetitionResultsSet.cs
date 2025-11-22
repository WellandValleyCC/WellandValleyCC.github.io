using ClubCore.Models;
using ClubCore.Models.Enums;
using System.Globalization;

namespace ClubSiteGenerator.ResultsGenerator
{
    // Juveniles
    public sealed class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        private JuvenilesCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> calendar)
            : base(rides, calendar) { }

        public override string DisplayName => "Juveniles Competition";
        public override string FileName => "juveniles";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Juvenile;
        public override string CompetitionCode => "JUV";

        // Factory: full calendar
        public static JuvenilesCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            var juvenileRides = allRides
                .Where(r => 
                    r.Competitor != null
                    && r.Competitor.IsJuvenile
                    && r.Status == RideStatus.Valid);


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

            //var groupedByCompetitor = Rides
            //    .Where(r => r.Competitor != null && r.Competitor.IsJuvenile)
            //    .GroupBy(r => r.Competitor!)
            //    .ToList();

            //var rows = new List<HtmlRow>();

            //foreach (var group in groupedByCompetitor)
            //{
            //    var competitor = group.Key;

            //    // Map eventNumber to points
            //    var pointsByEvent = group.ToDictionary(
            //        r => r.EventNumber,
            //        r => r.JuvenilesPoints);

            //    // Build event cells aligned to calendar
            //    var eventCells = Calendar
            //        .Select(ev => pointsByEvent.TryGetValue(ev.EventNumber, out var pts)
            //            ? (pts?.ToString() ?? string.Empty)
            //            : string.Empty)
            //        .ToList();

            //    // Calculate totals
            //    var best8TenMile = group
            //        .Where(r => Calendar.First(ev => ev.EventNumber == r.EventNumber).IsEvening10)
            //        .OrderByDescending(r => r.JuvenilesPoints)
            //        .Take(8)
            //        .Sum(r => r.JuvenilesPoints);

            //    var nonTenMileBest2 = group
            //        .Where(r => !Calendar.First(ev => ev.EventNumber == r.EventNumber).IsEvening10)
            //        .OrderByDescending(r => r.JuvenilesPoints)
            //        .Take(2)
            //        .Sum(r => r.JuvenilesPoints);

            //    var remainingBest9 = group
            //        .OrderByDescending(r => r.JuvenilesPoints)
            //        .Skip(2) // after taking best 2 non‑10s
            //        .Take(9)
            //        .Sum(r => r.JuvenilesPoints);

            //    var scoring11 = nonTenMileBest2 + remainingBest9;

            //    // Build row cells
            //    var cells = new List<string>
            //    {
            //        competitor.FullName,
            //        "", // rank will be filled after sorting
            //        group.Count().ToString(), // events completed
            //        best8TenMile?.ToString() ?? "",
            //        scoring11?.ToString() ?? ""
            //    };
            //    cells.AddRange(eventCells);

            //    rows.Add(new HtmlRow(cells, competitor));


                return new JuvenilesCompetitionResultsSet(juvenileRides, events);
        }
    }

}
