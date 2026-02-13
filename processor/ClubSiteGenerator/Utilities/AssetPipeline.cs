using ClubCore.Interfaces;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Handles all asset copying for the Round Robin site.
    /// Copies year-specific CSS, logos, and any other asset folders.
    /// Returns an AssetPipelineResult containing the selected CSS filename.
    /// </summary>
    public class AssetPipeline
    {
        private readonly IAssetCopier assetCopier;
        private readonly IDirectoryCopyHelper copyHelper;
        private readonly IDirectoryProvider directoryProvider;
        private readonly ILog log;

        public AssetPipeline(
            IAssetCopier assetCopier,
            IDirectoryCopyHelper copyHelper,
            IDirectoryProvider directoryProvider,
            ILog log)
        {
            this.assetCopier = assetCopier;
            this.copyHelper = copyHelper;
            this.directoryProvider = directoryProvider;
            this.log = log;
        }

        public AssetPipelineResult CopyRoundRobinAssets(string repoRoot, int year)
        {
            log.Info($"Starting Round Robin asset pipeline for year {year}");
            log.Info($"Repo root: {repoRoot}");

            // Resolve canonical folders
            var rrAssetsRoot = Path.Combine(repoRoot, PathTokens.RoundRobinAssetsFolder);
            var rrOutputRoot = Path.Combine(repoRoot, PathTokens.RoundRobinOutputFolder);

            log.Info($"Assets root: {rrAssetsRoot}");
            log.Info($"Output root: {rrOutputRoot}");

            // 1. Copy year-specific stylesheet
            var cssSource = Path.Combine(rrAssetsRoot, PathTokens.AssetsFolder);
            log.Info($"Selecting CSS from: {cssSource}");

            var cssFile = assetCopier.CopyYearSpecificStylesheet(
                cssSource,
                rrOutputRoot,
                year,
                prefix: PathTokens.RoundRobinCssPrefix
            );

            log.Info($"Copied CSS file: {cssFile}");

            // Exclusions (e.g., markdown files)
            var exclude = new[] { PathTokens.MarkdownExtension };

            // 2. Copy logos (if present)
            var logosSource = Path.Combine(rrAssetsRoot, PathTokens.LogosFolder);
            var logosDest = Path.Combine(rrOutputRoot, PathTokens.LogosFolder);

            log.Info($"Copying logos from {logosSource} to {logosDest}");
            copyHelper.CopyRecursive(logosSource, logosDest, exclude);

            // 3. Copy any other asset folders that exist
            foreach (var folder in directoryProvider.GetDirectories(rrAssetsRoot))
            {
                var name = Path.GetFileName(folder);

                // Skip folders we already handled explicitly
                if (name.Equals(PathTokens.AssetsFolder, StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Equals(PathTokens.LogosFolder, StringComparison.OrdinalIgnoreCase)) continue;

                var dest = Path.Combine(rrOutputRoot, name);

                log.Info($"Copying additional asset folder: {name}");
                copyHelper.CopyRecursive(folder, dest, exclude);
            }

            log.Info("Asset pipeline complete.");

            return new AssetPipelineResult
            {
                CssFile = cssFile
            };
        }
    }
}