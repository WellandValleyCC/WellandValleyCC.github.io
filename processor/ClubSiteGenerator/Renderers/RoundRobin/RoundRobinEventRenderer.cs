using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Globalization;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinEventRenderer
    {
        private readonly string indexFileName;
        private readonly RoundRobinEventResultsSet resultsSet;

        private readonly string eventTitle;
        private readonly int eventNumber;
        private readonly DateOnly eventDate;
        private readonly double eventMiles;
        private readonly bool isCancelled;

        private readonly string eventDistanceText;

        public RoundRobinEventRenderer(string indexFileName, RoundRobinEventResultsSet resultsSet)
        {
            this.indexFileName = indexFileName;
            this.resultsSet = resultsSet;

            this.eventTitle = resultsSet.DisplayName;
            this.eventNumber = resultsSet.EventNumber;
            this.eventDate = resultsSet.EventDate;
            this.eventMiles = resultsSet.CalendarEvent.Miles;
            this.isCancelled = resultsSet.CalendarEvent.IsCancelled;

            this.eventDistanceText = $"{resultsSet.CalendarEvent.Miles:0.#} miles";
        }

        public string Render()
        {
            var prevLinkHtml = string.IsNullOrEmpty(resultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{resultsSet.PrevLink}"" aria-label=""Previous"">{resultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(resultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{resultsSet.NextLink}"" aria-label=""Next"">{resultsSet.NextLabel}</a>";

            var eventDateText = resultsSet.EventDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
    <title>{eventTitle}</title>
    <link rel=""stylesheet"" href=""../assets/{resultsSet.CssFile}"">
</head>
<body class=""rr event-page"">

<header>
  <div class=""header-and-legend"">
    <div class=""event-header-core"">
      <h1>{eventTitle}</h1>
      <p class=""event-date"">{eventDateText}</p>
      <p class=""event-distance"">Distance: {eventDistanceText}</p>
    </div>

    <nav class=""event-nav"" aria-label=""Event navigation"">
      {prevLinkHtml}
      <a class=""index"" href=""../{indexFileName}"" aria-label=""Back to index"">Index</a>
      {nextLinkHtml}
    </nav>
  </div>
</header>

<main>
  <p>This is a placeholder page for Round Robin event {eventNumber}.</p>
  <p>The real renderer will output full event results here.</p>
</main>

</body>
</html>";
        }
    }
}