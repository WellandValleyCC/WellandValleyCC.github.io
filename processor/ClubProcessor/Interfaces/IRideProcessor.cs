using ClubProcessor.Models;

namespace ClubProcessor.Interfaces
{
    /// <summary>
    /// Represents a processor that modifies rides — either by assigning ranks, scores, flags, or other metadata.
    /// Used to unify scoring and ranking calculators under a common orchestration model.
    /// </summary>
    public interface IRideProcessor
    {
        /// <summary>
        /// Applies processing logic to rides for a single event.
        /// Returns the number of rides affected.
        /// </summary>
        int ProcessEvent(int eventNumber, List<Ride> eventRides);
    }
}
