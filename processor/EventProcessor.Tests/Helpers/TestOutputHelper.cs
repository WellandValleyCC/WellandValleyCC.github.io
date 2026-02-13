using ClubCore.Utilities;
using System;
using System.IO;

namespace ClubProcessor.Tests.Helpers
{
    public static class TestOutputHelper
    {
        /// <summary>
        /// Returns a writable directory for test artefacts.
        /// - In CI (detected via GITHUB_ACTIONS env var), uses Path.GetTempPath()
        /// - Locally, uses repo-rooted "TestOutput" folder
        /// </summary>
        public static string GetOutputDirectory()
        {
            bool runningInCi = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));

            string root;
            if (runningInCi)
            {
                root = Path.Combine(Path.GetTempPath(), "wvcc-tests");
            }
            else
            {
                var folderLocator = new DefaultFolderLocator(
                    new DefaultDirectoryProvider(),
                    new DefaultLog());

                var repoRoot = folderLocator.FindGitRepoRoot();
                root = Path.Combine(repoRoot, "TestOutput");
            }

            Directory.CreateDirectory(root);
            return root;
        }

        public static string GetUniqueFilePath(string prefix, string extension = ".csv")
        {
            var dir = GetOutputDirectory();
            var fileName = $"{prefix}_{Guid.NewGuid()}{extension}";
            return Path.Combine(dir, fileName);
        }
    }
}
