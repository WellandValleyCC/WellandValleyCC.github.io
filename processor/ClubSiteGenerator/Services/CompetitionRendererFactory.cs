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
            string indexFileName,
            CompetitionResultsSet resultsSet,
            IEnumerable<CalendarEvent> calendar,
            ICompetitionRules rules)
        {
            return resultsSet.CompetitionType switch
            {
                CompetitionType.Seniors => new SeniorsCompetitionRenderer(indexFileName, resultsSet, rules),
                CompetitionType.NevBrooks => new NevBrooksCompetitionRenderer(indexFileName, resultsSet, rules),
                _ => new CompetitionRenderer(indexFileName, resultsSet, rules)
            };
        }
    }
}
