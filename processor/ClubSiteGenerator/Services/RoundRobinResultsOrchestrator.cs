using ClubCore.Models;
using ClubCore.Utilities;
using ClubSiteGenerator.Utilities;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public class RoundRobinResultsOrchestrator
    {
        private readonly string outputDir;
        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<Competitor> competitors;
        private readonly IEnumerable<CalendarEvent> calendar;
        private readonly IEnumerable<RoundRobinClub> clubs;

        private readonly int competitionYear;

        public RoundRobinResultsOrchestrator(
            string outputDir,
            IEnumerable<Ride> rides,
            IEnumerable<Competitor> competitors,
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinClub> clubs)
        {
            this.outputDir = outputDir;
            this.rides = rides;
            this.competitors = competitors;
            this.calendar = calendar;
            this.clubs = clubs;

            // Determine competition year from first event
            competitionYear = calendar.First().EventDate.Year;
        }

        public void GenerateAll(string indexFileName)
        {
            GenerateIndex(indexFileName);
        }

        public void GenerateIndex(string indexFileName)
        {
            var repoRoot = FolderLocator.FindGitRepoRoot();

            var cssFile = AssetPipeline.CopyRoundRobinAssets(repoRoot, competitionYear);

            var outputRoot = Path.Combine(repoRoot, PathTokens.RoundRobinOutputFolder);

            var renderer = new RoundRobinIndexRenderer(
                calendar,
                clubs,
                outputRoot,
                cssFile
            );

            renderer.RenderIndex(indexFileName);

            RenderRedirectIndex(indexFileName);
        }

        private string BuildPlaceholderHtml()
        {
            var rows = string.Join("\n",
                calendar.Select(e =>
                    $"<tr>" +
                    $"<td>{e.RoundRobinEventNumber}</td>" +
                    $"<td>{e.EventDate:ddd dd MMM}</td>" +
                    $"<td>{e.EventName}</td>" +
                    $"</tr>"
                ));

            const string template = """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Round Robin TT</title>
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif;
            background: #f5f5f5;
            color: #333;
            display: flex;
            align-items: flex-start;
            justify-content: center;
            min-height: 100vh;
            text-align: center;
        }
        .container {
            padding: 2rem;
            max-width: 900px;
        }
        h1 {
            font-size: 2.5rem;
            margin-bottom: 0.5rem;
        }
        p {
            font-size: 1.2rem;
            opacity: 0.85;
        }
        table {
            width: 100%;
            border-collapse: collapse;
            margin: 2rem 0;
            background: white;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 4px rgba(0,0,0,0.08);
        }
        th, td {
            padding: 0.75rem 1rem;
            border-bottom: 1px solid #eee;
            text-align: left;
        }
        th {
            background: #fafafa;
            font-weight: 600;
        }
        tr:last-child td {
            border-bottom: none;
        }
        .clubs {
            margin-top: 2.5rem;
        }
        .club-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
            gap: 1.5rem;
            margin-top: 1.5rem;
        }
        .club {
            background: white;
            padding: 1rem;
            border-radius: 8px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.08);
        }
        .club img {
            max-width: 100px;
            max-height: 100px;
            margin-bottom: 0.5rem;
        }
        .club a {
            color: #0066cc;
            text-decoration: none;
            font-size: 0.95rem;
        }
        .club a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <div class="container">
        <h1>Round Robin TT</h1>
        <p>Website coming soon</p>

        <h2>Round Robin Calendar</h2>
        <table>
            <thead>
                <tr>
                    <th>#</th>
                    <th>Date</th>
                    <th>Event</th>
                </tr>
            </thead>
            <tbody>
                {{ROWS}}
            </tbody>
        </table>

        <div class="clubs">
            <h2>Participating Clubs</h2>

            <div class="club-grid">

                <div class="club">
                    <img src="logos/wvcc.png" alt="WVCC Logo">
                    <div><strong>WVCC</strong></div>
                    <a href="https://wellandvalleycc.co.uk/">Welland Valley Cycling Club</a>
                </div>

                <div class="club">
                    <img src="logos/hcrc.png" alt="HCRC Logo">
                    <div><strong>HCRC</strong></div>
                    <a href="https://hinckleycrc.org/">Hinckley Cycle Racing Club</a>
                </div>

                <div class="club">
                    <img src="logos/rfw.png" alt="RFW Logo">
                    <div><strong>RFW</strong></div>
                    <a href="https://www.rockinghamforestwheelers.org/">Rockingham Forest Wheelers</a>
                </div>

                <div class="club">
                    <img src="logos/ratae.png" alt="Ratae Logo">
                    <div><strong>Ratae</strong></div>
                    <a href="https://www.rataerc.org.uk/">Ratae Road Club</a>
                </div>

                <div class="club">
                    <img src="logos/lfcc.png" alt="LFCC Logo">
                    <div><strong>LFCC</strong></div>
                    <a href="https://www.leicesterforest.com/">Leicester Forest Cycling Club</a>
                </div>

            </div>
        </div>
    </div>
</body>
</html>
""";

            return template.Replace("{{ROWS}}", rows);
        }

        private void RenderRedirectIndex(string indexFileName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine($"  <meta http-equiv=\"refresh\" content=\"0; url={indexFileName}\">");
            sb.AppendLine("  <title>Round Robin TT Season Index</title>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine($"<p>Redirecting to <a href=\"{indexFileName}\">{competitionYear} Season</a></p>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            var path = Path.Combine(outputDir, "index.html");
            File.WriteAllText(path, sb.ToString());

            // Also write .htm version for legacy compatibility
            path = Path.Combine(outputDir, "index.htm");
            File.WriteAllText(path, sb.ToString());
        }
    }
}
