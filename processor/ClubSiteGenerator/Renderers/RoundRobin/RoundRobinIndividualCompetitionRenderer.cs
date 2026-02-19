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

    <nav class=""event-nav"" aria-label=""Competition navigation"">
      {prevLinkHtml}
      <a class=""index"" href=""../{IndexFileName}"">Index</a>
      {nextLinkHtml}
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