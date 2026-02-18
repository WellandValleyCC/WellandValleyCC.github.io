using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Models.RoundRobin
{
    public class RoundRobinRiderResult
    {
        /// <summary>
        /// The rider this result belongs to (DB or synthetic WVCC rider).
        /// </summary>
        public required RoundRobinRider Rider { get; set; }

        /// <summary>
        /// All rides completed by this rider across the RR season.
        /// </summary>
        public IReadOnlyList<Ride> Rides { get; set; } = Array.Empty<Ride>();

        /// <summary>
        /// Points scored per event number. Null means DNS/DNF/DQ.
        /// </summary>
        public Dictionary<int, double?> EventPoints { get; set; } = new();

        /// <summary>
        /// Status per event number (Valid, DNS, DNF, DQ).
        /// </summary>
        public Dictionary<int, RideStatus> EventStatuses { get; set; } = new();

        /// <summary>
        /// Total number of events completed (Valid rides only).
        /// </summary>
        public int EventsCompleted { get; set; }

        /// <summary>
        /// The rider’s overall RR score.
        /// </summary>
        public CompetitionScore Total { get; set; } = new CompetitionScore();
    }
}
