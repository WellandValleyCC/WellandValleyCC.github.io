using ClubCore.Utilities;
using System.Text.Json;

namespace ClubProcessor.Configuration
{
    public static partial class SeasonsConfig
    {
        public static IReadOnlyList<int> GetSeasons()
        {
            var configDir = FolderLocator.GetConfigDirectory();
            var path = Path.Combine(configDir, "seasons.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Seasons config not found at {path}");

            var json = File.ReadAllText(path);
            var manifest = JsonSerializer.Deserialize<SeasonManifest>(json);

            return manifest?.Seasons ?? new List<int>();
        }
    }
}
