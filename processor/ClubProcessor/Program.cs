using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        string mode = null;
        string filePath = null;

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

        var options = new DbContextOptionsBuilder<ClubDbContext>()
            .UseSqlite("Data Source=data/results.db")
            .Options;

        using var context = new ClubDbContext(options);

        switch (mode.ToLower())
        {
            case "competitors":
                var importer = new CompetitorImporter(context);
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
