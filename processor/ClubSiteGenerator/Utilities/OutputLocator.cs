using ClubCore.Utilities;

namespace ClubSiteGenerator.Services
{
    public static class OutputLocator
    {
        public static string GetOutputDirectory()
        {
            bool runningInCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
            string root;

            if (runningInCi)
            {
                root = Path.Combine(Path.GetTempPath(), "wvcc-site");
                Console.WriteLine($"[OutputLocator] Running in CI. Root output directory: {root}");
            }
            else
            {
                var folderLocator = new DefaultFolderLocator(
                    new DefaultDirectoryProvider(),
                    new DefaultLog());

                root = folderLocator.FindGitRepoRoot();
                Console.WriteLine($"[OutputLocator] Running locally. Root output directory: {root}");
            }

            return root;
        }
    }
}
