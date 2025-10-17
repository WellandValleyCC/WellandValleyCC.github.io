using ClubProcessor.Services;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ClubProcessor.exe --mode <competitors|events> --file <path>");
            return;
        }

        var mode = args[0].Replace("--mode", "").Trim();
        var filePath = args[1].Replace("--file", "").Trim();

        switch (mode.ToLower())
        {
            case "competitors":
                CompetitorImporter.Import(filePath);
                break;
            case "events":
                Console.WriteLine("Event processing not yet implemented.");
                break;
            default:
                Console.WriteLine("Unsupported mode. Use 'competitors' or 'events'.");
                break;
        }
    }
}
