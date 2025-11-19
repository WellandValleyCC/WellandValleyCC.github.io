using ClubCore.Models;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> EventsCalendar;

        protected CompetitionResultsSet(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> eventsCalendar)
            : base(allRides)
        {
            EventsCalendar = eventsCalendar;
        }

        // Each specialization defines which property to read from Ride
        protected abstract double? GetPoints(Ride ride);

        // Common scoring logic: best 11 events
        protected double CalculateTotalPoints(IEnumerable<Ride> rides)
        {
            return rides
                .Select(r => GetPoints(r) ?? 0)
                .OrderByDescending(p => p)
                .Take(11)
                .Sum();
        }

        public override HtmlTable CreateTable()
        {
            var headers = new List<string> { "Name" };
            headers.AddRange(EventsCalendar.Select(e => e.EventName));
            headers.Add("Total (Best 11)");

            var rows = FilteredRides()
                .GroupBy(r => r.Competitor)
                .Select(g =>
                {
                    var cells = new List<string> { $"{g.Key?.GivenName} {g.Key?.Surname}" };

                    foreach (var ev in EventsCalendar)
                    {
                        var ride = g.FirstOrDefault(r => r.EventNumber == ev.EventNumber);
                        cells.Add(ride != null ? (GetPoints(ride)?.ToString() ?? "") : "");
                    }

                    var total = CalculateTotalPoints(g);
                    cells.Add(total.ToString());

                    return new HtmlRow(cells, g.First());
                });

            return new HtmlTable(headers, rows);
        }
    }
}
