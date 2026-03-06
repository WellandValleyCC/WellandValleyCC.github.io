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
        /// Club scoring rules (best N open + best M women).
        /// </summary>
        public RoundRobinClubRules Club { get; set; } = new();

        /// <summary>
        /// Whether the "Guest" pseudo‑club should be included in the Club competition
        /// scoring. This does <em>not</em> affect individual Round Robin scoring.
        /// Defaults to false. Only meaningful in seasons where Guest riders receive
        /// RoundRobinPoints and RoundRobinWomenPosition (e.g. 2025).
        /// </summary>
        public bool IncludeGuestClub { get; set; } = false;
    }
}