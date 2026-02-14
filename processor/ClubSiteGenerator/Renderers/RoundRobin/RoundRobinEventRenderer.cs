using ClubSiteGenerator.ResultsGenerator.RoundRobin;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public sealed class RoundRobinEventRenderer
    {
        private readonly string indexFileName;
        private readonly RoundRobinEventResultsSet resultsSet;

        public RoundRobinEventRenderer(string indexFileName, RoundRobinEventResultsSet resultsSet)
        {
            this.indexFileName = indexFileName;
            this.resultsSet = resultsSet;
        }

        public string Render()
        {
            // Placeholder HTML until the real renderer is implemented.
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <title>{resultsSet.DisplayName}</title>
</head>
<body>
    <h1>{resultsSet.DisplayName}</h1>
    <p>This is a placeholder page for Round Robin event {resultsSet.EventNumber}.</p>
    <p>The real renderer will output full event results here.</p>

    <p><a href=""../{indexFileName}"">Back to index</a></p>
</body>
</html>";
        }
    }
}