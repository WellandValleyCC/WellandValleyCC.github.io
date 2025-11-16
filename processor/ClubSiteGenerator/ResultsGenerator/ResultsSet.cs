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
    }
}
