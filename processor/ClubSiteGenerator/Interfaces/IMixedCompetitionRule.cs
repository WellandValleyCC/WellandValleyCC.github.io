namespace ClubSiteGenerator.Interfaces
{
    public interface IMixedCompetitionRule : ICompetitionRule
    {
        /// <summary>
        /// Defines the minimum number of non‑ten‑mile events required for scoring.
        /// </summary>
        int RequiredNonTens { get; }
    }
}
