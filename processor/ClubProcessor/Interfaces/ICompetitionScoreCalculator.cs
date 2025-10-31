using ClubProcessor.Models;

namespace ClubProcessor.Interfaces
{
    public interface ICompetitionScoreCalculator
    {
        string CompetitionName { get; }  // e.g., "Juveniles", "Veterans"

        /// <summary>
        /// Applies scores and returns the number of rides affected.
        /// </summary>
        int ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition);
    }
}
