namespace ClubSiteGenerator.Tests.Helpers
{
    public static class TestFileSystem
    {
        public static string CreateTempAssetsFolder()
        {
            var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(root, "Assets", "css"));
            return root;
        }

        public static void CreateCssFile(string root, string fileName)
        {
            var path = Path.Combine(root, "Assets", "css", fileName);
            File.WriteAllText(path, "/* test css */");
        }
    }
}
