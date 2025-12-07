namespace ClubSiteGenerator.Rules
{
    /// <summary>
    /// Contract for resolved scoring rules for a given competition year.
    /// Calculators and renderers should depend on this interface rather than
    /// the concrete CompetitionRules class, to allow mocking and alternative implementations.
    /// </summary>
    public interface ICompetitionRules
    {
        /// <summary>
        /// The name of the organization or company sponsoring the league. Used in the page titles.
        /// </summary>
        string? LeagueSponsor { get; }

        /// <summary>
        /// Number of ten-mile rides to count for the ten-mile competition.
        /// </summary>
        int TenMileCount { get; }

        /// <summary>
        /// Minimum number of non-ten events required in the mixed-distance scoring.
        /// </summary>
        int NonTenMinimum { get; }

        /// <summary>
        /// Total number of events to count for the mixed-distance (Scoring-N) competition.
        /// </summary>
        int MixedEventCount { get; }

        /// <summary>
        /// Title text for the ten-mile scoring column.
        /// </summary>
        string TenMileTitle { get; }

        /// <summary>
        /// Title text for use where context makes it clear it's about ten‑mile scoring (e.g. Nev Brooks).
        /// </summary>
        string TenMileShortTitle { get; }

        /// <summary>
        /// Title text for the mixed-distance scoring column.
        /// </summary>
        string FullCompetitionTitle { get; }

        /// <summary>
        /// Narrative rule text for the mixed-distance scoring.
        /// </summary>
        string RuleTextMixedCompetition { get; }

        /// <summary>
        /// Narrative rule text for the 10 TTs scoring.
        /// </summary>
        string RuleTextTensCompetition { get; }
    }
}