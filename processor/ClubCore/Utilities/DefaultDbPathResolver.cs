using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultDbPathResolver : IDbPathResolver
    {
        private readonly DbPathResolver inner;

        public DefaultDbPathResolver(IFolderLocator folderLocator)
        {
            inner = new DbPathResolver(folderLocator);
        }

        public string GetCompetitorDbPath(string year) => inner.GetCompetitorDbPath(year);
        public string GetEventDbPath(string year) => inner.GetEventDbPath(year);
    }
}