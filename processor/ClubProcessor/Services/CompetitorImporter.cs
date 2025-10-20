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
            var lines = File.ReadAllLines(csvPath).Skip(1);

            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var importDate = parts.Count() == 10 ? DateTime.Parse(parts[9]) : runtime;

                var competitor = new Competitor
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
                };

                var existingRecords = context.Competitors
                    .Where(c => c.ClubNumber == competitor.ClubNumber)
                    .OrderByDescending(c => c.CreatedUtc)
                    .ToList();

                var latest = existingRecords.FirstOrDefault();

                if (latest == null)
                {
                    // First-ever import
                    context.Competitors.Add(competitor);
                }
                else if (latest.ClaimStatus != competitor.ClaimStatus)
                {
                    // Claim status changed — insert new record
                    context.Competitors.Add(competitor);
                }
                else
                {
                    // Same claim status — treat as correction
                    latest.Surname = competitor.Surname;
                    latest.GivenName = competitor.GivenName;
                    latest.IsFemale = competitor.IsFemale;
                    latest.IsJuvenile = competitor.IsJuvenile;
                    latest.IsJunior = competitor.IsJunior;
                    latest.IsSenior = competitor.IsSenior;
                    latest.IsVeteran = competitor.IsVeteran;
                    latest.LastUpdatedUtc = importDate;
                }
            }

            context.SaveChanges();
        }
    }
}
