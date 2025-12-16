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
            using var competitorDb = DbContextHelper.CreateReadonlyCompetitorDbContext(year);
            using var eventDb = DbContextHelper.CreateReadonlyEventDbContext(year);

            var eventCalendar = DataLoader.LoadCalendar(eventDb);
            var allRides = DataLoader.LoadRides(eventDb);
            var allCompetitors = DataLoader.LoadCompetitors(competitorDb);
            DataLoader.AttachReferencesToRides(allRides, allCompetitors, eventCalendar);

            // Orchestrate results generation
            var orchestrator = new ResultsOrchestrator(allRides, allCompetitors, eventCalendar);
            var indexFileName = $"index{year}";
            orchestrator.GenerateAll(indexFileName);
            orchestrator.GenerateIndex(indexFileName);
        }
    }
}
