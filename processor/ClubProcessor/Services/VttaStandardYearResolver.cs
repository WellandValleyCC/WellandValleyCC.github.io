using ClubCore.Utilities;
using System.Text.RegularExpressions;


namespace ClubProcessor.Services
{
    public static class VttaStandardYearResolver
    {
        private static readonly SortedSet<int> AvailableYears;

        static VttaStandardYearResolver()
        {
            var repoRoot = FolderLocator.FindGitRepoRoot();
            var dataFolder = Path.Combine(repoRoot, PathTokens.DataFolder);
            var files = Directory.GetFiles(dataFolder, "vtta-standards-combined.*.csv");
            AvailableYears = new SortedSet<int>(
                files.Select(f => Path.GetFileNameWithoutExtension(f))
                     .Select(name => Regex.Match(name, @"\.(\d{4})$"))
                     .Where(m => m.Success)
                     .Select(m => int.Parse(m.Groups[1].Value))
            );
        }

        public static int GetEffectiveStandardYear(int seasonYear)
        {
            if (!AvailableYears.Any())
                throw new InvalidOperationException("No VTTA standards CSVs found.");

            return GetEffectiveStandardYear(seasonYear, AvailableYears);
        }

        public static int GetEffectiveStandardYear(int seasonYear, IEnumerable<int> availableYears)
        {
            var candidates = availableYears.Where(y => y <= seasonYear);
            if (!candidates.Any())
                throw new InvalidOperationException($"No standards available for season {seasonYear}");

            return candidates.Max();
        }
    }
}
