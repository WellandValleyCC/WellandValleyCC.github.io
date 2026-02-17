namespace ClubSiteGenerator.Rules
{
    public class RoundRobinRules
    {
        /// <summary>
        /// Number of rides to count for Open/Women competitions.
        /// May be fixed or derived from formula.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Minimum number of rides required to qualify.
        /// </summary>
        public int Minimum { get; set; } = 0;

        /// <summary>
        /// Team scoring rules (best N open + best M women).
        /// </summary>
        public RoundRobinTeamRules Team { get; set; } = new();
    }
}