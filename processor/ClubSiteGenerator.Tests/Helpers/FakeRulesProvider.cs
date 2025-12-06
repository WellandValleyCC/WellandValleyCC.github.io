using ClubCore.Models.Enums;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Services;
using System;

namespace ClubSiteGenerator.Tests.Helpers
{
    internal class FakeRulesProvider : ICompetitionRulesProvider
    {
        public ICompetitionRule GetRule(int year, CompetitionRuleScope scope)
        {
            return scope switch
            {
                CompetitionRuleScope.TenMile => new CompetitionRuleDefinedNumberOfEvents(fixedNumber: 8),
                CompetitionRuleScope.Full => new CompetitionRuleHalfPlusOneCapped(cap: 11),
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, "Unsupported scope")
            };
        }
    }

}
