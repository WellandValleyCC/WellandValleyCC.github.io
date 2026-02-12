namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Recursive directory copy helper with support for excluding files
    /// by extension. Safe if the source directory does not exist.
    /// </summary>
    public static class DirectoryCopyHelper
    {
        public static void CopyRecursive(
            string sourceDir,
            string destDir,
            string[] excludeExtensions)
        {
            if (!Directory.Exists(sourceDir))
                return; // safe: some folders may not exist

            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var ext = Path.GetExtension(file);

                if (excludeExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                    continue;

                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var name = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, name);

                CopyRecursive(subDir, destSubDir, excludeExtensions);
            }
        }
    }
}