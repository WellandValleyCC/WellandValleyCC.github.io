using ClubSiteGenerator.Services;

namespace ClubSiteGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ClubSiteGenerator starting…");

            // Decide output folder (repo-rooted TestOutput or CI temp)
            var outputDir = OutputLocator.GetOutputDirectory();
            Console.WriteLine($"Writing site to: {outputDir}");

            var htmlPath = Path.Combine(outputDir, "preview.html");

            // Minimal HTML scaffold
            var html = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>Club Site</title>
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

            File.WriteAllText(htmlPath, html);

            Console.WriteLine($"[OK] HTML written to {htmlPath}");
        }
    }
}
