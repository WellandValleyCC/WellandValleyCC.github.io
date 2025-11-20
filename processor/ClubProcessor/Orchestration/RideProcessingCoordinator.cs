using ClubProcessor.Interfaces;
using ClubCore.Models;

namespace ClubProcessor.Orchestration
{
    /// <summary>
    /// Coordinates all ride processing logic, including competition scoring and event ranking.
    /// </summary>
    public class RideProcessingCoordinator
    {
        private readonly List<IRideProcessor> processors;
        private readonly Func<int, int> pointsForPosition;

        /// <summary>
        /// Constructs the coordinator with all registered ride processors.
        /// </summary>
        /// <param name="processors">Injected processors for scoring and ranking.</param>
        /// <param name="pointsForPosition">Function yielding the points to use for each position.</param>
        public RideProcessingCoordinator(IEnumerable<IRideProcessor> processors, Func<int, int> pointsForPosition)
        {
            this.processors = processors.ToList();
            this.pointsForPosition = pointsForPosition;
        }

        /// <summary>
        /// Applies all ride processing logic to the provided rides.
        /// </summary>
        /// <param name="rides">All rides for the event.</param>
        /// <param name="competitors">All registered competitors.</param>
        /// <param name="calendarEvents">All events in the calendar.</param>
        public void ProcessAll(
            IEnumerable<Ride> rides, 
            IEnumerable<Competitor> competitors, 
            IEnumerable<CalendarEvent> calendarEvents)
        {
            RideHydrationHelper.HydrateCalendarEvents(rides, calendarEvents);
            RideHydrationHelper.HydrateCompetitors(rides, competitors);

            var ridesByEvent = rides
                .GroupBy(r => r.EventNumber)
                .OrderBy(g => g.Key);

            Console.WriteLine("Processing rides...");

            foreach (var processor in processors)
            {
                Console.WriteLine($"  Processor: {processor.GetType().Name}");

                foreach (var group in ridesByEvent)
                {
                    int eventNumber = group.Key;
                    int affected = processor.ProcessEvent(eventNumber, group.ToList());
                    Console.WriteLine($"    Event {eventNumber}: {affected} rides affected");
                }
            }
        }
    }
}

