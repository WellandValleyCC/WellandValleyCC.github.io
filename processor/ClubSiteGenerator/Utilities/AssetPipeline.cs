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
            // Resolve canonical folders
            var rrAssetsRoot = Path.Combine(repoRoot, PathTokens.RoundRobinAssetsFolder);
            var rrOutputRoot = Path.Combine(repoRoot, PathTokens.RoundRobinOutputFolder);

            // 1. Copy year-specific stylesheet
            var cssFile = AssetCopier.CopyYearSpecificStylesheet(
                Path.Combine(rrAssetsRoot, PathTokens.AssetsFolder),
                rrOutputRoot,
                year,
                prefix: PathTokens.RoundRobinCssPrefix
            );

            // Exclusions (e.g., markdown files)
            var exclude = new[] { PathTokens.MarkdownExtension };

            // 2. Copy logos (if present)
            DirectoryCopyHelper.CopyRecursive(
                Path.Combine(rrAssetsRoot, PathTokens.LogosFolder),
                Path.Combine(rrOutputRoot, PathTokens.LogosFolder),
                exclude
            );

            // 3. Copy any other asset folders that exist
            foreach (var folder in Directory.GetDirectories(rrAssetsRoot))
            {
                var name = Path.GetFileName(folder);

                // Skip folders we already handled explicitly
                if (name.Equals(PathTokens.AssetsFolder, StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Equals(PathTokens.LogosFolder, StringComparison.OrdinalIgnoreCase)) continue;

                DirectoryCopyHelper.CopyRecursive(
                    folder,
                    Path.Combine(rrOutputRoot, name),
                    exclude
                );
            }

            return new AssetPipelineResult
            {
                CssFile = cssFile
            };
        }
    }
}