using ClubCore.Models;

namespace ClubCore.Utilities
{
    public sealed record NevBrooksCell
    {
        public string Display { get; init; } = "";
        public Ride? Ride { get; init; }
    }
}
