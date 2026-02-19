using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public abstract class RoundRobinCompetitionPageRenderer<T>
        : RoundRobinPageRenderer
    {
        protected new RoundRobinCompetitionResultsSet<T> ResultsSet { get; }

        protected RoundRobinCompetitionPageRenderer(
            string indexFileName,
            RoundRobinCompetitionResultsSet<T> resultsSet)
            : base(indexFileName, resultsSet)
        {
            ResultsSet = resultsSet;
        }
    }
}
