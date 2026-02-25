using ClubCore.Utilities;
using System.Text.Json;

namespace ClubProcessor.Configuration
{
    public static partial class SeasonsConfig
    {
        public static IReadOnlyList<int> GetClubSeasons() =>
            LoadManifest().ClubSeasons;

        public static IReadOnlyList<int> GetRoundRobinSeasons() =>
            LoadManifest().RoundRobinSeasons;

        private static SeasonManifest LoadManifest()
        {
            var folderLocator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            var configDir = folderLocator.GetConfigDirectory();
            var path = Path.Combine(configDir, "seasons.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Seasons config not found at {path}");

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<SeasonManifest>(json)
                   ?? throw new InvalidOperationException("Invalid seasons.json");
        }
    }
}
