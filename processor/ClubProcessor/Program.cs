using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

class Program
{
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
        Console.WriteLine($"[INFO] Starting events ingestion for: {inputPath}");

        var eventDbPath = Path.Combine("data", $"club_events_{year}.db");
        var competitorDbPath = Path.Combine("data", $"club_competitors_{year}.db");

        var eventOptions = new DbContextOptionsBuilder<EventDbContext>()
            .UseSqlite($"Data Source={eventDbPath}")
            .Options;

        var competitorOptions = new DbContextOptionsBuilder<CompetitorDbContext>()
            .UseSqlite($"Data Source={competitorDbPath}")
            .Options;

        using var eventContext = new EventDbContext(eventOptions);
        using var competitorContext = new CompetitorDbContext(competitorOptions);

        eventContext.Database.Migrate();
        Console.WriteLine($"[INFO] Migration complete for: {eventDbPath}");

        competitorContext.Database.Migrate();
        Console.WriteLine($"[INFO] Migration complete for: {competitorDbPath}");

        var calendarCsvPath = Path.Combine(inputPath, $"Calendar_{year}.csv");
        if (File.Exists(calendarCsvPath))
        {
            Console.WriteLine($"[INFO] Importing calendar from: {calendarCsvPath}");
            var calendarImporter = new CalendarImporter(eventContext);
            calendarImporter.ImportFromCsv(calendarCsvPath);
            Console.WriteLine("[OK] Calendar import complete");
        }
        else
        {
            Console.WriteLine($"[WARN] Calendar CSV not found: {calendarCsvPath}");
        }

        var processor = new EventsImporter(eventContext, competitorContext);
        processor.ImportFromFolder(inputPath);
    }
}
