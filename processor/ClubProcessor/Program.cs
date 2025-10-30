using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using ClubProcessor.Interfaces;
using ClubProcessor.Orchestration;
using ClubProcessor.Calculators;

class Program
{
    static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register all calculators
        services.AddScoped<ICompetitionScoreCalculator, JuvenilesScoreCalculator>();

        /*
        services.AddScoped<ICompetitionScoreCalculator, JuniorsScoreCalculator>();
        services.AddScoped<ICompetitionScoreCalculator, SeniorsScoreCalculator>();
        services.AddScoped<ICompetitionScoreCalculator, WomenScoreCalculator>();
        services.AddScoped<ICompetitionScoreCalculator, RoadBikeMenScoreCalculator>();
        services.AddScoped<ICompetitionScoreCalculator, RoadBikeWomenScoreCalculator>();
        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new VeteransScoreCalculator(VeteransScoringMode.StandardTimes));

        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new LeagueScoreCalculator(LeagueLevel.Prem));
        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new LeagueScoreCalculator(LeagueLevel.League1));
        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new LeagueScoreCalculator(LeagueLevel.League2));
        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new LeagueScoreCalculator(LeagueLevel.League3));
        services.AddScoped<ICompetitionScoreCalculator>(sp =>
            new LeagueScoreCalculator(LeagueLevel.League4));

        services.AddScoped<ICompetitionScoreCalculator, NevBrooksScoreCalculator>();

        */

        // Register orchestrators
        services.AddScoped<EventImportCoordinator>();
        services.AddScoped<CompetitionPointsCalculator>();

        return services.BuildServiceProvider();
    }

    static void Main(string[] args)
    {
        string? mode = null;
        string? inputPath = null;

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode") mode = args[i + 1];
            if (args[i] == "--file") inputPath = args[i + 1];
        }

        if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(inputPath))
        {
            Console.WriteLine("Usage: ClubProcessor.exe --mode <competitors|events> --file <path>");
            return;
        }

        string year = InferYear(mode, inputPath);
        Directory.CreateDirectory("data");

        switch (mode.ToLower())
        {
            case "competitors":
                ImportCompetitors(inputPath, year);
                break;

            case "events":
                ImportEvents(inputPath, year);
                break;

            default:
                Console.WriteLine("Unsupported mode. Use 'competitors' or 'events'.");
                break;
        }
    }

    static string InferYear(string mode, string inputPath)
    {
        string year = DateTime.UtcNow.Year.ToString();

        if (mode == "competitors" && File.Exists(inputPath))
        {
            var filename = Path.GetFileNameWithoutExtension(inputPath);
            var match = Regex.Match(filename, @"\d{4}");
            if (match.Success) year = match.Value;
        }
        else if (mode == "events" && Directory.Exists(inputPath))
        {
            var folderName = new DirectoryInfo(inputPath).Name;
            var match = Regex.Match(folderName, @"\d{4}");
            if (match.Success) year = match.Value;
        }

        return year;
    }

    static void ImportCompetitors(string inputPath, string year)
    {
        var dbPath = Path.Combine("data", $"club_competitors_{year}.db");
        var options = new DbContextOptionsBuilder<CompetitorDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        using var context = new CompetitorDbContext(options);

        context.Database.Migrate();
        Console.WriteLine($"[INFO] Migration complete for: {dbPath}");

        var importer = new CompetitorImporter(context, DateTime.UtcNow);
        importer.Import(inputPath);
    }

    static void ImportEvents(string inputPath, string year)
    {
        var provider = ConfigureServices();
        var scorer = provider.GetRequiredService<CompetitionPointsCalculator>();
        var coordinator = provider.GetRequiredService<EventImportCoordinator>();
        coordinator.Run(inputPath, year);
    }
}
