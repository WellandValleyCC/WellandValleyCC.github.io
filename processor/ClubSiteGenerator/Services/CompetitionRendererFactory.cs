using ClubCore.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.Services
{
    internal static class CompetitionRendererFactory
    {
        public static CompetitionRenderer Create(
            CompetitionResultsSet resultsSet,
            IEnumerable<CalendarEvent> calendar,
            ICompetitionRules rules)
        {
            return resultsSet.CompetitionType switch
            {
                CompetitionType.Seniors => new SeniorsCompetitionRenderer(resultsSet, rules),
                CompetitionType.NevBrooks => new NevBrooksCompetitionRenderer(resultsSet, rules),
                _ => new CompetitionRenderer(resultsSet, rules)
            };
        }
    }
}
