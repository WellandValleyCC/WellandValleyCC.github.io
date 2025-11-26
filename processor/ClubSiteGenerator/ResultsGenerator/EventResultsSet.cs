using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class EventResultsSet : ResultsSet
    {
        private readonly CalendarEvent calendarEvent;
        public readonly IEnumerable<Ride> Rides;

        private EventResultsSet(CalendarEvent ev, IEnumerable<Ride> rides) 
        {
            this.calendarEvent = ev;
            this.Rides = rides;
        }

        public CalendarEvent CalendarEvent => calendarEvent;
        public int EventNumber => calendarEvent.EventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(calendarEvent.EventDate);

        public override string DisplayName => $"{calendarEvent.EventName}";
        public override string FileName => $"event-{calendarEvent.EventNumber:D2}";
        public override string SubFolderName => "events";

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
