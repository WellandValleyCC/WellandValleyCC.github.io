using ClubCore.Models;

namespace ClubSiteGenerator.Rules
{
    public interface ICompetitionRulesProvider
    {
        ICompetitionRules GetRules(int competitionYear, IEnumerable<CalendarEvent> calendar);
    }
}

