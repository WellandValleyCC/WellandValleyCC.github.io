using ClubCore.Context;
using ClubCore.Utilities;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ClubSiteGenerator starting…");

            var yearIndex = Array.IndexOf(args, "--year");
            if (yearIndex == -1 || yearIndex + 1 >= args.Length)
            {
                Console.Error.WriteLine("[ERROR] Missing --year argument (e.g. --year 2025)");
                Environment.Exit(1);
            }

            var year = args[yearIndex + 1];

            Console.WriteLine($"[INFO] Processing year: {year}");
            
            // Decide output folder (repo-rooted TestOutput or CI temp)
            var outputDir = OutputLocator.GetOutputDirectory();
            Console.WriteLine($"Writing site to: {outputDir}");

            // Create DbContexts (connection strings configured in OnConfiguring or appsettings.json)
            using var competitorDb = DbContextHelper.CreateCompetitorContext("2025");
            using var eventDb = DbContextHelper.CreateEventContext("2025");

            DbContextHelper.Migrate(competitorDb);
            DbContextHelper.Migrate(eventDb);

            var eventCalendar = DataLoader.LoadCalendar(eventDb);
            var allRides = DataLoader.LoadHydratedRides(competitorDb, eventDb);
            
            // Orchestrate results generation
            var orchestrator = new ResultsOrchestrator(eventCalendar, allRides);
            orchestrator.GenerateAll();
            orchestrator.GenerateIndex();

            // Still emit preview.html as a homepage stub
            var previewPath = Path.Combine(outputDir, "preview.html");
            var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Club Site</title>
    <link rel=""stylesheet"" href=""./assets/csv/styles.css"">
</head>
<body>
    <h1>Welland Valley CC</h1>
    <p>Static site generation stub working!</p>
    <ul>
        <li>Competitor: Theo Marlin</li>
        <li>Event: TT01 (2025-05-01)</li>
    </ul>
</body>
</html>";
            File.WriteAllText(previewPath, html);

            Console.WriteLine($"[OK] Preview written to {previewPath}");
        }
    }
}
