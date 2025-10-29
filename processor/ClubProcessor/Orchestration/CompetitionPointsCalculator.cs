using ClubProcessor.Interfaces;
using ClubProcessor.Models;

namespace ClubProcessor.Orchestration
{
    /// <summary>
    /// Orchestrates scoring across all club competitions by delegating to category-specific calculators.
    /// </summary>
    public class CompetitionPointsCalculator
    {
        private readonly List<ICompetitionScoreCalculator> calculators;

        /// <summary>
        /// Constructs the orchestrator with all registered competition calculators.
        /// </summary>
        /// <param name="calculators">Injected calculators for each competition.</param>
        public CompetitionPointsCalculator(IEnumerable<ICompetitionScoreCalculator> calculators)
        {
            this.calculators = calculators.ToList();
        }

        /// <summary>
        /// Applies scoring logic for all competitions to the provided rides.
        /// </summary>
        /// <param name="rides">All rides for the event.</param>
        /// <param name="competitors">All registered competitors.</param>
        /// <param name="calendarEvents">All events in the calendar.</param>
        /// <param name="pointsForPosition">Delegate that returns points for a given position.</param>
        public void ScoreAllCompetitions(List<Ride> rides, List<Competitor> competitors, List<CalendarEvent> calendarEvents, Func<int, int> pointsForPosition)
        {
            RideHydrationHelper.HydrateCompetitors(rides, competitors);
            RideHydrationHelper.HydrateCalendarEvents(rides, calendarEvents);

            foreach (var calculator in calculators)
            {
                calculator.ApplyScores(rides, pointsForPosition);
            }
        }
    }
}

