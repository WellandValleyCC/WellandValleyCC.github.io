using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Recursive directory copy helper with support for excluding files
    /// by extension. Logs each copied file.
    /// </summary>
    public class DirectoryCopyHelper : IDirectoryCopyHelper
    {
        private readonly IDirectoryProvider directoryProvider;
        private readonly IFileProvider fileProvider;
        private readonly ILog log;

        public DirectoryCopyHelper(
            IDirectoryProvider directoryProvider,
            IFileProvider fileProvider,
            ILog log)
        {
            this.directoryProvider = directoryProvider;
            this.fileProvider = fileProvider;
            this.log = log;
        }

        public void CopyRecursive(
            string sourceDir,
            string destDir,
            string[] excludeExtensions)
        {
            if (!directoryProvider.Exists(sourceDir))
            {
                log.Info($"Source directory does not exist, skipping: {sourceDir}");
                return;
            }

            directoryProvider.CreateDirectory(destDir);

            foreach (var file in fileProvider.GetFiles(sourceDir))
            {
                var ext = Path.GetExtension(file);

                if (excludeExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
                {
                    log.Info($"Skipping excluded file: {Path.GetFileName(file)}");
                    continue;
                }

                var destFile = Path.Combine(destDir, Path.GetFileName(file));

                fileProvider.Copy(file, destFile, overwrite: true);

                if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    log.Info($"Copied PNG asset: {Path.GetFileName(file)}");
                }
                else
                {
                    log.Info($"Copied asset: {Path.GetFileName(file)}");
                }
            }

            foreach (var subDir in directoryProvider.GetDirectories(sourceDir))
            {
                var name = Path.GetFileName(subDir);
                var destSubDir = Path.Combine(destDir, name);

                log.Info($"Entering subfolder: {subDir}");
                CopyRecursive(subDir, destSubDir, excludeExtensions);
            }
        }
    }
}