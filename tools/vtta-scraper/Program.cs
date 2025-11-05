namespace vttaScraper
{
    internal static class Program

    {
        private static async Task<int> Main(string[] args)
        { 
            var distances = new[] { "0.88", "5", "8.5", "9", "10", "11", "13.5", "20", "25" };
            var headless = true;

            try
            {
                var scraper = new Scraper(headless);
                var aggregator = new VttaAggregator();

                foreach (var distance in distances)
                {
                    Console.WriteLine($"[INFO] Processing distance {distance} miles.");

                    var rows = await scraper.ScrapeAsync(distance);
                    aggregator.Add(distance, rows);
                }

                var outputPath = "vtta-standards-combined.csv";
                CsvWriter.Write(outputPath, aggregator.GetMerged());
                Console.WriteLine($"[INFO] Combined CSV written to: {Path.GetFullPath(outputPath)}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[ERROR]: {ex.Message}");
                return 1;
            }
        }
    }
}