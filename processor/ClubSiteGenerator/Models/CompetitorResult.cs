using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Models
{
    public class CompetitorResult
    {
        public Competitor Competitor { get; set; } = default!;
        public IReadOnlyList<Ride> Rides { get; set; } = Array.Empty<Ride>();

        /// <summary>
        /// Points scored per event number. If competitor did not ride, entry is absent. May be null if DNS/DNF/DQ.
        /// </summary>
        public Dictionary<int, double?> EventPoints { get; set; } = new();

        /// <summary>
        /// Status per event number (Valid, DNS, DNF, DQ).
        /// </summary>
        public Dictionary<int, RideStatus> EventStatuses { get; set; } = new();

        /// <summary>
        /// Number of events completed by this competitor.
        /// </summary>
        public int EventsCompleted { get; set; }

        // Three different views of the competitor’s results
        public CompetitionScore AllEvents { get; set; } = new CompetitionScore();
        public CompetitionScore TenMileCompetition { get; set; } = new CompetitionScore();
        public CompetitionScore FullCompetition { get; set; } = new CompetitionScore();
    }
}