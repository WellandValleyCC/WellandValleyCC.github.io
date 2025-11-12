using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.ResultsGenerator
{
    public class EventResults : BaseResults
    {
        private readonly int _eventNumber;

        public EventResults(int eventNumber, List<Ride> rides)
            : base(rides)
        {
            _eventNumber = eventNumber;
        }

        public override string Name => $"event-{_eventNumber}";

        public override IEnumerable<Ride> Query()
            => Rides.Where(r => r.EventNumber == _eventNumber && r.Eligibility == RideEligibility.Valid);
    }
}
