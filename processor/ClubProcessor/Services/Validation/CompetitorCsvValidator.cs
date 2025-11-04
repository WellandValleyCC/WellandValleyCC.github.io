using ClubProcessor.Models.Csv.ClubProcessor.Models.Csv;
using ClubProcessor.Models.Enums;

static public class CompetitorCsvValidator
{
    static public IEnumerable<string> Validate(IEnumerable<CompetitorCsvRow> rows)
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

            // AgeGroup: must match known values Juvenile, Junior, Senior, Veteran (case-insensitive)
            if (row.AgeGroup == AgeGroup.Undefined)
            {
                issues.Add($"Line {lineNumber}: AgeGroup is 'Unknown' and must be explicitly set.");
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
