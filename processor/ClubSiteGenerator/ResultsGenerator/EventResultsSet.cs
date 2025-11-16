using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public class EventResultsSet : ResultsSet
    {
        public readonly CalendarEvent CalendarEvent;

        public EventResultsSet(int eventNumber, IEnumerable<CalendarEvent> eventsCalendar, IEnumerable<Ride> rides)
            : base(rides)
        {
            CalendarEvent = eventsCalendar.Single(e => e.EventNumber == eventNumber);
        }

        public override string DisplayName => CalendarEvent.EventName;
        public override string FileName => $"event-{CalendarEvent.EventNumber:D2}";
        public override string SubFolderName => "events";

        public int EventNumber => CalendarEvent.EventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(CalendarEvent.EventDate);

        public override IEnumerable<Ride> FilteredRides()
            => AllRides.Where(r => r.EventNumber == CalendarEvent.EventNumber);

        public override HtmlTable CreateTable()
        {
            var headers = new List<string>
            {
                "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
            };

            var rides = FilteredRides().ToList();

            var ranked = rides.Where(r => r.Eligibility == RideEligibility.Valid)
                              .OrderBy(r => r.EventRank);

            var dnfs = OrderedIneligibleRides(rides, RideEligibility.DNF);
            var dnss = OrderedIneligibleRides(rides, RideEligibility.DNS);
            var dqs = OrderedIneligibleRides(rides, RideEligibility.DQ);

            var ordered = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            var rows = ordered.Select(r =>
            {
                var miles = r.CalendarEvent?.Miles ?? 0;
                var avgMph = (r.Eligibility == RideEligibility.Valid && r.TotalSeconds > 0 && miles > 0)
                    ? (miles / (r.TotalSeconds / 3600)).ToString("0.00")
                    : string.Empty;

                var timeCell = r.Eligibility switch
                {
                    RideEligibility.DNF => "DNF",
                    RideEligibility.DNS => "DNS",
                    RideEligibility.DQ => "DQ",
                    _ => TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"hh\:mm\:ss")
                };

                var cells = new List<string>
                {
                    r.Name ?? "Unknown",
                    r.EventRank?.ToString() ?? "",
                    r.EventRoadBikeRank?.ToString() ?? "",
                    timeCell,
                    avgMph
                };

                return new HtmlRow(cells, r);
            });

            return new HtmlTable(headers, rows);
        }
    }
}