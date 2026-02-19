using ClubSiteGenerator.Models.RoundRobin;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinTeamCompetitionRenderer
        : RoundRobinCompetitionPageRenderer<RoundRobinTeamResult>
    {
        public RoundRobinTeamCompetitionRenderer(
            string indexFileName,
            RoundRobinCompetitionResultsSet<RoundRobinTeamResult> resultsSet)
            : base(indexFileName, resultsSet)
        {
        }

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        private string PageTitle => ResultsSet.DisplayName;

        protected override string GetPageTitle() => PageTitle;

        // ------------------------------------------------------------
        //  HEADER (TEAM COMPETITION)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var title = PageTitle;

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""event-header-core"">
        <h1>{title}</h1>
      </div>
    </div>

    {RenderNavigationPills()}
  </div>
</header>";
        }

        // ------------------------------------------------------------
        //  LEGEND (TEAM COMPETITION DOES NOT USE IT)
        // ------------------------------------------------------------

        protected override string RenderLegendIfNeeded() => string.Empty;

        // ------------------------------------------------------------
        //  MAIN CONTENT
        // ------------------------------------------------------------

        protected override string RenderMainContent()
        {
            return $@"
{RenderCompetitionRules()}

<p>Team competition standings will appear here</p>";
        }
    }
}