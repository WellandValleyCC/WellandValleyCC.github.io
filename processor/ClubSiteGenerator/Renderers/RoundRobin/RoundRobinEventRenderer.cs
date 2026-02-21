using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Extensions;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinEventRenderer : RoundRobinPageRenderer
    {
        private readonly RoundRobinEventResultsSet eventResults;

        private readonly string eventTitle;
        private readonly int eventNumber;
        private readonly DateOnly eventDate;
        private readonly double eventMiles;
        private readonly bool isCancelled;
        private readonly string eventDistanceText;

        public RoundRobinEventRenderer(string indexFileName, RoundRobinEventResultsSet resultsSet)
            : base(indexFileName, resultsSet)
        {
            eventResults = resultsSet;

            eventTitle = CleanTitle(resultsSet.DisplayName);
            eventNumber = resultsSet.EventNumber;
            eventDate = resultsSet.EventDate;
            eventMiles = resultsSet.CalendarEvent.Miles;
            isCancelled = resultsSet.CalendarEvent.IsCancelled;

            eventDistanceText = $"{resultsSet.CalendarEvent.Miles:0.#} miles";
        }

        // ------------------------------------------------------------
        //  PAGE TITLE
        // ------------------------------------------------------------

        protected override string GetPageTitle() => eventTitle;

        // ------------------------------------------------------------
        //  HEADER (EVENT-SPECIFIC)
        // ------------------------------------------------------------

        protected override string RenderHeader()
        {
            var prevLinkHtml = string.IsNullOrEmpty(eventResults.PrevLink)
                ? ""
                : $@"<a class=""prev"" href=""{eventResults.PrevLink}"" aria-label=""Previous"">{eventResults.PrevLabel}</a>";

            var nextLinkHtml = string.IsNullOrEmpty(eventResults.NextLink)
                ? ""
                : $@"<a class=""next"" href=""{eventResults.NextLink}"" aria-label=""Next"">{eventResults.NextLabel}</a>";

            var hostClubName = FormatHosts(eventResults.CalendarEvent.RoundRobinClub);
            var rrHeaderDate = eventDate.ToString("dddd, dd MMMM yyyy", CultureInfo.InvariantCulture);

            var headerClasses = isCancelled
                ? "event-header-core cancelled-event"
                : "event-header-core";

            return $@"
<header>
  <div class=""rr-banner-header"">
    <div class=""header-and-legend"">
      <div class=""{headerClasses}"">
        <h1>
          <span class=""event-number"">Event {eventNumber}:</span>
          {eventTitle}
        </h1>
        <p class=""event-host"">Hosted by {hostClubName}</p>
        <p class=""event-date"">{rrHeaderDate}</p>
        <p class=""event-distance"">Distance: {eventDistanceText}</p>
      </div>
    </div>

    {RenderNavigationPills()}
  <div>
</header>";
        }

        // ------------------------------------------------------------
        //  LEGEND (EVENT PAGES ALWAYS SHOW IT)
        // ------------------------------------------------------------

        protected override string RenderLegendIfNeeded() => @"
<div class=""legend"">
  <span class=""competition-eligible"">Club Member</span>
  <span class=""guest-non-club-member"">Guest</span>
</div>";

        // ------------------------------------------------------------
        //  MAIN CONTENT (RESULTS TABLE)
        // ------------------------------------------------------------

        protected override string RenderMainContent()
        {
            var sb = new StringBuilder();
            sb.AppendLine("<table class=\"results\">");
            sb.AppendLine(RenderHeaderRow());
            sb.AppendLine(RenderBody());
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        //  TABLE STRUCTURE
        // ------------------------------------------------------------

        private readonly List<string> columnTitles = new()
        {
            "Name", "Club", "Position", "Road Bike", "Actual Time", "Avg. mph"
        };

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

            foreach (var ride in eventResults.Rides)
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

        // ------------------------------------------------------------
        //  CELL BUILDING
        // ------------------------------------------------------------

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

            if (index == 2)
                return RenderPositionCell(encoded, tdClass, ride);

            // Default behaviour for all other cells
            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{encoded}</td>"
                : $"<td class=\"{tdClass}\">{encoded}</td>";
        }

        private string RenderPositionCell(string encodedRank, string tdClass, Ride ride)
        {
            var hidden = BuildHiddenPointsHtml(ride);

            // No points → normal cell
            if (string.IsNullOrEmpty(hidden))
            {
                return string.IsNullOrEmpty(tdClass)
                    ? $"<td>{encodedRank}</td>"
                    : $"<td class=\"{tdClass}\">{encodedRank}</td>";
            }

            // Points exist → wrap in clickable wrapper
            var wrapper = $@"
<div class=""rr-event-points-wrapper"" onclick=""this.classList.toggle('expanded')"">
    <div class=""rr-pos-visible"">{encodedRank}</div>
    {hidden}
</div>";

            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{wrapper}</td>"
                : $"<td class=\"{tdClass}\">{wrapper}</td>";
        }

        private static string BuildHiddenPointsHtml(Ride ride)
        {
            if (ride.RoundRobinPoints == null && ride.RoundRobinWomenPoints == null)
                return string.Empty;

            var open = ride.RoundRobinPoints.HasValue
                ? $"Open: {ride.RoundRobinPoints.Value} pts"
                : string.Empty;

            var women = ride.RoundRobinWomenPoints.HasValue
                ? $"Women: {ride.RoundRobinWomenPoints.Value} pts"
                : string.Empty;

            return $@"
<div class=""rr-event-points-collapsed"">
  <div class=""rr-event-points-inner"">
    <div class=""rr-event-open"">{open}</div>
    <div class=""rr-event-women"">{women}</div>
  </div>
</div>";
        }

        // ------------------------------------------------------------
        //  ROW CLASSIFICATION
        // ------------------------------------------------------------

        private static string GetRowClass(Ride ride) =>
            !string.IsNullOrWhiteSpace(ride.RoundRobinClub)
                ? "competition-eligible"
                : "guest-non-club-member";
    }
}