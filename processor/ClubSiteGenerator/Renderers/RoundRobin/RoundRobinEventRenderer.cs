using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Extensions;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

            this.eventTitle = CleanTitle(resultsSet.DisplayName);
            this.eventNumber = resultsSet.EventNumber;
            this.eventDate = resultsSet.EventDate;
            this.eventMiles = resultsSet.CalendarEvent.Miles;
            this.isCancelled = resultsSet.CalendarEvent.IsCancelled;

            this.eventDistanceText = $"{resultsSet.CalendarEvent.Miles:0.#} miles";
        }

        private static string CleanTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return title;

            // Remove “Round Robin” in any casing, with optional extra spaces
            var cleaned = Regex.Replace(
                title,
                @"\s*round\s*robin\s*",
                "",
                RegexOptions.IgnoreCase
            );

            return cleaned.Trim();
        }

        private static string FormatHosts(string rawHosts)
        {
            if (string.IsNullOrWhiteSpace(rawHosts))
                return string.Empty;

            // Split on comma, trim whitespace, remove empties
            var parts = rawHosts
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0)
                .ToList();

            if (parts.Count == 0)
                return string.Empty;

            if (parts.Count == 1)
                return parts[0];

            if (parts.Count == 2)
                return $"{parts[0]} & {parts[1]}";

            // 3 or more clubs → Oxford comma style
            return string.Join(", ", parts.Take(parts.Count - 1))
                   + " & " + parts.Last();
        }

        public string Render()
        {
            var timestamp = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm 'UTC'");

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

{RenderHeader()}

{RenderLegend()}

<main>
  {RenderResultsTable()}
</main>

<footer><p class=""generated"">Generated {timestamp}</p></footer>

</body>
</html>";
        }

        private string RenderHeader()
        {
            var prevLinkHtml = string.IsNullOrEmpty(resultsSet.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{resultsSet.PrevLink}"" aria-label=""Previous"">{resultsSet.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(resultsSet.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{resultsSet.NextLink}"" aria-label=""Next"">{resultsSet.NextLabel}</a>";

            var hostClubName = FormatHosts(resultsSet.CalendarEvent.RoundRobinClub);
            var rrHeaderTitle = eventTitle;
            var rrHeaderDate = eventDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""event-header-core"">
        <h1>
          <span class=""event-number"">Event {eventNumber}:</span>
          {rrHeaderTitle}
        </h1>
        <p class=""event-host"">Hosted by {hostClubName}</p>
        <p class=""event-date"">{rrHeaderDate}</p>
        <p class=""event-distance"">Distance: {eventDistanceText}</p>
      </div>
    </div>

    <nav class=""event-nav"" aria-label=""Event navigation"">
      {prevLinkHtml}
      <a class=""index"" href=""../{indexFileName}"" aria-label=""Back to index"">Index</a>
      {nextLinkHtml}
    </nav>
  <div>
</header>";
        }

        private string RenderLegend()
        {
            return @"
<div class=""legend"">
  <span class=""competition-eligible"">Club Member</span>
  <span class=""guest-non-club-member"">Guest</span>
</div>";
        }

        //
        // RESULTS TABLE
        //

        private readonly List<string> columnTitles = new()
{
    "Name", "Club", "Position", "Road Bike", "Actual Time", "Avg. mph"
};

        private string RenderResultsTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table class=\"results\">");
            sb.AppendLine(RenderHeaderRow());
            sb.AppendLine(RenderBody());
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        private string RenderHeaderRow()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<thead><tr>");

            foreach (var title in columnTitles)
                sb.AppendLine($"<th>{WebUtility.HtmlEncode(title)}</th>");

            sb.AppendLine("</tr></thead>");
            return sb.ToString();
        }

        private string RenderBody()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<tbody>");

            foreach (var ride in resultsSet.Rides)
                sb.AppendLine(RenderRow(ride));

            sb.AppendLine("</tbody>");
            return sb.ToString();
        }

        private string RenderRow(Ride ride)
        {
            var sb = new StringBuilder();
            var cssClass = GetRowClass(ride);

            sb.AppendLine($"<tr class=\"{cssClass}\">");

            foreach (var cell in BuildCells(ride).Select((value, index) => RenderCell(value, index, ride)))
                sb.AppendLine(cell);

            sb.AppendLine("</tr>");
            return sb.ToString();
        }

        private IEnumerable<string> BuildCells(Ride ride)
        {
            var timeCell = ride.Status switch
            {
                RideStatus.DNF => "DNF",
                RideStatus.DNS => "DNS",
                RideStatus.DQ => "DQ",
                _ => TimeSpan.FromSeconds(ride.TotalSeconds).ToString(@"hh\:mm\:ss")
            };

            var cleanName = StripClubSuffix(ride.Name ?? "Unknown", ride.RoundRobinClub);
            yield return cleanName;

            yield return ride.RoundRobinClub ?? "";
            yield return ride.EventRank?.ToString() ?? "";
            yield return ride.EventRoadBikeRank?.ToString() ?? "";
            yield return timeCell;
            yield return ride.AvgSpeed?.ToString("0.00") ?? string.Empty;
        }

        private string RenderCell(string value, int index, Ride ride)
        {
            var encoded = WebUtility.HtmlEncode(value);
            var tdClass = index switch
            {
                2 => ride.GetRREligibleRidersRankClass(),
                3 => ride.GetRREligibleRoadBikeRidersRankClass(),
                _ => string.Empty
            };

            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{encoded}</td>"
                : $"<td class=\"{tdClass}\">{encoded}</td>";
        }

        private static string GetRowClass(Ride ride) =>
            !string.IsNullOrWhiteSpace(ride.RoundRobinClub)
                ? "competition-eligible"
                : "guest-non-club-member";

        private static string StripClubSuffix(string name, string? roundRobinClub)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(roundRobinClub))
                return name;

            // Pattern: "(XYZ)" at end of string, with optional whitespace
            var pattern = @"\s*\(" + Regex.Escape(roundRobinClub.Trim()) + @"\)\s*$";

            return Regex.Replace(name, pattern, "", RegexOptions.IgnoreCase).TrimEnd();
        }
    }
}