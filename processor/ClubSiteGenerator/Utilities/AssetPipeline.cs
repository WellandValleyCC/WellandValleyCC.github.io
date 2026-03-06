using ClubCore.Interfaces;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Copies all static assets required for a generated site.
    /// This includes:
    /// - Year-specific CSS
    /// - Favicons
    /// - Logos
    /// - Any additional asset folders
    ///
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

        /// <summary>
        /// Executes the asset pipeline for a site.
        /// Copies CSS, favicons, logos, and any additional asset folders.
        /// </summary>
        public AssetPipelineResult CopyAssets(
            string assetsRoot,
            string outputRoot,
            int year,
            string cssPrefix,
            string siteName)
        {
            log.Info($"Starting asset pipeline for {siteName}, year {year}");
            log.Info($"Assets root: {assetsRoot}");
            log.Info($"Output root: {outputRoot}");

            // 1. Copy year-specific stylesheet
            var cssSource = Path.Combine(assetsRoot, PathTokens.AssetsFolder);
            log.Info($"Selecting CSS from: {cssSource}");

            var cssFile = assetCopier.CopyYearSpecificStylesheet(
                cssSource,
                outputRoot,
                year,
                prefix: cssPrefix
            );

            log.Info($"Copied CSS file: {cssFile}");

            // 1b. Copy favicon files
            var faviconSource = Path.Combine(assetsRoot, PathTokens.AssetsFolder);
            var faviconDest = Path.Combine(outputRoot, PathTokens.AssetsFolder);

            Directory.CreateDirectory(faviconDest);

            var faviconFiles = new[]
            {
                "favicon.svg",
                "favicon-32.png",
                "favicon-16.png",
                "apple-touch-icon.png"
            };

            foreach (var f in faviconFiles)
            {
                var src = Path.Combine(faviconSource, f);
                var dst = Path.Combine(faviconDest, f);

                if (File.Exists(src))
                {
                    log.Info($"Copying favicon: {f}");
                    File.Copy(src, dst, overwrite: true);
                }
                else
                {
                    log.Warn($"Favicon missing: {src}");
                }
            }

            // Exclusions (e.g., markdown files)
            var exclude = new[] { PathTokens.MarkdownExtension };

            // 2. Copy logos (if present)
            var logosSource = Path.Combine(assetsRoot, PathTokens.LogosFolder);
            var logosDest = Path.Combine(outputRoot, PathTokens.LogosFolder);

            log.Info($"Copying logos from {logosSource} to {logosDest}");
            copyHelper.CopyRecursive(logosSource, logosDest, exclude);

            // 3. Copy any other asset folders
            foreach (var folder in directoryProvider.GetDirectories(assetsRoot))
            {
                var name = Path.GetFileName(folder);

                // Skip folders handled explicitly
                if (name.Equals(PathTokens.AssetsFolder, StringComparison.OrdinalIgnoreCase)) continue;
                if (name.Equals(PathTokens.LogosFolder, StringComparison.OrdinalIgnoreCase)) continue;

                var dest = Path.Combine(outputRoot, name);

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