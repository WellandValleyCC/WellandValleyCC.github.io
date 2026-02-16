using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinEventResultsSet : ResultsSet
    {
        private readonly CalendarEvent roundRobinEvent;
        public readonly IEnumerable<Ride> Rides;

        private RoundRobinEventResultsSet(
            IEnumerable<CalendarEvent> roundRobinCalendar,
            IEnumerable<Ride> rides,
            int roundRobinEventNumber)
            : base(roundRobinCalendar)
        {
            this.roundRobinEvent = roundRobinCalendar.Single(
                ev => 
                    ev.RoundRobinEventNumber == roundRobinEventNumber && 
                    ev.IsRoundRobinEvent);
            this.Rides = rides;
            AssignRoundRobinRanks();
        }

        public CalendarEvent CalendarEvent => roundRobinEvent;
        public int EventNumber => roundRobinEvent.RoundRobinEventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(roundRobinEvent.EventDate);

        public override string SubFolderName => "events";

        public string CssFile { get; set; } = "";

        /// <summary>
        /// E.g. 2026-rr-event-03
        /// </summary>
        public override string FileName =>
            $"{Year}-rr-event-{EventNumber:D2}";

        public override string DisplayName => roundRobinEvent.EventName;

        public override string LinkText => $"Event {EventNumber}";

        private static IEnumerable<Ride> OrderedIneligibleRides(IEnumerable<Ride> rides, RideStatus rideStatus)
        {
            var roundRobinMembers = rides
                .Where(r =>
                    r.Status == rideStatus &&
                    !string.IsNullOrEmpty(r.RoundRobinClub))
                .OrderBy(r => NameParts.Split(r.Name).Surname)
                .ThenBy(r => NameParts.Split(r.Name).GivenNames);

            var guests = rides
                .Where(r =>
                    r.Status == rideStatus &&
                    string.IsNullOrEmpty(r.RoundRobinClub))
                .OrderBy(r => NameParts.Split(r.Name).Surname)
                .ThenBy(r => NameParts.Split(r.Name).GivenNames);

            return roundRobinMembers.Concat(guests);
        }

        // Factory: single CalendarEvent
        public static RoundRobinEventResultsSet CreateFrom(
            IEnumerable<CalendarEvent> roundRobinCalendar,
            IEnumerable<Ride> allRides,
            int eventNumber)
        {
            if (HasMissingCompetitors(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with Competitors.", nameof(allRides));

            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            var rrEvent = roundRobinCalendar.Single(ev => ev.RoundRobinEventNumber == eventNumber);

            if (!rrEvent.IsRoundRobinEvent)
                throw new InvalidOperationException(
                    $"CalendarEvent {eventNumber} is not a Round Robin event.");

            var hydratedRidesForEvent = allRides.Where(r => r.EventNumber == eventNumber);

            var ranked = hydratedRidesForEvent
                .Where(r => r.Status == RideStatus.Valid)
                .OrderBy(r => r.EventRank);

            var dnfs = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DNF);
            var dnss = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DNS);
            var dqs = OrderedIneligibleRides(hydratedRidesForEvent, RideStatus.DQ);

            var orderedHydratedRidesForEvent =
                ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            return new RoundRobinEventResultsSet(
                roundRobinCalendar,
                orderedHydratedRidesForEvent,
                eventNumber);
        }

        private void AssignRoundRobinRanks()
        {
            // RR-eligible = riders with a RoundRobinClub (not guests) and valid rides
            var rrEligible = Rides
                .Where(r => !string.IsNullOrWhiteSpace(r.RoundRobinClub) &&
                            r.Status == RideStatus.Valid)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            int currentRank = 1;

            for (int i = 0; i < rrEligible.Count; i++)
            {
                if (i > 0 && rrEligible[i].TotalSeconds == rrEligible[i - 1].TotalSeconds)
                {
                    // Same time → same rank
                    rrEligible[i].RREligibleRidersRank = rrEligible[i - 1].RREligibleRidersRank;
                }
                else
                {
                    // New time → rank = position + 1
                    rrEligible[i].RREligibleRidersRank = currentRank;
                }

                currentRank++;
            }

            // Road bike subset
            var rrEligibleRoadBike = rrEligible
                .Where(r => r.IsRoadBike)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            currentRank = 1;

            for (int i = 0; i < rrEligibleRoadBike.Count; i++)
            {
                if (i > 0 && rrEligibleRoadBike[i].TotalSeconds == rrEligibleRoadBike[i - 1].TotalSeconds)
                {
                    rrEligibleRoadBike[i].RREligibleRoadBikeRidersRank =
                        rrEligibleRoadBike[i - 1].RREligibleRoadBikeRidersRank;
                }
                else
                {
                    rrEligibleRoadBike[i].RREligibleRoadBikeRidersRank = currentRank;
                }

                currentRank++;
            }
        }
    }
}