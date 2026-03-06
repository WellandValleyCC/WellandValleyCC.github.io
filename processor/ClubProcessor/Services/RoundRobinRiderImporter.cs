using ClubCore.Context;
using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubProcessor.Services.Validation;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace ClubProcessor.Services
{
    public class RoundRobinRiderImporter
    {
        private readonly EventDbContext context;

        public RoundRobinRiderImporter(EventDbContext context)
        {
            this.context = context;
        }

        public void Import(string csvPath)
        {
            context.RoundRobinRiders.ExecuteDelete();

            var roundRobinRidersFromCsv = ParseCsv(csvPath);

            context.RoundRobinRiders.AddRange(roundRobinRidersFromCsv);

            context.SaveChanges();
        }

        private List<RoundRobinRider> ParseCsv(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rows = csv.GetRecords<RoundRobinRiderCsvRow>().ToList();

            var validation = RoundRobinRiderCsvValidator.Validate(rows);
            if (validation.Any())
            {
                var issues = string.Join(Environment.NewLine, validation);
                throw new InvalidDataException($"RoundRobinRiders CSV validation failed with the following issues:{Environment.NewLine}{issues}");
            }

            var validClubs = context.RoundRobinClubs
                .Select(c => c.ShortName)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
         
            var riders = new List<RoundRobinRider>();

            foreach (var row in rows)
            {
                // Skip riders whose club is not in the RR club list
                if (!validClubs.Contains(row.Club))
                {
                    if (!string.IsNullOrWhiteSpace(row.Club))
                    {
                        Console.WriteLine($"[INFO] Skipping ride {row.Name}: Club '{row.Club}' is not a valid RoundRobinClub.");
                    }
                    continue;
                }

                riders.Add(new RoundRobinRider
                {
                    Name = row.Name,
                    RoundRobinClub = row.Club,
                    // Readonly field DecoratedName = row.DecoratedName,
                    IsFemale = IsYes(row.IsFemale),
                });
            }

            return riders;
        }

        private static bool IsYes(string? raw) =>
            !string.IsNullOrWhiteSpace(raw) &&
            new[] { "Y", "YES", "TRUE" }.Contains(raw.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
