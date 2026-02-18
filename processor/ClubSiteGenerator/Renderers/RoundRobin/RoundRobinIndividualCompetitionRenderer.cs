using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinIndividualCompetitionRenderer : RoundRobinPageRenderer
    {
        public RoundRobinIndividualCompetitionRenderer(
            string indexFileName,
            RoundRobinResultsSet resultsSet)
            : base(indexFileName, resultsSet)
        {
        }

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        protected override string GetPageTitle() =>
            CleanTitle(ResultsSet.DisplayName);

        // ------------------------------------------------------------
        //  HEADER (COMPETITION-SPECIFIC)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var title = CleanTitle(ResultsSet.DisplayName);

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""event-header-core"">
        <h1>{title}</h1>
      </div>
    </div>

    <nav class=""event-nav"" aria-label=""Competition navigation"">
      <a class=""index"" href=""../{IndexFileName}"">Index</a>
    </nav>
  </div>
</header>";
        }

        // ------------------------------------------------------------
        //  LEGEND (INDIVIDUAL COMPETITIONS ALWAYS SHOW IT)
        // ------------------------------------------------------------

        protected override string RenderLegendIfNeeded() => @"
<div class=""legend"">
  <span class=""competition-eligible"">Club Member</span>
  <span class=""guest-non-club-member"">Guest</span>
</div>";

        // ------------------------------------------------------------
        //  MAIN CONTENT
        // ------------------------------------------------------------

        protected override string RenderMainContent()
        {
            return "<p>Individual competition standings will appear here.</p>";
        }
    }
}