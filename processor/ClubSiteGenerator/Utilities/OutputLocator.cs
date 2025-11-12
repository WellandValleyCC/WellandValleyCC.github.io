using ClubCore.Utilities;

namespace ClubSiteGenerator.Services
{
    public static class OutputLocator
    {
        public static string GetOutputDirectory()
        {
            bool runningInCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
            if (runningInCi)
                return Path.Combine(Path.GetTempPath(), "wvcc-site");

            var repoRoot = RepoLocator.FindGitRepoRoot();
            var dir = Path.Combine(repoRoot, "SiteOutput");
            Directory.CreateDirectory(dir);

            return dir;
        }
    }
}
