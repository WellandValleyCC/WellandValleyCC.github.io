using ClubCore.Context;
using ClubCore.Models;
using ClubCore.Models.Csv;
using CsvHelper;
using System.Globalization;

namespace ClubProcessor.Services
{
    public class CompetitorImporter
    {
        private readonly CompetitorDbContext context;
        private readonly DateTime runtime;

        public CompetitorImporter(CompetitorDbContext context, DateTime runtime)
        {
            this.context = context;
            this.runtime = runtime;
        }

        public void Import(string csvPath)
        {
            var competitorsFromCsv = ParseCsv(csvPath);
            ApplyImportLogic(competitorsFromCsv);
            RemoveObsoleteCompetitors(competitorsFromCsv.Select(c => c.ClubNumber).ToHashSet());
            context.SaveChanges();
        }

        private List<Competitor> ParseCsv(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rows = csv.GetRecords<CompetitorCsvRow>().ToList();

            var validation = CompetitorCsvValidator.Validate(rows);
            if (validation.Any())
            {
                var issues = string.Join(Environment.NewLine, validation);
                throw new InvalidDataException($"Competitor CSV validation failed with the following issues:{Environment.NewLine}{issues}");
            }

            var competitors = new List<Competitor>();

            foreach (var row in rows)
            {
                competitors.Add(new Competitor
                {
                    ClubNumber = row.ClubNumber,
                    Surname = row.Surname,
                    GivenName = row.GivenName,
                    ClaimStatus = row.ClaimStatus,
                    IsFemale = row.IsFemale,
                    AgeGroup = row.AgeGroup,
                    VetsBucket = row.VetsBucket,
                    CreatedUtc = row.ImportDate,
                    LastUpdatedUtc = row.ImportDate,
                });
            }

            return competitors;
        }

        private void ApplyImportLogic(List<Competitor> incoming)
        {
            foreach (var competitor in incoming)
            {
                var existingRecords = context.Competitors
                    .Where(c => c.ClubNumber == competitor.ClubNumber && c.CreatedUtc <= competitor.CreatedUtc)
                    .OrderByDescending(c => c.CreatedUtc)
                    .ToList();

                var latest = existingRecords.FirstOrDefault();

                if (latest == null || latest.ClaimStatus != competitor.ClaimStatus)
                {
                    context.Competitors.Add(competitor);
                }
                else
                {
                    latest.Surname = competitor.Surname;
                    latest.GivenName = competitor.GivenName;
                    latest.IsFemale = competitor.IsFemale;

                    latest.AgeGroup = competitor.AgeGroup;
                    latest.VetsBucket = competitor.VetsBucket;

                    latest.LastUpdatedUtc = competitor.LastUpdatedUtc;
                }
            }
        }

        private void RemoveObsoleteCompetitors(HashSet<int> validClubNumbers)
        {
            var obsolete = context.Competitors
                .Where(c => !validClubNumbers.Contains(c.ClubNumber))
                .ToList();

            if (obsolete.Any())
            {
                context.Competitors.RemoveRange(obsolete);
            }
        }
    }
}
