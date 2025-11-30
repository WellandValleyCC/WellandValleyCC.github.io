using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Models
{
    public class CompetitorResult
    {
        public Competitor Competitor { get; set; } = default!;
        public IReadOnlyList<Ride> Rides { get; set; } = Array.Empty<Ride>();

        /// <summary>
        /// Assigned rank including all events (null if not ranked).
        /// </summary>
        public int? AllEventsRank { get; set; }

        /// <summary>
        /// Assigned rank for the Ten-mile events subject to competition limits (best 8 events in 2025) (null if not ranked).
        /// </summary>
        public int? TenMileCompetitionRank { get; set; }

        /// <summary>
        /// Assigned rank for the full competition subject to competition limits (best 11 events in 2025) (null if not ranked).
        /// </summary>
        public int? FullCompetitionRank { get; set; }

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

        public double? Best8TenMile { get; set; }
        public IReadOnlyList<Ride> Best8TenMileRides { get; set; } = Array.Empty<Ride>();

        public double? Scoring11 { get; set; }
        public IReadOnlyList<Ride> Scoring11Rides { get; set; } = Array.Empty<Ride>();

        /// <summary>
        /// Returns a formatted string for Best8TenMile (rounded or "n/a").
        /// </summary>
        public string Best8TenMileDisplay =>
            Best8TenMile.HasValue ? Math.Round(Best8TenMile.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";

        /// <summary>
        /// Returns a formatted string for Scoring11 (rounded or "n/a").
        /// </summary>
        public string Scoring11Display =>
            Scoring11.HasValue ? Math.Round(Scoring11.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";

        public string AllEventsRankDisplay =>
            AllEventsRank.HasValue ? AllEventsRank.Value.ToString() : "n/a";

        public string TenMileCompetitionRankDisplay =>
            TenMileCompetitionRank.HasValue ? TenMileCompetitionRank.Value.ToString() : "n/a";

        public string FullCompetitionRankDisplay =>
            FullCompetitionRank.HasValue ? FullCompetitionRank.Value.ToString() : "n/a";
    }
}
