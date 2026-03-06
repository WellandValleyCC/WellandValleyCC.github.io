using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;

namespace ClubSiteGenerator.ResultsGenerator.RoundRobin
{
    public sealed class RoundRobinEventResultsSet : RoundRobinResultsSet
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
            int roundRobinEventNumber)
        {
            if (HasMissingCompetitors(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with Competitors.", nameof(allRides));

            if (HasMissingCalendarEvents(allRides))
                throw new ArgumentException($"{nameof(allRides)} must be hydrated with CalendarEvents.", nameof(allRides));

            var rrEvent = roundRobinCalendar.Single(ev => ev.RoundRobinEventNumber == roundRobinEventNumber);

            var fullSeasonEventNumber = rrEvent.EventNumber;

            if (!rrEvent.IsRoundRobinEvent)
                throw new InvalidOperationException(
                    $"CalendarEvent {roundRobinEventNumber} is not a Round Robin event.");

            var hydratedRidesForEvent = allRides.Where(r => r.EventNumber == fullSeasonEventNumber);

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
                roundRobinEventNumber);
        }

        private void AssignRoundRobinRanks()
        {
            // RR-eligible = riders with a RoundRobinClub (not guests) and valid rides
            var rrEligible = Rides
                .Where(r => !string.IsNullOrWhiteSpace(r.RoundRobinClub) &&
                            r.Status == RideStatus.Valid)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            AssignRanksWithTies(
                rrEligible,
                (ride, rank) => ride.RREligibleRidersRank = rank
            );

            // Road bike subset
            var rrEligibleRoadBike = rrEligible
                .Where(r => r.IsRoadBike)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            AssignRanksWithTies(
                rrEligibleRoadBike,
                (ride, rank) => ride.RREligibleRoadBikeRidersRank = rank
            );
        }

        private static void AssignRanksWithTies(
            IList<Ride> rides,
            Action<Ride, int> assignRank)
        {
            if (rides.Count == 0)
                return;

            int currentRank = 1;

            for (int i = 0; i < rides.Count; i++)
            {
                if (i > 0 && rides[i].TotalSeconds == rides[i - 1].TotalSeconds)
                {
                    // Same time → same rank
                    assignRank(rides[i], rides[i - 1].RREligibleRidersRank!.Value);
                }
                else
                {
                    // New time → new rank
                    assignRank(rides[i], currentRank);
                }

                currentRank++;
            }
        }
    }
}