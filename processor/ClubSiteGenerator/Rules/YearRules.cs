using System.Text.Json.Serialization;

namespace ClubSiteGenerator.Rules
{
    public class YearRules
    {
        [JsonPropertyName("leagueSponsor")]
        public string? LeagueSponsor { get; set; }

        [JsonPropertyName("tenMile")]
        public RuleDefinition TenMile { get; set; } = new();

        [JsonPropertyName("mixedDistance")]
        public RuleDefinition MixedDistance { get; set; } = new();

        //
        // NEW: Optional Round Robin rules block
        // (Omitted entirely for 2024, present for 2025+)
        //
        [JsonPropertyName("roundRobin")]
        public RoundRobinRuleDefinition? RoundRobin { get; set; }
    }
}