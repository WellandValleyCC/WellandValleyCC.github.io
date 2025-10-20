using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

class Program
{
    static void Main(string[] args)
    {
        string? mode = null;
        string? filePath = null;


        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == "--mode") mode = args[i + 1];
            if (args[i] == "--file") filePath = args[i + 1];
        }

        if (string.IsNullOrEmpty(mode) || string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Usage: ClubProcessor.exe --mode <competitors|events> --file <path>");
            return;
        }

        // Extract year from filename
        var filename = Path.GetFileNameWithoutExtension(filePath); // e.g. "competitors_2026"
        var yearMatch = Regex.Match(filename, @"\d{4}");
        var year = yearMatch.Success ? yearMatch.Value : DateTime.UtcNow.Year.ToString();


        // Construct DB path
        var dbPath = $"data/club_competitors_{year}.db";
        Directory.CreateDirectory("data");

        var options = new DbContextOptionsBuilder<ClubDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        using var context = new ClubDbContext(options);
        context.Database.EnsureCreated();

        switch (mode.ToLower())
        {
            case "competitors":
                var importer = new CompetitorImporter(context, DateTime.UtcNow);
                importer.Import(filePath);
                break;
            case "events":
                Console.WriteLine($"[Stub] Would process event workbook: {filePath}");
                break;
            default:
                Console.WriteLine("Unsupported mode. Use 'competitors' or 'events'.");
                break;
        }
    }
}
