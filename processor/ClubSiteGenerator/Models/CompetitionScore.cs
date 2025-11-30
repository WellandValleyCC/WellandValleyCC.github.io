using ClubCore.Models;

namespace ClubSiteGenerator.Models
{
    /// <summary>
    /// Encapsulates rank and points for a single competition view.
    /// </summary>
    public class CompetitionScore
    {
        public int? Rank { get; set; }
        public double? Points { get; set; }
        public IReadOnlyList<Ride> Rides { get; set; } = Array.Empty<Ride>();

        public string RankDisplay =>
            Rank.HasValue ? Rank.Value.ToString() : "n/a";

        public string PointsDisplay =>
            Points.HasValue ? Math.Round(Points.Value, MidpointRounding.AwayFromZero).ToString() : "n/a";
    }
}

