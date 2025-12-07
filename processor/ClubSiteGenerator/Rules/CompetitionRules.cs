namespace ClubSiteGenerator.Rules
{
    /// <summary>
    /// Represents the resolved scoring rules for a given competition year.
    /// These values are derived from config (fixed or formula) and are used
    /// by calculators and renderers to ensure consistency.
    /// </summary>
    public class CompetitionRules : ICompetitionRules
    {
        /// <summary>
        /// Number of ten‑mile rides to count for the ten‑mile competition.
        /// </summary>
        public int TenMileCount { get; }

        /// <summary>
        /// Minimum number of non‑ten events required in the mixed‑distance scoring.
        /// </summary>
        public int NonTenMinimum { get; }

        /// <summary>
        /// Total number of events to count for the mixed‑distance (Scoring‑N) competition.
        /// </summary>
        public int MixedEventCount { get; }

        /// <summary>
        /// Title text for the ten‑mile scoring column (e.g. "10-mile TTs Best 8").
        /// </summary>
        public string TenMileTitle => $"10-mile TTs Best {TenMileCount}";

        /// <summary>
        /// Title text for use where context makes it clear it's about ten‑mile scoring (e.g. Nev Brooks).
        /// </summary>
        public string TenMileShortTitle => $"Best {TenMileCount}";

        /// <summary>
        /// Title text for the mixed‑distance scoring column (e.g. "Scoring 11").
        /// </summary>
        public string FullCompetitionTitle => $"Scoring {MixedEventCount}";

        /// <summary>
        /// Narrative rule text for the mixed‑distance scoring.
        /// </summary>
        public string RuleText =>
            $"Your championship score is the total of the points " +
            $"from your {NonTenMinimum} highest scoring non‑ten events, " +
            $"plus your best {MixedEventCount - NonTenMinimum} other events of any distance.";

        public CompetitionRules(int tenMileCount, int nonTenMinimum, int mixedEventCount)
        {
            TenMileCount = tenMileCount;
            NonTenMinimum = nonTenMinimum;
            MixedEventCount = mixedEventCount;
        }
    }
}