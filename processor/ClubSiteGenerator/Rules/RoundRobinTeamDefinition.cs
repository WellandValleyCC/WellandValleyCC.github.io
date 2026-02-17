using System.Text.Json.Serialization;

namespace ClubSiteGenerator.Rules
{
    public class RoundRobinTeamDefinition
    {
        [JsonPropertyName("openCount")]
        public int OpenCount { get; set; }

        [JsonPropertyName("womenCount")]
        public int WomenCount { get; set; }
    }
}