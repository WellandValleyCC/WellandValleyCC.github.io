using System;
using System.Collections.Generic;
using System.Linq;
using ClubProcessor.Interfaces;

namespace ClubProcessor.Services
{
    public class VetsHandicapLookup : IVetsHandicapProvider
    {
        // Gender independent handicap tables mapping distanceMiles --> { vetsBucket --> seconds }
        private static readonly Dictionary<double, Dictionary<int, int>> MaleSeconds;
        private static readonly Dictionary<double, Dictionary<int, int>> FemaleSeconds;

        private const int MinBucket = 1;
        private const int MaxBucket = 40;
        private const int BucketDomainSize = MaxBucket - MinBucket + 1; // 40

        static VetsHandicapLookup()
        {
            MaleSeconds = new();
            FemaleSeconds = new();

            var repoRoot = FindGitRepoRoot();
            var dataFolder = Path.Combine(repoRoot, "data");
            var filePath = Path.Combine(dataFolder, $"vtta-standards-combined.2025.csv");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Standards file not found: {filePath}");
            }
            var lines = File.ReadAllLines(filePath);
            var headers = lines[0].Split(',');

            var maleCols = headers
                .Select((h, i) => (h, i))
                .Where(t => t.h.StartsWith("m"))
                .ToList();

            var femaleCols = headers
                .Select((h, i) => (h, i))
                .Where(t => t.h.StartsWith("f"))
                .ToList();

            foreach (var (h, i) in maleCols)
            {
                var dist = double.Parse(h.Substring(1));
                MaleSeconds[dist] = new Dictionary<int, int>();
            }

            foreach (var (h, i) in femaleCols)
            {
                var dist = double.Parse(h.Substring(1));
                FemaleSeconds[dist] = new Dictionary<int, int>();
            }

            foreach (var line in lines.Skip(1))
            {
                var cells = line.Split(',');
                if (!int.TryParse(cells[0], out int age)) continue;

                int vetsBucket = age - 49;
                if (vetsBucket < 1 || vetsBucket > 40) continue;

                foreach (var (h, i) in maleCols)
                {
                    var dist = double.Parse(h.Substring(1));
                    MaleSeconds[dist][vetsBucket] = ParseTime(cells[i]);
                }

                foreach (var (h, i) in femaleCols)
                {
                    var dist = double.Parse(h.Substring(1));
                    FemaleSeconds[dist][vetsBucket] = ParseTime(cells[i]);
                }
            }
        }

        public static string FindGitRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, ".git")))
            {
                dir = dir.Parent;
            }

            if (dir == null)
                throw new DirectoryNotFoundException("Could not locate Git repo root (no .git folder found)");

            return dir.FullName;
        }

        private static int ParseTime(string s)
        {
            return TimeSpan.TryParse(s, out var ts) ? (int)ts.TotalSeconds : 0;
        }

        public int GetHandicapSeconds(double distanceMiles, bool isFemale, int vetsBucket)
        {
            if (vetsBucket < MinBucket || vetsBucket > MaxBucket)
            {
                throw new ArgumentOutOfRangeException(nameof(vetsBucket), vetsBucket,
                    $"vetsBucket must be between {MinBucket} and {MaxBucket} (inclusive).");
            }

            var source = isFemale ? FemaleSeconds : MaleSeconds;

            if (!source.TryGetValue(distanceMiles, out var bucketMap))
            {
                var supported = string.Join(", ", source.Keys.OrderBy(k => k));
                throw new ArgumentOutOfRangeException(nameof(distanceMiles), distanceMiles,
                    $"Unsupported distance {distanceMiles}. Supported distances: {supported}.");
            }

            if (!bucketMap.TryGetValue(vetsBucket, out var seconds))
            {
                throw new ArgumentOutOfRangeException(nameof(vetsBucket), vetsBucket,
                    $"No standard found for bucket {vetsBucket} at distance {distanceMiles}.");
            }

            return seconds;
        }
    }
}
