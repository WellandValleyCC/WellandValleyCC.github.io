using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;

namespace ClubCore.Utilities
{
    public class DbPathResolver : IDbPathResolver
    {
        private readonly IFolderLocator folderLocator;

        public DbPathResolver(IFolderLocator folderLocator)
        {
            this.folderLocator = folderLocator;
        }

        public string GetCompetitorDbPath(string year)
        {
            var root = folderLocator.FindGitRepoRoot();
            return Path.Combine(root, PathTokens.DataFolder, $"club_competitors_{year}.db");
        }

        public string GetEventDbPath(string year)
        {
            var root = folderLocator.FindGitRepoRoot();
            return Path.Combine(root, PathTokens.DataFolder, $"club_events_{year}.db");
        }

        // -----------------------------
        // Static fallback API for DbContext
        // -----------------------------
        public static string ResolveCompetitorDbPath(string year)
        {
            var locator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            return new DbPathResolver(locator).GetCompetitorDbPath(year);
        }

        public static string ResolveEventDbPath(string year)
        {
            var locator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            return new DbPathResolver(locator).GetEventDbPath(year);
        }

    }
}