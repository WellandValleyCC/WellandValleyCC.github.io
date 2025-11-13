namespace ClubSiteGenerator.Utilities
{
    public static class StylesWriter
    {
        private const string CssContent = @"/* results table styling */ 
table.results {
  width: 100%;
  border-collapse: collapse;
  margin: 1em 0;
  font-family: system-ui, sans-serif;
  font-size: 0.9rem;
}
table.results th, table.results td {
  border: 1px solid #ddd;
  padding: 0.5em 0.75em;
  text-align: center;
}
table.results th {
  background-color: #f4f4f4;
  font-weight: bold;
}
table.results tbody tr:hover { background-color: #f1f7ff; }
table.results td:first-child { text-align: left; font-weight: 500; }

/* claim status colouring */
table.results tr.claim-first   { background-color: #e0ffe0; } /* light green */
table.results tr.claim-second  { background-color: #e0f0ff; } /* light blue */
table.results tr.guest         { background-color: #fff0e0; } /* light orange */
";

        public static void EnsureStylesheet(string outputDir)
        {
            // Drop into SiteOutput/assets/csv/
            var assetsDir = Path.Combine(outputDir, "assets");
            Directory.CreateDirectory(assetsDir);

            var cssPath = Path.Combine(assetsDir, "styles.css");

            File.WriteAllText(cssPath, CssContent);
        }
    }
}
