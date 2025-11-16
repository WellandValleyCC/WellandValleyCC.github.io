using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.Tests.Helpers
{
    public static class CsvTestLoader
    {
        public static List<Competitor> LoadCompetitorsFromCsv(string csv)
        {
            var lines = csv.Split(Environment.NewLine)
                           .Skip(1) // skip header
                           .Where(l => !string.IsNullOrWhiteSpace(l));

            return lines.Select(static line =>
            {
                var parts = line.Split(',');

                return new Competitor
                {
                    ClubNumber = int.Parse(parts[0]),
                    Surname = parts[1],
                    GivenName = parts[2],
                    ClaimStatus = Enum.Parse<ClaimStatus>(parts[3], ignoreCase: true),
                    IsFemale = bool.Parse(parts[4]),
                    AgeGroup = Enum.Parse<AgeGroup>(parts[5], ignoreCase: true),
                    VetsBucket = string.IsNullOrWhiteSpace(parts[6]) ? null : int.Parse(parts[6])
                };
            }).ToList();
        }


        public static List<Ride> LoadRidesFromCsv(string csv, List<Competitor> competitors)
        {
            var lines = csv.Split(Environment.NewLine)
                           .Skip(1) // skip header
                           .Where(l => !string.IsNullOrWhiteSpace(l));

            return lines.Select(line =>
            {
                var parts = line.Split(',');

                // ClubNumber: empty -> guest (null), else parse integer
                int? clubNumber = string.IsNullOrWhiteSpace(parts[1])
                    ? (int?)null
                    : int.Parse(parts[1]);

                // Competitor resolution
                Competitor? competitor = null;
                if (clubNumber is int cn)
                {
                    competitor = competitors.FirstOrDefault(c => c.ClubNumber == cn)
                        ?? throw new InvalidOperationException($"No competitor found with ClubNumber {cn}");
                }

                // Eligibility
                var eligibility = Enum.Parse<RideEligibility>(parts[2], ignoreCase: true);

                // Optional numeric fields
                int? eventRank = string.IsNullOrWhiteSpace(parts[3]) ? null : int.Parse(parts[3]);
                int? eventRoadBikeRank = string.IsNullOrWhiteSpace(parts[4]) ? null : int.Parse(parts[4]);
                int totalSeconds = string.IsNullOrWhiteSpace(parts[5]) ? 0 : int.Parse(parts[5]);

                // Name column (required)
                var name = parts.Length > 6 ? parts[6].Trim() : string.Empty;
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException("Ride CSV must include a Name column");

                // For club members, enforce consistency with Competitor
                if (competitor != null)
                {
                    var expectedName = $"{competitor.GivenName} {competitor.Surname}";
                    if (!string.Equals(name, expectedName, StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"Ride name '{name}' does not match competitor '{expectedName}'");
                    }
                }

                return new Ride
                {
                    EventNumber = int.Parse(parts[0]),
                    ClubNumber = clubNumber,
                    Competitor = competitor,
                    Eligibility = eligibility,
                    EventRank = eventRank,
                    EventRoadBikeRank = eventRoadBikeRank,
                    TotalSeconds = totalSeconds,
                    Name = name
                };
            }).ToList();
        }
    }
}
