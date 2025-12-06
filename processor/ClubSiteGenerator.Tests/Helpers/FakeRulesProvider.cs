using ClubCore.Models.Enums;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.Tests.Helpers
{
    /// <summary>
    /// Deterministic rules provider for unit tests.
    /// Always returns simple, predictable rules so tests don't depend on JSON.
    /// </summary>
    internal class FakeRulesProvider : ICompetitionRulesProvider
    {
        public ICompetitionRule GetRule(int year, CompetitionRuleScope scope)
        {
            return scope switch
            {
                // Best 8 ten‑mile rides
                CompetitionRuleScope.TenMile =>
                    new DefinedNumberOfEventsRule(fixedNumber: 8),

                // Full competition: half‑plus‑one capped at 11, with 2 required non‑tens
                CompetitionRuleScope.Full =>
                    new HalfPlusOneMixedCappedRule(cap: 11, requiredNonTens: 2),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(scope), scope, "Unsupported competition scope")
            };
        }
    }
}

