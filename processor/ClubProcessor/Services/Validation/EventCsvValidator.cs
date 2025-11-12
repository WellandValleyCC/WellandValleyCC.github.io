using ClubCore.Models;
using ClubCore.Models.Csv;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClubProcessor.Services.Validation
{
    public class EventCsvValidator
    {
        private readonly ILogger<EventCsvValidator> _logger;

        public EventCsvValidator(ILogger<EventCsvValidator> logger)
        {
            _logger = logger;
        }

        public IEnumerable<RideCsvRow> Validate(IEnumerable<RideCsvRow> rows, IEnumerable<Competitor> competitors)
        {
            var validRows = new List<RideCsvRow>();

            foreach (var (row, index) in rows.Select((r, i) => (r, i + 2)))
            {
                var issues = ValidateRow(row, index, competitors);
                if (issues.Any())
                {
                    foreach (var issue in issues)
                        _logger.LogWarning("CSV validation failed at line {Line}: {Issue}", index, issue);
                    continue;
                }

                validRows.Add(row);
            }

            return validRows;
        }

        private List<string> ValidateRow(RideCsvRow row, int lineNumber, IEnumerable<Competitor> competitors)
        {
            var issues = new List<string>();

            if (string.IsNullOrWhiteSpace(row.NumberOrName))
                issues.Add("Number/Name is blank");

            if (!int.TryParse(row.Hours, out _))
                issues.Add("H is not a valid integer");

            if (!int.TryParse(row.Minutes, out var m) || m < 0 || m > 59)
                issues.Add("M is not in range 0–59");

            if (!double.TryParse(row.Seconds, out _))
                issues.Add("S is not a valid number");

            if (!string.IsNullOrEmpty(row.RoadBike) && !Regex.IsMatch(row.RoadBike, "^[rR]$"))
                issues.Add("Roadbike? must be empty or 'r'/'R'");

            if (!string.IsNullOrEmpty(row.EligibilityRaw) &&
                !Regex.IsMatch(row.EligibilityRaw, "^(DNS|DNF|DQ)$", RegexOptions.IgnoreCase))
                issues.Add("DNS/DNF/DQ must be empty or one of DNS/DNF/DQ");

            if (string.IsNullOrWhiteSpace(row.Name))
                issues.Add("Name is blank");

            // Name matching logic
            if (!string.IsNullOrWhiteSpace(row.Name))
            {
                if (int.TryParse(row.NumberOrName, out var clubNumber))
                {
                    var competitor = competitors.FirstOrDefault(c => c.ClubNumber == clubNumber);
                    if (competitor == null || !competitor.MatchesName(row.Name))
                        issues.Add($"Name '{row.Name}' does not match ClubNumber {clubNumber}");
                }
                else
                {
                    var normalizedName = row.Name.Trim().ToLowerInvariant().Replace(" ", "");
                    var normalizedFallback = row.NumberOrName?.Trim().ToLowerInvariant().Replace(" ", "");

                    if (normalizedName != normalizedFallback)
                        issues.Add($"Name '{row.Name}' does not match string Number/Name '{row.NumberOrName}'");
                }
            }

            return issues;
        }
    }
}
