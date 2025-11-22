using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    // -------------------- Event Results --------------------

    public sealed class EventResultsSet : ResultsSet
    {
        private readonly CalendarEvent calendarEvent;

        private EventResultsSet(CalendarEvent ev, IEnumerable<Ride> rides) : base(rides)
        {
            this.calendarEvent = ev;
        }

        public CalendarEvent CalendarEvent => calendarEvent;
        public int EventNumber => calendarEvent.EventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(calendarEvent.EventDate);

        public override string DisplayName => $"Results for {calendarEvent.EventName}";
        public override string FileName => $"event-{calendarEvent.EventNumber:D2}";
        public override string SubFolderName => "events";

        public override HtmlTable CreateTable()
        {
            var headerRow = new List<string>
            {
                "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
            };
            var headers = new List<HtmlHeaderRow> { new HtmlHeaderRow(headerRow) };

            var rides = Rides.ToList();

            var ranked = rides.Where(r => r.Status == RideStatus.Valid)
                              .OrderBy(r => r.EventRank);

            var dnfs = OrderedIneligibleRides(rides, RideStatus.DNF);
            var dnss = OrderedIneligibleRides(rides, RideStatus.DNS);
            var dqs = OrderedIneligibleRides(rides, RideStatus.DQ);

            var ordered = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            var rows = ordered.Select(r =>
            {
                var miles = r.CalendarEvent?.Miles ?? 0;
                var avgMph = r.AvgSpeed?.ToString("0.00") ?? string.Empty;

                var timeCell = r.Status switch
                {
                    RideStatus.DNF => "DNF",
                    RideStatus.DNS => "DNS",
                    RideStatus.DQ => "DQ",
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

        private static IEnumerable<Ride> OrderedIneligibleRides(IEnumerable<Ride> rides, RideStatus eligibility)
        {
            var firstClaim = rides
                .Where(r => r.Status == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.FirstClaim)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var secondClaim = rides
                .Where(r => r.Status == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.SecondClaim)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var guests = rides
                .Where(r => r.Status == eligibility && r.ClubNumber == null)
                .OrderBy(r =>
                {
                    var parts = (r.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 0 ? parts[^1] : "";   // surname = last token
                })
                .ThenBy(r =>
                {
                    var parts = (r.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length > 1
                        ? string.Join(" ", parts.Take(parts.Length - 1)) // given names = everything before surname
                        : r.Name ?? "";
                });


            return firstClaim.Concat(secondClaim).Concat(guests);
        }

        // Factory: single CalendarEvent
        public static EventResultsSet CreateFrom(CalendarEvent ev, IEnumerable<Ride> allRides)
        {
            var hydratedRidesForEvent = allRides.Where(r => r.EventNumber == ev.EventNumber);

            var ranked = hydratedRidesForEvent.Where(r => r.Status == RideStatus.Valid)
                  .OrderBy(r => r.EventRank);
            var dnfs = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DNF);
            var dnss = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DNS);
            var dqs = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DQ);

            var orderedHydratedRidesForEvent = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            return new EventResultsSet(ev, orderedHydratedRidesForEvent);
        }
    }

}
