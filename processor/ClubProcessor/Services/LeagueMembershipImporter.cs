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

        public void Import(string csvPath)
        {
            var leaguesFromCsv = ParseCsv(csvPath); // ClubNumber → League
            ApplyLeagueAssignments(leaguesFromCsv);
            RemoveObsoleteLeagueAssignments(leaguesFromCsv.Keys.ToHashSet());
            context.SaveChanges();
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

            return MapRowsToLeagueAssignments(rows);
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

        private Competitor ValidateCompetitor(LeagueCsvRow row)
        {
            var matches = context.Competitors
                .Where(c => c.ClubNumber == row.ClubNumber)
                .AsEnumerable() // force client-side evaluation because FullName is not a database field
                .Where(c => c.FullName == row.ClubMemberName)
                .ToList();

            if (matches.Count == 0)
            {
                throw new InvalidOperationException(
                    $"League assignment failed: no competitor found for ClubNumber={row.ClubNumber}, Name={row.ClubMemberName}");
            }
            if (matches.Count > 1)
            {
                throw new InvalidOperationException(
                    $"League assignment failed: multiple competitors found for ClubNumber={row.ClubNumber}, Name={row.ClubMemberName}");
            }

            return matches.Single();
        }

        private void ApplyLeagueAssignments(Dictionary<int, League> leagues)
        {
            foreach (var kvp in leagues)
            {
                var competitor = context.Competitors.SingleOrDefault(c => c.ClubNumber == kvp.Key);
                if (competitor != null)
                {
                    competitor.League = kvp.Value;
                    competitor.LastUpdatedUtc = runtime;
                }
            }
        }

        private void RemoveObsoleteLeagueAssignments(HashSet<int> validClubNumbers)
        {
            foreach (var competitor in context.Competitors)
            {
                if (!validClubNumbers.Contains(competitor.ClubNumber))
                {
                    competitor.League = League.Undefined;
                    competitor.LastUpdatedUtc = runtime;
                }
            }
        }
    }
}
