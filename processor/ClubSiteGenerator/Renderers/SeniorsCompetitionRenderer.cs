using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.Renderers
{
    internal class SeniorsCompetitionRenderer : CompetitionRenderer
    {
        public SeniorsCompetitionRenderer(CompetitionResultsSet resultsSet, ICompetitionRules rules) 
            : base(resultsSet, rules)
        {
        }

        protected override string RenderNameCell(Competitor competitor, string encodedValue)
        {
            var baseName = $"<td class=\"competitor-name\">{encodedValue}</td>";
            var leagueBadge = competitor!.League != League.Undefined
                ? $"<span class=\"league-badge\">{competitor.League}</span>"
                : string.Empty;

            // Embed badge inside the cell
            return $"<td class=\"competitor-name\">{encodedValue}{leagueBadge}</td>";
        }
    }
}
