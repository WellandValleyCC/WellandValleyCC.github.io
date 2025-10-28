using ClubProcessor.Models;

namespace ClubProcessor.Interfaces
{
    public interface ICompetitionScoreCalculator
    {
        string CompetitionName { get; }
        void ApplyScores(List<Ride> rides, Func<int, int> pointsForPosition);
    }
}
