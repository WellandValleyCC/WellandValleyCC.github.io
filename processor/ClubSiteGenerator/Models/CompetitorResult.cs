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

        public double? AllEventsPoints { get; set; }
        public double? TenMileCompetitionPoints { get; set; }
        public IReadOnlyList<Ride> TenMileCompetitionRides { get; set; } = Array.Empty<Ride>();

        public double? FullCompetitionPoints { get; set; }
        public IReadOnlyList<Ride> FullCompetitionRides { get; set; } = Array.Empty<Ride>();

        /// <summary>
        /// Returns a formatted string for all ridden events points - no best-n behaviour (rounded or "n/a").
        /// </summary>
        public string AllEventsPointsDisplay =>
            AllEventsPoints.HasValue ? Math.Round(AllEventsPoints.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";

        /// <summary>
        /// Returns a formatted string for TenMileCompetitionPoints (rounded or "n/a").
        /// </summary>
        public string TenMileCompetitionPointsDisplay =>
            TenMileCompetitionPoints.HasValue ? Math.Round(TenMileCompetitionPoints.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";

        /// <summary>
        /// Returns a formatted string for FullCompetitionPoints (rounded or "n/a").
        /// </summary>
        public string FullCompetitionPointsDisplay =>
            FullCompetitionPoints.HasValue ? Math.Round(FullCompetitionPoints.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";

        public string AllEventsRankDisplay =>
            AllEventsRank.HasValue ? AllEventsRank.Value.ToString() : "n/a";

        public string TenMileCompetitionRankDisplay =>
            TenMileCompetitionRank.HasValue ? TenMileCompetitionRank.Value.ToString() : "n/a";

        public string FullCompetitionRankDisplay =>
            FullCompetitionRank.HasValue ? FullCompetitionRank.Value.ToString() : "n/a";
    }
}
