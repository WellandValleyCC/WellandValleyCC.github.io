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
    }
}
