namespace ClubSiteGenerator.Utilities
{
    public static class AssetCopier
    {
        /// <summary>
        /// Copies a year-specific CSS file into the output folder, falling back to the
        /// most recent earlier year if the exact year is not available.
        /// Returns the filename that was copied (e.g. "roundrobin2025.css").
        /// </summary>
        public static string CopyYearSpecificStylesheet(
            string outputDir,
            int year,
            string prefix)
        {
            var assetsRoot = Path.Combine("Assets", "css");

            // Find all matching files: prefixYYYY.css
            var available = Directory.GetFiles(assetsRoot, $"{prefix}*.css")
                                     .Select(Path.GetFileName)
                                     .Where(f => f != null)
                                     .ToList();

            if (!available.Any())
                throw new FileNotFoundException(
                    $"No CSS files found for prefix '{prefix}' in {assetsRoot}");

            // Try exact match first
            var exact = $"{prefix}{year}.css";
            if (available.Contains(exact))
            {
                Copy(assetsRoot, exact, outputDir);
                return exact;
            }

            // Extract available years
            var availableYears = available
                .Select(f =>
                {
                    var digits = new string(f.Where(char.IsDigit).ToArray());
                    return int.TryParse(digits, out var y) ? y : (int?)null;
                })
                .Where(y => y.HasValue)
                .Select(y => y.Value)
                .OrderByDescending(y => y)
                .ToList();

            // Find the latest year < target year
            var fallbackYear = availableYears
                .Where(y => y < year)
                .FirstOrDefault();

            if (fallbackYear == 0)
                throw new FileNotFoundException(
                    $"No suitable CSS found for prefix '{prefix}' for {year} or any earlier year.");

            var fallbackFile = $"{prefix}{fallbackYear}.css";
            Copy(assetsRoot, fallbackFile, outputDir);
            return fallbackFile;
        }

        private static void Copy(string root, string fileName, string outputDir)
        {
            var source = Path.Combine(root, fileName);

            var assetsDir = Path.Combine(outputDir, "assets");
            Directory.CreateDirectory(assetsDir);

            var dest = Path.Combine(assetsDir, fileName);
            File.Copy(source, dest, overwrite: true);
        }
    }
}