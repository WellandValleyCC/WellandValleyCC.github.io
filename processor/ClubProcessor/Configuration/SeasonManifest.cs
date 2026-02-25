using System.Text.Json.Serialization;

namespace ClubProcessor.Configuration
{
    public static partial class SeasonsConfig
    {
        private class SeasonManifest
        {
            [JsonPropertyName("club-seasons")]
            public List<int> ClubSeasons { get; set; } = new(); 
            
            [JsonPropertyName("round-robin-seasons")]
            public List<int> RoundRobinSeasons { get; set; } = new();
        }
    }
}
