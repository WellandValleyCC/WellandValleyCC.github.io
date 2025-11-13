using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class BaseResults
    {
        protected readonly List<Ride> AllRides;

        protected BaseResults(List<Ride> allRides)
        {
            AllRides = allRides;
        }

        // DisplayName for this result set
        public abstract string DisplayName { get; }

        // Friendly name for output file
        public abstract string FileName { get; }

        public abstract string SubFolderName { get; }

        // Query logic: which rides belong in this result set
        public abstract IEnumerable<Ride> EventRides();

        // Shape into a table model
        public HtmlTable CreateTable()
        {
            var headers = new List<string>
            {
                "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
            };

            var rows = EventRides()
                .OrderBy(r => r.EventRank)
                .Select(r =>
                {
                    var miles = r.CalendarEvent?.Miles ?? 0;
                    var avgMph = r.TotalSeconds > 0 && miles > 0
                        ? (miles / (r.TotalSeconds / 3600)).ToString("0.00")
                        : "";

                    var cells = new List<string>
                    {
                        r.Name ?? "Unknown",
                        r.EventRank?.ToString() ?? "",
                        r.EventRoadBikeRank?.ToString() ?? "",
                        TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"hh\:mm\:ss"),
                        avgMph
                    };

                    return new HtmlRow(cells, r); // attach Ride metadata
                });

            return new HtmlTable(headers, rows);
        }

    }
}
