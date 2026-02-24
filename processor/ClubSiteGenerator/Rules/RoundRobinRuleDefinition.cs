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

        [JsonPropertyName("club")]
        public RoundRobinClubDefinition? Club { get; set; }

        /// <summary>
        /// Whether the "Guest" pseudo‑club should be included in the Club
        /// competition scoring. Only meaningful in seasons where Guest riders
        /// receive individual Round Robin points (e.g. 2025).
        /// </summary>
        [JsonPropertyName("includeGuestsClub")]
        public bool? IncludeGuestsClub { get; set; }
    }
}