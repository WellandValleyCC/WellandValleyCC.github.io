using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Csv.ClubProcessor.Models.Csv;
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
                    IsJuvenile = row.IsJuvenile,
                    IsJunior = row.IsJunior,
                    IsSenior = row.IsSenior,
                    IsVeteran = row.IsVeteran,
                    CreatedUtc = row.ImportDate,
                    LastUpdatedUtc = row.ImportDate,
                    VetsBucket = row.VetsBucket,
                });
            }

            return competitors;
        }

        private void ApplyImportLogic(List<Competitor> incoming)
        {
            foreach (var competitor in incoming)
            {
                var existingRecords = context.Competitors
                    .Where(c => c.ClubNumber == competitor.ClubNumber)
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
                    latest.IsJuvenile = competitor.IsJuvenile;
                    latest.IsJunior = competitor.IsJunior;
                    latest.IsSenior = competitor.IsSenior;
                    latest.IsVeteran = competitor.IsVeteran;
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
