using ClubCore.Models;

namespace ClubProcessor.Interfaces
{
    /// <summary>
    /// Assigns display-only ranks to rides for a single event.
    /// Used for non-scoring categories like AllRider or RoadBike.
    /// </summary>
    public interface IEventRankCalculator
    {
        /// <summary>
        /// Applies ranking logic to the provided rides.
        /// Returns the number of rides that received a rank.
        /// </summary>
        int AssignRanks(int eventNumber, List<Ride> rides);
    }
}