using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet
    {
        protected readonly IEnumerable<Ride> AllRides;

        protected ResultsSet(IEnumerable<Ride> allRides)
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

            var rides = EventRides().ToList();

            var ranked = rides.Where(r => r.Eligibility == RideEligibility.Valid)
                              .OrderBy(r => r.EventRank);

            var dnfs = OrderedIneligibleRides(rides, RideEligibility.DNF);
            var dnss = OrderedIneligibleRides(rides, RideEligibility.DNS);
            var dqs = OrderedIneligibleRides(rides, RideEligibility.DQ);

            var ordered = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            var rows = ordered
                .Select(r =>
                {
                    var miles = r.CalendarEvent?.Miles ?? 0;
                    var avgMph = (r.Eligibility == RideEligibility.Valid && r.TotalSeconds > 0 && miles > 0)
                        ? (miles / (r.TotalSeconds / 3600)).ToString("0.00")
                        : string.Empty;

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

        public static IEnumerable<Ride> OrderedIneligibleRides(IEnumerable<Ride> eventRides, RideEligibility rideEligibility)
        {
            var dxxCompetitionEligible = eventRides
                .Where(r =>
                    r.Eligibility == rideEligibility &&
                    r.EventEligibleRidersRank != null)
                .OrderBy(dnf => dnf.Competitor!.Surname)
                .ThenBy(dnf => dnf.Competitor!.GivenName);

            var dxxSecondClaim = eventRides
                .Where(r =>
                    r.Eligibility == rideEligibility &&
                    r.EventEligibleRidersRank == null &&
                    r.ClubNumber != null)
                .OrderBy(dnf => dnf.Name);

            var dxxGuest = eventRides
                .Where(r =>
                    r.Eligibility == rideEligibility &&
                    r.EventEligibleRidersRank == null &&
                    r.ClubNumber == null)
                .OrderBy(dnf => dnf.Name);

            return dxxCompetitionEligible.Concat(dxxSecondClaim).Concat(dxxGuest);
        }
    }
}
