using ClubCore.Models;

namespace ClubSiteGenerator.Models
{
    public class CompetitorResult
    {
        public Competitor Competitor { get; set; } = default!;
        public IReadOnlyList<Ride> Rides { get; set; } = Array.Empty<Ride>();

        public double? Best8TenMile { get; set; }
        public IReadOnlyList<Ride> Best8TenMileRides { get; set; } = Array.Empty<Ride>();

        public double? Scoring11 { get; set; }
        public IReadOnlyList<Ride> Scoring11Rides { get; set; } = Array.Empty<Ride>();
    }


}
