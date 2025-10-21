using ClubProcessor.Models;
using ClubProcessor.Context;

namespace ClubProcessor.Services
{
    public class CompetitorImporter
    {
        private readonly ClubDbContext context;
        private readonly DateTime runtime;

        public CompetitorImporter(ClubDbContext context, DateTime runtime)
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
            var lines = File.ReadAllLines(csvPath).Skip(1);
            var competitors = new List<Competitor>();

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var importDate = parts.Length == 10
                    ? DateTime.Parse(parts[9])
                    : runtime;

                competitors.Add(new Competitor
                {
                    ClubNumber = parts[0],
                    Surname = parts[1],
                    GivenName = parts[2],
                    ClaimStatus = parts[3],
                    IsFemale = bool.Parse(parts[4]),
                    IsJuvenile = bool.Parse(parts[5]),
                    IsJunior = bool.Parse(parts[6]),
                    IsSenior = bool.Parse(parts[7]),
                    IsVeteran = bool.Parse(parts[8]),
                    CreatedUtc = importDate,
                    LastUpdatedUtc = importDate
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

        private void RemoveObsoleteCompetitors(HashSet<string> validClubNumbers)
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
