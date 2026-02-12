using System.IO;
using ClubCore.Utilities;

namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Recursive directory copy helper with support for excluding files
    /// by extension. Logs each copied file.
    /// </summary>
    public static class DirectoryCopyHelper
    {
        public static void CopyRecursive(
            string sourceDir,
            string destDir,
            string[] excludeExtensions)
        {
            if (!Directory.Exists(sourceDir))
            {
                Log.Info($"Source directory does not exist, skipping: {sourceDir}");
                return;
            }

            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var ext = Path.GetExtension(file);

                if (excludeExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    Log.Info($"Skipping excluded file: {Path.GetFileName(file)}");
                    continue;
                }

                var destFile = Path.Combine(destDir, Path.GetFileName(file));

                File.Copy(file, destFile, overwrite: true);

                // Log copied file — highlight PNGs
                if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    Log.Info($"Copied PNG asset: {Path.GetFileName(file)}");
                }
                else
                {
                    Log.Info($"Copied asset: {Path.GetFileName(file)}");
                }
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                var name = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, name);

                Log.Info($"Entering subfolder: {subDir}");
                CopyRecursive(subDir, destSubDir, excludeExtensions);
            }
        }
    }
}