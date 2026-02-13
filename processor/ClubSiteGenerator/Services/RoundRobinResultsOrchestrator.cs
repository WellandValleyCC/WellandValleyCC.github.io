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

        private void GenerateIndex(string indexFileName)
        {
            var repoRoot = FolderLocator.FindGitRepoRoot();

            var pipeline = CreateAssetPipeline();
            var result = pipeline.CopyRoundRobinAssets(repoRoot, competitionYear);
            var cssFile = result.CssFile;

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

        private AssetPipeline CreateAssetPipeline()
        {
            var directoryProvider = new DefaultDirectoryProvider();
            var fileProvider = new DefaultFileProvider();
            var log = new DefaultLog();

            var copyHelper = new DefaultDirectoryCopyHelper(
                directoryProvider,
                fileProvider,
                log
            );

            return new AssetPipeline(
                new DefaultAssetCopier(),
                copyHelper,
                directoryProvider,
                log
            );
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
