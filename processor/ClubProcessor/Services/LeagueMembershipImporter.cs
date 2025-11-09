using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Csv;
using ClubProcessor.Models.Enums;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace ClubProcessor.Services
{
    public class LeagueMembershipImporter
    {
        private readonly CompetitorDbContext context;
        private readonly DateTime runtime;

        public LeagueMembershipImporter(CompetitorDbContext context, DateTime runtime)
        {
            this.context = context;
            this.runtime = runtime;
        }

        public (int updatedCount, int clearedCount) Import(string csvPath)
        {
            var leaguesFromCsv = ParseCsv(csvPath); // ClubNumber → League
            var updatedCount = ApplyLeagueAssignments(leaguesFromCsv);
            var clearedCount = RemoveObsoleteLeagueAssignments(leaguesFromCsv.Keys.ToHashSet());
            context.SaveChanges();
            return (updatedCount, clearedCount);
        }

        private Dictionary<int, League> ParseCsv(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                HeaderValidated = null,
                MissingFieldFound = null
            });

            ValidateHeaders(csv);

            var rows = csv.GetRecords<LeagueCsvRow>().ToList();

            ValidateNoDuplicateCompetitors(rows);

            return MapRowsToLeagueAssignments(rows);
        }

        private static void ValidateNoDuplicateCompetitors(IEnumerable<LeagueCsvRow> rows)
        {
            var duplicates = rows
                .GroupBy(r => (r.ClubNumber, r.ClubMemberName))
                .Where(g => g.Count() > 1)
                .ToList();

            if (duplicates.Any())
            {
                var dupList = string.Join(", ",
                    duplicates.Select(d => $"{d.Key.ClubNumber}-{d.Key.ClubMemberName}"));
                throw new FormatException(
                    $"Leagues CSV contains duplicate competitor rows: {dupList}");
            }
        }

        private void ValidateHeaders(CsvReader csv)
        {
            csv.Read();
            csv.ReadHeader();
            var headers = csv.Context.Reader?.HeaderRecord ?? Array.Empty<string>();

            var requiredHeaders = new[] { "ClubNumber", "ClubMemberName", "LeagueDivision" };
            foreach (var required in requiredHeaders)
            {
                if (!headers.Contains(required))
                {
                    throw new FormatException(
                        $"Leagues CSV is missing required column: {required}. " +
                        $"Expected columns: {string.Join(", ", requiredHeaders)}");
                }
            }
        }

        private Dictionary<int, League> MapRowsToLeagueAssignments(IEnumerable<LeagueCsvRow> rows)
        {
            var result = new Dictionary<int, League>();

            foreach (var row in rows)
            {
                if (result.ContainsKey(row.ClubNumber))
                {
                    throw new InvalidOperationException(
                        $"League assignment failed: duplicate entry in Leagues CSV for ClubNumber={row.ClubNumber}, Name={row.ClubMemberName}");
                }

                var competitor = ValidateCompetitor(row);
                result[row.ClubNumber] = row.League;
            }

            return result;
        }

        private IEnumerable<Competitor> ValidateCompetitor(LeagueCsvRow row)
        {
            var matches = context.Competitors
                .Where(c => c.ClubNumber == row.ClubNumber)
                .AsEnumerable()
                .Where(c => c.FullName == row.ClubMemberName)
                .ToList();

            if (!matches.Any())
            {
                throw new InvalidOperationException(
                    $"League assignment failed: no competitor found for ClubNumber={row.ClubNumber}, Name={row.ClubMemberName}");
            }

            return matches; // return all rows for this competitor
        }

        private int ApplyLeagueAssignments(Dictionary<int, League> leagues)
        {
            var count = 0;
            foreach (var kvp in leagues)
            {
                var competitors = context.Competitors
                    .Where(c => c.ClubNumber == kvp.Key)
                    .ToList();

                foreach (var competitor in competitors)
                {
                    competitor.League = kvp.Value;
                    competitor.LastUpdatedUtc = runtime;
                    count++;
                }
            }
            return count;
        }

        private int RemoveObsoleteLeagueAssignments(HashSet<int> validClubNumbers)
        {
            var count = 0;
            foreach (var competitor in context.Competitors)
            {
                if (!validClubNumbers.Contains(competitor.ClubNumber))
                {
                    competitor.League = League.Undefined;
                    competitor.LastUpdatedUtc = runtime;
                    count++;
                }
            }
            return count;
        }
    }
}
