using System.Text.Json.Serialization;

namespace ClubProcessor.Configuration
{
    public static partial class SeasonsConfig
    {
        private class SeasonManifest
        {
            [JsonPropertyName("seasons")]
            public List<int> Seasons { get; set; } = new();
        }
    }
}
