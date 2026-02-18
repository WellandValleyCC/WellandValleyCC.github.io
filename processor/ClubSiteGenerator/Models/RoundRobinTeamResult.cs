using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Models.RoundRobin
{
    public class RoundRobinTeamResult
    {
        /// <summary>
        /// Short club name (e.g. "Ratae", "WVCC").
        /// </summary>
        public required string ClubShortName { get; set; }

        /// <summary>
        /// All riders contributing to this team (DB or synthetic).
        /// </summary>
        public IReadOnlyList<RoundRobinRider> Riders { get; set; }
            = Array.Empty<RoundRobinRider>();

        /// <summary>
        /// Team points per event number.
        /// </summary>
        public Dictionary<int, double?> EventPoints { get; set; }
            = new();

        /// <summary>
        /// Status per event number (Valid, DNS, DNF, DQ).
        /// </summary>
        public Dictionary<int, RideStatus> EventStatuses { get; set; }
            = new();

        /// <summary>
        /// Total number of events completed by the team.
        /// </summary>
        public int EventsCompleted { get; set; }

        /// <summary>
        /// The team’s overall RR score.
        /// </summary>
        public CompetitionScore Total { get; set; }
            = new CompetitionScore();

        /// <summary>
        /// The N Open-category riders whose scores contributed to the team total for each event
        /// </summary>
        public Dictionary<int, IReadOnlyList<RoundRobinRiderScore>> ContributingOpenRidesByEvent { get; set; } = new();

        /// <summary>
        /// The M Women-category riders whose scores contributed to the team total for each event
        /// </summary>
        public Dictionary<int, IReadOnlyList<RoundRobinRiderScore>> ContributingWomenRidesByEvent { get; set; } = new();
    }
}
