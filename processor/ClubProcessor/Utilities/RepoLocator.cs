namespace ClubProcessor.Utilities
{
    public static class RepoLocator
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
    }
}
