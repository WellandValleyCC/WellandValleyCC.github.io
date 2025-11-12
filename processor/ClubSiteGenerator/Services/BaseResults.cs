using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public abstract class BaseResults
    {
        protected readonly List<Ride> Rides;
        protected readonly List<Competitor> Competitors;
        protected readonly List<CalendarEvent> CalendarEvents;

        protected BaseResults(List<Ride> rides, List<Competitor> competitors, List<CalendarEvent> calendarEvents)
        {
            Rides = rides;
            Competitors = competitors;
            CalendarEvents = calendarEvents;
        }

        // Friendly name for output file
        public abstract string Name { get; }

        // Query logic: which rides belong in this result set
        public abstract IEnumerable<Ride> Query();

        // Shape into a table model
        public HtmlTable CreateTable()
        {
            var headers = new List<string> { "Pos", "Name", "Club", "Time", "Points" };
            var rows = Query()
                .OrderBy(r => r.Position)
                .Select(r =>
                {
                    var competitor = Competitors.FirstOrDefault(c => c.ClubNumber == r.ClubNumber);
                    return new List<string>
                    {
                        r.Position.ToString(),
                        competitor?.Name ?? "Unknown",
                        competitor?.Club ?? "",
                        TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"mm\:ss"),
                        r.Points.ToString()
                    };
                })
                .ToList();

            return new HtmlTable(headers, rows);
        }
    }
}
