using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ClubProcessor.Interfaces;
using ClubProcessor.Utilities;

namespace ClubProcessor.Services
{
    public class VetsHandicapLookup : IVetsHandicapProvider
    {
        private const int MinBucket = 1;
        private const int MaxBucket = 40;

        private static readonly Dictionary<int, VetsHandicapLookup> Cache = new();

        private readonly Dictionary<double, Dictionary<int, int>> _maleSeconds;
        private readonly Dictionary<double, Dictionary<int, int>> _femaleSeconds;

        private VetsHandicapLookup(int effectiveYear)
        {
            _maleSeconds = new();
            _femaleSeconds = new();

            var repoRoot = RepoLocator.FindGitRepoRoot();
            var dataFolder = Path.Combine(repoRoot, "data");
            var filePath = Path.Combine(dataFolder, $"vtta-standards-combined.{effectiveYear}.csv");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Standards file not found: {filePath}");

            Console.WriteLine($"[INFO] Using VTTA standards file: {filePath}");

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

            foreach (var (h, _) in maleCols)
            {
                var dist = double.Parse(h.Substring(1));
                _maleSeconds[dist] = new();
            }

            foreach (var (h, _) in femaleCols)
            {
                var dist = double.Parse(h.Substring(1));
                _femaleSeconds[dist] = new();
            }

            foreach (var line in lines.Skip(1))
            {
                var cells = line.Split(',');
                if (!int.TryParse(cells[0], out int age)) continue;

                int vetsBucket = age - 49;
                if (vetsBucket < MinBucket || vetsBucket > MaxBucket) continue;

                foreach (var (h, i) in maleCols)
                {
                    var dist = double.Parse(h.Substring(1));
                    _maleSeconds[dist][vetsBucket] = ParseTime(cells[i]);
                }

                foreach (var (h, i) in femaleCols)
                {
                    var dist = double.Parse(h.Substring(1));
                    _femaleSeconds[dist][vetsBucket] = ParseTime(cells[i]);
                }
            }
        }

        public static VetsHandicapLookup ForSeason(int seasonYear)
        {
            int effectiveYear = VttaStandardYearResolver.GetEffectiveStandardYear(seasonYear);
            if (!Cache.TryGetValue(effectiveYear, out var instance))
            {
                instance = new VetsHandicapLookup(effectiveYear);
                Cache[effectiveYear] = instance;
            }
            return instance;
        }

        public int GetHandicapSeconds(int year, double distanceMiles, bool isFemale, int vetsBucket)
        {
            if (vetsBucket < MinBucket || vetsBucket > MaxBucket)
            {
                throw new ArgumentOutOfRangeException(nameof(vetsBucket), vetsBucket,
                    $"vetsBucket must be between {MinBucket} and {MaxBucket} (inclusive).");
            }

            var lookup = isFemale ? _femaleSeconds : _maleSeconds;

            if (!lookup.TryGetValue(distanceMiles, out var bucketMap))
            {
                var supported = string.Join(", ", lookup.Keys.OrderBy(k => k));
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

        private static int ParseTime(string s)
        {
            return TimeSpan.TryParse(s, out var ts) ? (int)ts.TotalSeconds : 0;
        }
    }
}
