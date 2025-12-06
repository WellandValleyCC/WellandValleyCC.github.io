using ClubSiteGenerator.Interfaces;
using System.Text.Json;

namespace ClubSiteGenerator.Services
{
    public static class CompetitionRuleFactory
    {
        public static ICompetitionRule Create(JsonElement ruleConfig)
        {
            var ruleClass = ruleConfig.GetProperty("ruleClass").GetString();

            return ruleClass switch
            {
                nameof(CompetitionRuleHalfPlusOneCapped) =>
                    new CompetitionRuleHalfPlusOneCapped(
                        ruleConfig.GetProperty("cap").GetInt32()),

                nameof(CompetitionRuleDefinedNumberOfEvents) =>
                    new CompetitionRuleDefinedNumberOfEvents(
                        ruleConfig.GetProperty("fixedNumber").GetInt32()),

                _ => throw new InvalidOperationException($"Unknown rule class: {ruleClass}")
            };
        }
    }
}
