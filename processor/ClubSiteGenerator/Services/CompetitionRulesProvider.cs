using ClubCore.Models.Enums;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Models.Enums;
using System.Text.Json;

namespace ClubSiteGenerator.Services
{
    internal class CompetitionRulesProvider : ICompetitionRulesProvider
    {
        private readonly JsonElement rulesJson;

        public CompetitionRulesProvider(JsonElement rulesJson)
        {
            this.rulesJson = rulesJson;
        }

        public ICompetitionRule GetRule(int year, CompetitionRuleScope competitionScope)
        {
            var seasonConfig = rulesJson.GetProperty(year.ToString());
            var compConfig = seasonConfig.GetProperty(competitionScope.ToString());

            return CompetitionRuleFactory.Create(compConfig);
        }
    }
}