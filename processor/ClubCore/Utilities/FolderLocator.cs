namespace ClubCore.Utilities
{
    public static class FolderLocator
    {
        public static string FindGitRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, ".git")))
            {
                dir = dir.Parent;
            }

            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate Git repo root (no .git folder found)");

            return dir.FullName;
        }

        public static string GetDataDirectory()
        {
            var repoRoot = FindGitRepoRoot();
            var dataDir = Path.Combine(repoRoot, "data");

            if (!Directory.Exists(dataDir))
                throw new DirectoryNotFoundException($"Data directory not found at {dataDir}");

            return dataDir;
        }

        public static string GetConfigDirectory()
        {
            var repoRoot = FindGitRepoRoot();
            var dataDir = Path.Combine(repoRoot, "config");

            if (!Directory.Exists(dataDir))
                throw new DirectoryNotFoundException($"Data directory not found at {dataDir}");

            return dataDir;
        }
    }
}
