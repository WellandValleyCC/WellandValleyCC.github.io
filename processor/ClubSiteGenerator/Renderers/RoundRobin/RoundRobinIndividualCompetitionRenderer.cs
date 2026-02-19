using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinIndividualCompetitionRenderer : RoundRobinPageRenderer
    {
        private readonly int IndividualCompetitionEventLimit;

        public RoundRobinIndividualCompetitionRenderer(
            string indexFileName,
            RoundRobinResultsSet resultsSet)
            : base(indexFileName, resultsSet)
        {
            IndividualCompetitionEventLimit = resultsSet.CompetitionRules?.RoundRobin.Count ?? 0;
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

            var prevLinkHtml = string.IsNullOrEmpty(ResultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{ResultsSet.PrevLink}"" aria-label=""Previous"">{ResultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(ResultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{ResultsSet.NextLink}"" aria-label=""Next"">{ResultsSet.NextLabel}</a>";

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
        //  LEGEND (INDIVIDUAL COMPETITIONS ALWAYS SHOW IT)
        // ------------------------------------------------------------

        // N/A - use default legend

        // ------------------------------------------------------------
        //  MAIN CONTENT
        // ------------------------------------------------------------

        protected override string RenderMainContent()
        {
            return $@"
{RenderCompetitionRules()}

<p>Individual competition standings will appear here.</p>";
        }
    }
}