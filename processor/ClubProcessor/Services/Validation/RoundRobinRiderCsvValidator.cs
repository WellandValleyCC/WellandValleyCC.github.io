using ClubCore.Models.Csv;

namespace ClubProcessor.Services.Validation
{
    public static class RoundRobinRiderCsvValidator
    {
        public static IEnumerable<string> Validate(IEnumerable<RoundRobinRiderCsvRow> rows)
        {
            var issues = new List<string>();
            var lineNumber = 1;

            foreach (var row in rows)
            {
                lineNumber++;

                if (IsBlankRow(row))
                    continue;

                issues.AddRange(ValidateRow(row, lineNumber));
            }

            return issues;
        }

        private static IEnumerable<string> ValidateRow(RoundRobinRiderCsvRow row, int line)
        {
            if (string.IsNullOrWhiteSpace(row.Name))
                yield return Issue(line, "Name is missing.");

            // Club is not mandatory - this helps when new riders sign up and we don't yet know their club affiliation.
            //if (string.IsNullOrWhiteSpace(row.Club))
            //    yield return Issue(line, "Club is missing.");

            if (string.IsNullOrWhiteSpace(row.DecoratedName))
                yield return Issue(line, "DecoratedName is missing.");
            else if (row.DecoratedName != $"{row.Name} ({row.Club})")
                yield return Issue(line, "DecoratedName should be \"Name (Club)\".");

            if (!IsValidIsFemale(row.IsFemale))
                yield return Issue(line, "IsFemale should be blank or \"Y\".");
        }

        private static bool IsValidIsFemale(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            return value.Trim().ToUpper() == "Y";
        }

        private static string Issue(int line, string message)
            => $"Line {line}: {message}";

        private static bool IsBlankRow(RoundRobinRiderCsvRow row)
        {
            return string.IsNullOrWhiteSpace(row.Name)
                && (string.IsNullOrWhiteSpace(row.DecoratedName) || row.DecoratedName == " ()")
                && string.IsNullOrWhiteSpace(row.IsFemale);
        }
    }
}