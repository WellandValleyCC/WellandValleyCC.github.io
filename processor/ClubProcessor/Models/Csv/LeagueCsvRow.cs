using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;
using CsvHelper.Configuration.Attributes;

namespace ClubProcessor.Models.Csv
{
    public class LeagueCsvRow
    {
        [Name("ClubNumber")]
        public int ClubNumber { get; set; }

        [Name("ClubMemberName")]
        public required string ClubMemberName { get; set; }

        [Name("LeagueDivision")]
        public required string LeagueRaw { get; set; }

        [Ignore]
        public League League => LeagueExtensions.ParseFromCsv(LeagueRaw);
    }
}

