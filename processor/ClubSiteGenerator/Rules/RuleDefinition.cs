using System.Text.Json.Serialization;

namespace ClubSiteGenerator.Rules
{
    public class RuleDefinition
    {
        [JsonPropertyName("count")]
        public int? Count { get; set; }

        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        [JsonPropertyName("cap")]
        public int? Cap { get; set; }

        [JsonPropertyName("nonTenMinimum")]
        public int? NonTenMinimum { get; set; }
    }
}
