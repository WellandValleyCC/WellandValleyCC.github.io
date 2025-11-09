using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Orchestration;

namespace ClubProcessor.Tests
{
    public class EventImportCoordinatorTests
    {
        [Fact]
        public void Run_ExecutesWithoutError_ForValidInput()
        {
            // Arrange
            var scorer = new StubCompetitionPointsCalculator();
            var coordinator = new EventImportCoordinator();

            string inputPath = "testdata/events/2025"; // Ensure this folder and Calendar_2025.csv exist
            string year = "2025";

            // Ensure DB directories exist
            var eventDbPath = Path.Combine("data", $"club_events_{year}.db");
            var competitorDbPath = Path.Combine("data", $"club_competitors_{year}.db");

            Directory.CreateDirectory(Path.GetDirectoryName(eventDbPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(competitorDbPath)!);

            // Optional: delete old test DBs
            if (File.Exists(eventDbPath)) File.Delete(eventDbPath);
            if (File.Exists(competitorDbPath)) File.Delete(competitorDbPath);

            // Act & Assert
            coordinator.Run(inputPath, year);
        }


        private class StubCompetitionPointsCalculator : RideProcessingCoordinator
        {
            public StubCompetitionPointsCalculator()
                : base(
                      new List<IRideProcessor> { new NoOpProcessor() },
                      position => 0 // Stubbed pointsForPosition delegate
                      )
            { }

            private class NoOpProcessor : ICompetitionScoreCalculator, IRideProcessor
            {
                public string CompetitionName => "Stub";

                public int ApplyScores(int eventNumber, List<Ride> rides, Func<int, int> pointsForPosition)
                {
                    // No-op
                    return 0;
                }

                public int ProcessEvent(int eventNumber, List<Ride> eventRides)
                {
                    // No-op
                    return 0;
                }
            }
        }
    }
}