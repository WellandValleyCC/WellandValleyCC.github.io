using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class BaseResults
    {
        protected readonly List<Ride> Rides;

        protected BaseResults(List<Ride> rides)
        {
            Rides = rides;
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
                .OrderBy(r => r.EventRank)
                .Select(r =>
                {
                    return new List<string>
                    {
                        r.Name ?? "Unknown",
                        TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"mm\:ss")
                    };
                })
                .ToList();

            return new HtmlTable(headers, rows);
        }
    }
}
