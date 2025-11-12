namespace ClubCore.Utilities
{
    public static class DbPathResolver
    {
        public static string GetCompetitorDbPath(string year) =>
            Path.Combine(FolderLocator.FindGitRepoRoot(), "data", $"club_competitors_{year}.db");

        public static string GetEventDbPath(string year) =>
            Path.Combine(FolderLocator.FindGitRepoRoot(), "data", $"club_events_{year}.db");

        //public static string GetFallbackDbPath() =>
        //    Path.Combine(RepoLocator.FindGitRepoRoot(), "data", "club_events_fallback.db");

        //public static string GetResultsDbPath() =>
        //    Path.Combine(RepoLocator.FindGitRepoRoot(), "data", "results.db");
    }
}
