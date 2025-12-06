using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.Interfaces
{
    public interface ICompetitionRulesProvider
    {
        ICompetitionRule GetRule(int year, CompetitionRuleScope competitionType);
    }
}
