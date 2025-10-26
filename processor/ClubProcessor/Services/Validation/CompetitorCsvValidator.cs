using ClubProcessor.Models;
using ClubProcessor.Models.Csv.ClubProcessor.Models.Csv;
using ClubProcessor.Models.Enums;
using System.Globalization;

public class CompetitorCsvValidator
{
    public IEnumerable<string> Validate(IEnumerable<CompetitorCsvRow> rows)
    {
        var issues = new List<string>();
        var seenClubNumbers = new HashSet<int>();
        var lineNumber = 1;

        foreach (var row in rows)
        {
            lineNumber++;

            // ClubNumber: must be unique
            var clubNumber = row.ClubNumber;
            if (!seenClubNumbers.Add(clubNumber))
            {
                issues.Add($"Line {lineNumber}: Duplicate ClubNumber '{clubNumber}'.");
            }

            // Surname and GivenName: required
            if (string.IsNullOrWhiteSpace(row.Surname))
                issues.Add($"Line {lineNumber}: Surname is missing.");

            if (string.IsNullOrWhiteSpace(row.GivenName))
                issues.Add($"Line {lineNumber}: GivenName is missing.");

            // ClaimStatus: must match known values (case-insensitive, allow embedded spaces)
            if (row.ClaimStatus == ClaimStatus.Unknown)
            {
                issues.Add($"Line {lineNumber}: ClaimStatus is 'Unknown' and must be explicitly set.");
            }

            // Age category: exactly one of Juvenile, Junior, Senior, Veteran must be true
            var ageCategories = new[]
            {
                row.IsJuvenile,
                row.IsJunior,
                row.IsSenior,
                row.IsVeteran
            };

            var trueCount = ageCategories.Count(b => b);

            if (trueCount != 1)
            {
                issues.Add($"Line {lineNumber}: Exactly one age category must be true (Juvenile, Junior, Senior, Veteran). Found {trueCount}.");
            }

            // ImportDate: must be YYYY-MM-DD
            if (row.ImportDate == default)
            {
                issues.Add($"Line {lineNumber}: ImportDate is missing or invalid.");
            }
        }

        return issues;
    }
}
