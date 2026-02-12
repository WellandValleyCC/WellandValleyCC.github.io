using ClubCore.Utilities;

namespace ClubSiteGenerator.Tests.Helpers
{
    public static class TestFileSystem
    {
        /// <summary>
        /// Creates a temporary folder structure that mirrors the real RoundRobinSiteAssets layout:
        ///
        ///   <tempRoot>\assets\
        ///   <tempRoot>\logos\
        ///
        /// Returns the tempRoot path.
        /// </summary>
        public static string CreateTempAssetsFolder()
        {
            var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(root);

            // Create the canonical asset subfolders
            Directory.CreateDirectory(Path.Combine(root, PathTokens.AssetsFolder));
            Directory.CreateDirectory(Path.Combine(root, PathTokens.LogosFolder));

            return root;
        }

        /// <summary>
        /// Creates a CSS file inside the assets folder.
        /// </summary>
        public static void CreateCssFile(string assetsRoot, string fileName)
        {
            var path = Path.Combine(assetsRoot, fileName);
            File.WriteAllText(path, "/* test css */");
        }
    }
}
