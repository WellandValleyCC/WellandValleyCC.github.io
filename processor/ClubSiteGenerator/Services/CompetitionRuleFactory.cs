using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Services;
using System.Text.Json;

public static class CompetitionRuleFactory
{
    public static ICompetitionRule Create(JsonElement ruleConfig)
    {
        var ruleClass = ruleConfig.GetProperty("ruleClass").GetString();

        return ruleClass switch
        {
            "DefinedNumberOfEvents" =>
                new DefinedNumberOfEventsRule(
                    ruleConfig.GetProperty("fixedNumber").GetInt32()),

            "HalfPlusOneCapped" =>
                new HalfPlusOneCappedRule(
                    ruleConfig.GetProperty("cap").GetInt32()),

            "HalfPlusOneMixedCapped" =>
                new HalfPlusOneMixedCappedRule(
                    ruleConfig.GetProperty("cap").GetInt32(),
                    ruleConfig.GetProperty("requiredNonTens").GetInt32()),

            _ => throw new InvalidOperationException($"Unknown rule class: {ruleClass}")
        };
    }
}