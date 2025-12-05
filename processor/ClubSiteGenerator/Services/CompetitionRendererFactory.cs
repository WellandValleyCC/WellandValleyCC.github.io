using ClubCore.Models;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Renderers;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.Services
{
    internal static class CompetitionRendererFactory
    {
        public static CompetitionRenderer Create(
            CompetitionResultsSet resultsSet,
            IEnumerable<CalendarEvent> calendar)
        {
            return resultsSet.CompetitionType switch
            {
                CompetitionType.Seniors => new SeniorsCompetitionRenderer(resultsSet),
                CompetitionType.NevBrooks => new NevBrooksCompetitionRenderer(resultsSet),
                _ => new CompetitionRenderer(resultsSet)
            };
        }
    }
}
