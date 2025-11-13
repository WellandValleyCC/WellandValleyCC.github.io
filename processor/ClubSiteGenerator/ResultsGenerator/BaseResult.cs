using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class BaseResults
    {
        protected readonly IEnumerable<Ride> AllRides;

        protected BaseResults(IEnumerable<Ride> allRides)
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

            var ranked = EventRides()
                .Where(r => r.Eligibility == RideEligibility.Valid)
                .OrderBy(r => r.EventRank);

            var dnfs = EventRides().Where(r => r.Eligibility == RideEligibility.DNF);
            var dnss = EventRides().Where(r => r.Eligibility == RideEligibility.DNS);
            var dqs = EventRides().Where(r => r.Eligibility == RideEligibility.DQ);

            var ordered = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            var rows = ordered
                .Select(r =>
                {
                    var miles = r.CalendarEvent?.Miles ?? 0;
                    var avgMph = r.TotalSeconds > 0 && miles > 0
                        ? (miles / (r.TotalSeconds / 3600)).ToString("0.00")
                        : "";

                    var timeCell = r.Eligibility switch
                    {
                        RideEligibility.DNF => "DNF",
                        RideEligibility.DNS => "DNS",
                        RideEligibility.DQ => "DQ",
                        _ => TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"hh\:mm\:ss")
                    };

                    var cells = new List<string>
                    {
                        r.Name ?? "Unknown",
                        r.EventRank?.ToString() ?? "",
                        r.EventRoadBikeRank?.ToString() ?? "",
                        timeCell,
                        avgMph
                    };

                    return new HtmlRow(cells, r); // attach Ride metadata
                });

            return new HtmlTable(headers, rows);
        }
    }
}
