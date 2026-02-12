using ClubCore.Utilities;

namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Handles all asset copying for the Round Robin site.
    /// Copies year-specific CSS, logos, and any other asset folders.
    /// Returns an AssetPipelineResult containing the selected CSS filename.
    /// </summary>
    public static class AssetPipeline
    {
        public static AssetPipelineResult CopyRoundRobinAssets(string repoRoot, int year)
        {
            Log.Info($"Starting Round Robin asset pipeline for year {year}");
            Log.Info($"Repo root: {repoRoot}");

            // Resolve canonical folders
            var rrAssetsRoot = Path.Combine(repoRoot, PathTokens.RoundRobinAssetsFolder);
            var rrOutputRoot = Path.Combine(repoRoot, PathTokens.RoundRobinOutputFolder);

            Log.Info($"Assets root: {rrAssetsRoot}");
            Log.Info($"Output root: {rrOutputRoot}");

            // 1. Copy year-specific stylesheet
            var cssSource = Path.Combine(rrAssetsRoot, PathTokens.AssetsFolder);
            Log.Info($"Selecting CSS from: {cssSource}");

            var cssFile = AssetCopier.CopyYearSpecificStylesheet(
                cssSource,
                rrOutputRoot,
                year,
                prefix: PathTokens.RoundRobinCssPrefix
            );

            Log.Info($"Copied CSS file: {cssFile}");

            // Exclusions (e.g., markdown files)
            var exclude = new[] { PathTokens.MarkdownExtension };

            // 2. Copy logos (if present)
            var logosSource = Path.Combine(rrAssetsRoot, PathTokens.LogosFolder);
            var logosDest = Path.Combine(rrOutputRoot, PathTokens.LogosFolder);

            Log.Info($"Copying logos from {logosSource} to {logosDest}");
            DirectoryCopyHelper.CopyRecursive(logosSource, logosDest, exclude);

            // 3. Copy any other asset folders that exist
            foreach (var folder in Directory.GetDirectories(rrAssetsRoot))
            {
                var name = Path.GetFileName(folder);

                // Skip folders we already handled explicitly
                if (name.Equals(PathTokens.AssetsFolder, StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Equals(PathTokens.LogosFolder, StringComparison.OrdinalIgnoreCase)) continue;

                var dest = Path.Combine(rrOutputRoot, name);

                Log.Info($"Copying additional asset folder: {name}");
                DirectoryCopyHelper.CopyRecursive(folder, dest, exclude);
            }

            Log.Info("Asset pipeline complete.");

            return new AssetPipelineResult
            {
                CssFile = cssFile
            };
        }
    }
}