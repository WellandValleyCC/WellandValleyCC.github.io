using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinTeamCompetitionRenderer : RoundRobinPageRenderer
    {
        private readonly int OpenCompetitionEventCount;
        private readonly int WomenCompetitionEventCount;

        public RoundRobinTeamCompetitionRenderer(
            string indexFileName,
            RoundRobinResultsSet resultsSet)
            : base(indexFileName, resultsSet)
        {
            OpenCompetitionEventCount = resultsSet.CompetitionRules?.RoundRobin.Team.OpenCount ?? 0;
            WomenCompetitionEventCount = resultsSet.CompetitionRules?.RoundRobin.Team.WomenCount ?? 0;
        }

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        protected override string GetPageTitle() =>
            CleanTitle(ResultsSet.DisplayName);

        // ------------------------------------------------------------
        //  HEADER (TEAM COMPETITION)
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
        //  LEGEND (TEAM COMPETITION DOES NOT USE IT)
        // ------------------------------------------------------------

        // N/A - use default legend

        // ------------------------------------------------------------
        //  MAIN CONTENT
        // ------------------------------------------------------------


        protected override string RenderMainContent()
        {
            return $@"
    {RenderCompetitionRules()}

    <p>Team competition standings will appear here</p>";
        }

        private string RenderCompetitionRules()
        {
            string RiderPhrase(int n) =>
                n == 1 ? "top rider's" : $"top {n} riders'";

            return $@"
<div class=""rules-and-legend"">
  <section class=""competition-rules"">
    <p>
      Each club's score at an event is the sum of their {RiderPhrase(OpenCompetitionEventCount)} points
      in the open competition plus the {RiderPhrase(WomenCompetitionEventCount)} points
      in the women's competition.
    </p>
  </section>
</div>";
        }
    }
}