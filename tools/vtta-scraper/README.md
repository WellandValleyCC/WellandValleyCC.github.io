# VTTA Standards Scraper

This tool automates the extraction of VTTA age-standard times from [vtta.org.uk](https://www.vtta.org.uk/standards) using Playwright. It supports scraping multiple custom distances and outputs a unified CSV table keyed by age.

## 📦 Output Format

The combined CSV file (`vtta-standards-combined.csv`) has the following structure:

```
age,m5,m8.5,m9,m10,f5,f8.5,f9,f10
50,0:00:18,0:01:23,...
```


- `Age` = age (starting from age 50)
- `mX` = Male/Open standard time for distance X
- `fX` = Female standard time for distance X

## 🚀 Usage

### Prerequisites

Install Playwright CLI and dependencies:

```bash
dotnet add package Microsoft.Playwright
dotnet tool install --global Microsoft.Playwright.CLI
playwright install
```

### Run the Scraper

```
dotnet run --project vtta-scraper
```

This will:

- Scrape VTTA standards for configured distances (e.g. "5", "8.5", "9", "10")

- Combine results into a single CSV

- Log output path using [INFO]

## Project structure

```
vtta-scraper/
├── Program.cs              # CLI entry point
├── Scraper.cs              # Playwright scraping logic
├── VttaRow.cs              # Data model for scraped rows
├── VttaAggregator.cs       # Combines results across distances
└── CsvWriter.cs            # Outputs final CSV
```

## Configuration

To change distances, edit the distances array in Program.cs:

```
var distances = new[] { "5", "8.5", "9", "10" };
```

## Status

[x] Scrapes VTTA standards reliably

[x] Supports multiple distances

[x] Outputs unified CSV with sorted columns

[x] Logs full output path

## License
MIT — feel free to adapt for club use or personal tooling.

Built for Welland Valley CC by volunteers who love reproducible infrastructure and clean data.