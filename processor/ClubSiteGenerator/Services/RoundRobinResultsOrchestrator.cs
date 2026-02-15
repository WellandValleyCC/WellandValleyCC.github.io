using ClubCore.Models;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Renderers.RoundRobin;
using ClubSiteGenerator.ResultsGenerator;
using ClubSiteGenerator.ResultsGenerator.RoundRobin;
using ClubSiteGenerator.Utilities;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public class RoundRobinResultsOrchestrator
    {
        private readonly List<ResultsSet> resultsSets = new();

        private readonly string outputDir;
        private readonly IEnumerable<Ride> rides;
        private readonly IEnumerable<Competitor> competitors;
        private readonly IEnumerable<CalendarEvent> calendar;
        private readonly IEnumerable<RoundRobinClub> clubs;

        private readonly int competitionYear;

        private string cssFile = "";

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

            // Guard: calendar must contain only RR events
            if (calendar.Any(ev => !ev.IsRoundRobinEvent))
            {
                throw new ArgumentException(
                    "RoundRobinResultsOrchestrator must be constructed with a calendar containing only Round Robin events.",
                    nameof(calendar));
            }

            // Determine competition year from first event
            competitionYear = calendar.First().EventDate.Year;
        }

        public void GenerateAll(string indexFileName)
        {
            PrepareAssets();
            InitializeResultsSets();
            WirePrevNextLinks();
            GeneratePages(indexFileName);
            GenerateIndex(indexFileName);
        }

        private void InitializeResultsSets()
        {
            foreach (var ev in calendar)
            {
                resultsSets.Add(
                    RoundRobinEventResultsSet.CreateFrom(
                        calendar,
                        rides,
                        ev.RoundRobinEventNumber));
            }
        }

        private void WirePrevNextLinks()
        {
            var orderedEvents = resultsSets
                .OfType<RoundRobinEventResultsSet>()
                .OrderBy(ev => ev.EventNumber)
                .Cast<IResultsSet>()
                .ToList();

            if (orderedEvents.Count <= 1)
                return;

            for (int i = 0; i < orderedEvents.Count; i++)
            {
                var current = orderedEvents[i];
                var prev = orderedEvents[(i - 1 + orderedEvents.Count) % orderedEvents.Count];
                var next = orderedEvents[(i + 1) % orderedEvents.Count];

                current.PrevLink = $"../{prev.SubFolderName}/{prev.FileName}.html";
                current.NextLink = $"../{next.SubFolderName}/{next.FileName}.html";
                current.PrevLabel = prev.LinkText;
                current.NextLabel = next.LinkText;
            }
        }

        private void GeneratePages(string indexFileName)
        {
            foreach (var resultsSet in resultsSets.OfType<RoundRobinEventResultsSet>())
            {
                resultsSet.CssFile = cssFile;

                var renderer = new RoundRobinEventRenderer(indexFileName, resultsSet);

                Console.WriteLine($"Generating RR event results for: {resultsSet.FileName}");

                var html = renderer.Render();

                var folderPath = Path.Combine(outputDir, resultsSet.SubFolderName);
                Directory.CreateDirectory(folderPath);

                File.WriteAllText(Path.Combine(folderPath, $"{resultsSet.FileName}.html"), html);
            }
        }

        private void GenerateIndex(string indexFileName)
        {
            var folderLocator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            var outputRoot = outputDir;

            var rrEventResults = resultsSets
                .OfType<RoundRobinEventResultsSet>()
                .OrderBy(ev => ev.EventDate)
                .ToList();

            var renderer = new RoundRobinIndexRenderer(
                calendar,
                clubs,
                outputRoot,
                cssFile,
                rrEventResults);

            renderer.RenderIndex(indexFileName);
            RenderRedirectIndex(indexFileName);
        }

        private void PrepareAssets()
        {
            var folderLocator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());
        
            var repoRoot = folderLocator.FindGitRepoRoot();
            var assetsRoot = Path.Combine(repoRoot, PathTokens.RoundRobinAssetsFolder);
            var outputRoot = outputDir;

            var pipeline = CreateAssetPipeline();
            var result = pipeline.CopyRoundRobinAssets(
                assetsRoot,
                outputRoot,
                competitionYear,
                PathTokens.RoundRobinCssPrefix,
                "Round Robin");

            cssFile = result.CssFile;
        }

        private AssetPipeline CreateAssetPipeline()
        {
            var directoryProvider = new DefaultDirectoryProvider();
            var fileProvider = new DefaultFileProvider();
            var log = new DefaultLog();

            var copyHelper = new DefaultDirectoryCopyHelper(
                directoryProvider,
                fileProvider,
                log);

            return new AssetPipeline(
                new DefaultAssetCopier(),
                copyHelper,
                directoryProvider,
                log);
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

            path = Path.Combine(outputDir, "index.htm");
            File.WriteAllText(path, sb.ToString());
        }
    }
}