using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet
    {
        public readonly IEnumerable<Ride> Rides;

        protected ResultsSet(IEnumerable<Ride> rides)
        {
            Rides = rides;
        }

        public abstract string DisplayName { get; }
        public abstract string FileName { get; }
        public abstract string SubFolderName { get; }

        public abstract HtmlTable CreateTable();
    }
}
