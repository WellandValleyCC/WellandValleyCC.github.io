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

        // Metadata for output
        public abstract string DisplayName { get; }
        public abstract string FileName { get; }
        public abstract string SubFolderName { get; }

        // Query logic: which rides belong in this result set
        public abstract IEnumerable<Ride> FilteredRides();

        // Each subclass defines how to shape its table
        public abstract HtmlTable CreateTable();

        // Shared helper for ordering ineligible rides
        protected static IEnumerable<Ride> OrderedIneligibleRides(
            IEnumerable<Ride> rides, RideEligibility eligibility)
        {
            var competitionEligible = rides
                .Where(r => r.Eligibility == eligibility && r.EventEligibleRidersRank != null)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var secondClaim = rides
                .Where(r => r.Eligibility == eligibility && r.EventEligibleRidersRank == null && r.ClubNumber != null)
                .OrderBy(r => r.Name);

            var guest = rides
                .Where(r => r.Eligibility == eligibility && r.EventEligibleRidersRank == null && r.ClubNumber == null)
                .OrderBy(r => r.Name);

            return competitionEligible.Concat(secondClaim).Concat(guest);
        }
    }
}
