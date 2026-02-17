namespace ClubSiteGenerator.Rules
{
    /// <summary>
    /// Represents the resolved scoring rules for a given competition year.
    /// These values are derived from config (fixed or formula) and are used
    /// by calculators and renderers to ensure consistency.
    /// </summary>
    public class CompetitionRules : ICompetitionRules
    {
        // -----------------------------
        // Existing WVCC properties
        // -----------------------------

        public string? LeagueSponsor { get; }

        public int TenMileCount { get; }
        public int NonTenMinimum { get; }
        public int MixedEventCount { get; }

        public string TenMileTitle => $"10-mile TTs Best {TenMileCount}";
        public string TenMileShortTitle => $"Best {TenMileCount}";
        public string FullCompetitionTitle => $"Scoring {MixedEventCount}";

        public string RuleTextMixedCompetition =>
            $"Your competition score is the total of the points " +
            $"from your {NonTenMinimum} highest scoring non‑ten events, " +
            $"plus your best {MixedEventCount - NonTenMinimum} other events of any distance.";

        public string RuleTextTensCompetition =>
            $"Your overall score is the total of the points from your best {TenMileCount} events.";

        // -----------------------------
        // NEW: Round Robin rules block
        // -----------------------------

        public RoundRobinRules RoundRobin { get; }

        // -----------------------------
        // Constructor
        // -----------------------------
        public CompetitionRules(
            int tenMileCount,
            int nonTenMinimum,
            int mixedEventCount,
            string? leagueSponsor,
            RoundRobinRules? roundRobinRules)
        {
            TenMileCount = tenMileCount;
            NonTenMinimum = nonTenMinimum;
            MixedEventCount = mixedEventCount;
            LeagueSponsor = leagueSponsor;

            // If roundRobinRules is null (e.g. 2024), provide safe defaults
            RoundRobin = roundRobinRules ?? new RoundRobinRules
            {
                Count = mixedEventCount,   // fallback: same as mixed-distance
                Minimum = 0,
                Team = new RoundRobinTeamRules
                {
                    OpenCount = 4,
                    WomenCount = 1
                }
            };
        }
    }

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

    public class RoundRobinTeamRules
    {
        public int OpenCount { get; set; }
        public int WomenCount { get; set; }
    }
}