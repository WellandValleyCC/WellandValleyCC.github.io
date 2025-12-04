using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.Renderers
{
    internal class SeniorsCompetitionRenderer : CompetitionRenderer
    {
        public SeniorsCompetitionRenderer(CompetitionResultsSet resultsSet, IEnumerable<CalendarEvent> calendar) : base(resultsSet, calendar)
        {
        }
    }
}
