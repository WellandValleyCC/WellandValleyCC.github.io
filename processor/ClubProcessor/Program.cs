using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        // Parse CLI arguments
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

        // Infer year from filename or folder name
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

        Directory.CreateDirectory("data");

        switch (mode.ToLower())
        {
            case "competitors":
                {
                    var dbPath = Path.Combine("data", $"club_competitors_{year}.db");
                    var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                        .UseSqlite($"Data Source={dbPath}")
                        .Options;

                    using var context = new CompetitorDbContext(options);
                    context.Database.Migrate();

                    var importer = new CompetitorImporter(context, DateTime.UtcNow);
                    importer.Import(inputPath);
                    break;
                }

            case "events":
                {
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
                    competitorContext.Database.Migrate();

                    var processor = new EventProcessor(eventContext, competitorContext);
                    processor.ProcessFolder(inputPath);
                    break;
                }

            default:
                Console.WriteLine("Unsupported mode. Use 'competitors' or 'events'.");
                break;
        }
    }
}
