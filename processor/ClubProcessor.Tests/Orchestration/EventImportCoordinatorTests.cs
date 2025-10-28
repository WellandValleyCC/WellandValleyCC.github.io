using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Orchestration;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Xunit;

public class EventImportCoordinatorTests
{
    [Fact]
    public void Run_ExecutesWithoutError_ForValidInput()
    {
        // Arrange
        var scorer = new StubCompetitionPointsCalculator();
        var coordinator = new EventImportCoordinator(scorer);

        string inputPath = "testdata/events/2025"; // Ensure this folder and Calendar_2025.csv exist
        string year = "2025";

        // Act & Assert
        coordinator.Run(inputPath, year);
    }

    private class StubCompetitionPointsCalculator : CompetitionPointsCalculator
    {
        public StubCompetitionPointsCalculator() : base(new List<ICompetitionScoreCalculator> { new NoOpCalculator() }) { }

        private class NoOpCalculator : ICompetitionScoreCalculator
        {
            public string CompetitionName => "Stub";

            public void ApplyScores(List<Ride> rides, Func<int, int> pointsForPosition)
            {
                // No-op
            }
        }
    }
}
