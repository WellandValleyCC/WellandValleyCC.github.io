using System.Text.Json.Serialization;

namespace ClubSiteGenerator.Rules
{
    //
    // NEW: Round Robin rule definition for JSON deserialisation
    //
    public class RoundRobinRuleDefinition
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        [JsonPropertyName("cap")]
        public int? Cap { get; set; }

        [JsonPropertyName("minimum")]
        public int? Minimum { get; set; }

        [JsonPropertyName("team")]
        public RoundRobinTeamDefinition? Team { get; set; }
    }
}