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
            var prevLinkHtml = string.IsNullOrEmpty(resultsSet.PrevLink)
                ? ""
                : $@"<a class=""nav-prev"" href=""{resultsSet.PrevLink}"">&laquo; {resultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(resultsSet.NextLink)
                ? ""
                : $@"<a class=""nav-next"" href=""{resultsSet.NextLink}"">{resultsSet.NextLabel} &raquo;</a>";

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <title>{resultsSet.DisplayName}</title>
    <style>
        body {{
            font-family: sans-serif;
            margin: 2rem;
        }}
        .nav {{
            display: flex;
            justify-content: space-between;
            margin-bottom: 2rem;
        }}
        .nav a {{
            text-decoration: none;
            color: #0066cc;
            font-weight: bold;
        }}
    </style>
</head>
<body>

    <div class=""nav"">
        <div>{prevLinkHtml}</div>
        <div>{nextLinkHtml}</div>
    </div>

    <h1>{resultsSet.DisplayName}</h1>
    <p>This is a placeholder page for Round Robin event {resultsSet.EventNumber}.</p>
    <p>The real renderer will output full event results here.</p>

    <p><a href=""../{indexFileName}"">Back to index</a></p>

</body>
</html>";
        }
    }
}