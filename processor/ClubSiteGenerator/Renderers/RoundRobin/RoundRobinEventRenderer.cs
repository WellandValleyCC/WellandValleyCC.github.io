using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models.Extensions;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using System.Net;
using System.Text;

namespace ClubSiteGenerator.Renderers.RoundRobin
{
    public class RoundRobinEventRenderer : RoundRobinPageRenderer
    {
        public RoundRobinEventRenderer(string indexFileName, RoundRobinEventResultsSet resultsSet)
            : base(indexFileName, resultsSet)
        {
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

            foreach (var ride in ResultsSet.Rides)
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

            return string.IsNullOrEmpty(tdClass)
                ? $"<td>{encoded}</td>"
                : $"<td class=\"{tdClass}\">{encoded}</td>";
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