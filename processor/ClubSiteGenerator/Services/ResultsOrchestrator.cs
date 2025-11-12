using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.Services
{
    public class ResultsOrchestrator
    {
        private readonly List<BaseResults> _resultsGenerators = new();

        public ResultsOrchestrator(List<Ride> rides)
        {
            // Discover all event numbers dynamically
            var eventNumbers = rides.Select(r => r.EventNumber).Distinct();
            foreach (var e in eventNumbers)
                _resultsGenerators.Add(new EventResults(e, rides));

            // Later: competitions auto‑discovered via reflection
        }

        public void GenerateAll()
        {
            foreach (var generator in _resultsGenerators)
            {
                var table = generator.CreateTable();
                var html = ResultsRenderer.RenderAsHtml(table);
                var outputDir = OutputLocator.GetOutputDirectory();
                File.WriteAllText(Path.Combine(outputDir, $"{generator.Name}.html"), html);
            }
        }
    }

}
