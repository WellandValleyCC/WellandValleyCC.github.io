namespace ClubCore.Utilities
{
    public static class DbPathResolver
    {
        public static string GetCompetitorDbPath(string year) =>
            Path.Combine(FolderLocator.FindGitRepoRoot(), PathTokens.DataFolder, $"club_competitors_{year}.db");

        public static string GetEventDbPath(string year) =>
            Path.Combine(FolderLocator.FindGitRepoRoot(), PathTokens.DataFolder, $"club_events_{year}.db");

        //public static string GetFallbackDbPath() =>
        //    Path.Combine(RepoLocator.FindGitRepoRoot(), FolderNames.Data, "club_events_fallback.db");

        //public static string GetResultsDbPath() =>
        //    Path.Combine(RepoLocator.FindGitRepoRoot(), FolderNames.Data, "results.db");
    }
}
