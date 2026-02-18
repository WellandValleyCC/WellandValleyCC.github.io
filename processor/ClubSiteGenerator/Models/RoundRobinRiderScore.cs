using ClubCore.Models;

namespace ClubSiteGenerator.Models.RoundRobin
{
    /// <summary>
    /// A rider and the points they contributed to a team score.
    /// </summary>
    public class RoundRobinRiderScore
    {
        public required RoundRobinRider Rider { get; init; }
        public required double Points { get; init; }
    }
}
