namespace ClubSiteGenerator.Interfaces
{
    public interface ICompetitionRule
    {
        /// <summary>
        /// Defines the number of events that count towards scoring
        /// </summary>
        int GetLimit(int eventsInCalendar);

        /// <summary>
        /// Defines the minimum number of non‑ten‑mile events required for scoring.
        /// </summary>
        /// <remarks>
        /// Ignored if this rule is used for a ten‑mile competition.
        /// </remarks>
        int RequiredNonTens { get; }
    }
}
