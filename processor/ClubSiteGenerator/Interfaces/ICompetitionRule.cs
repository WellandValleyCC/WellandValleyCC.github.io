namespace ClubSiteGenerator.Interfaces
{
    public interface ICompetitionRule
    {
        /// <summary>
        /// Defines the number of events that count towards scoring
        /// </summary>
        int GetLimit(int eventsInCalendar);
    }
}
