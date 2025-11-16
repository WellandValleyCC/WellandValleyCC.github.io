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
            var firstClaimDxxRides = rides
                .Where(r => r.Eligibility == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.FirstClaim)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var secondClaimDxxRides = rides
                .Where(r => r.Eligibility == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.SecondClaim)
                .OrderBy(r => r.Name);

            var guestDxxRides = rides
                .Where(r => r.Eligibility == eligibility && r.ClubNumber == null)
                .OrderBy(r => r.Name);

            return firstClaimDxxRides.Concat(secondClaimDxxRides).Concat(guestDxxRides);
        }
    }
}
