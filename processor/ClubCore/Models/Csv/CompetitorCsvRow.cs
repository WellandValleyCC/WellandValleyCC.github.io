using ClubCore.Models.Enums;
using CsvHelper.Configuration.Attributes;

namespace ClubCore.Models.Csv
{
    public class CompetitorCsvRow
    {
        [Name("ClubNumber")]
        public int ClubNumber { get; set; }

        [Name("Surname")]
        public required string Surname { get; set; }

        [Name("GivenName")]
        public required string GivenName { get; set; }

        [Name("ClaimStatus")]
        public required string ClaimStatusRaw { get; set; }

        [Name("isFemale")]
        public bool IsFemale { get; set; }

        [Name("AgeGroup")]
        public required string AgeGroupRaw { get; set; }

        [Name("ImportDate")]
        public DateTime ImportDate { get; set; }

        [Name("VetsBucket")]
        public int? VetsBucket { get; set; }

        [Ignore]
        public string FullName => $"{GivenName} {Surname}".Trim();

        [Ignore]
        public ClaimStatus ClaimStatus =>
            ClaimStatusRaw?
                .Trim()
                .ToLowerInvariant()
                .Replace(" ", "") switch
            {
                "firstclaim" => ClaimStatus.FirstClaim,
                "secondclaim" => ClaimStatus.SecondClaim,
                "honorary" => ClaimStatus.Honorary,
                _ => ClaimStatus.Unknown
            };

        [Ignore]
        public AgeGroup AgeGroup =>
            AgeGroupRaw?
                .Trim()
                .ToLowerInvariant() switch
            {
                "juvenile" => AgeGroup.Juvenile,
                "junior" => AgeGroup.Junior,
                "senior" => AgeGroup.Senior,
                "veteran" => AgeGroup.Veteran,
                _ => AgeGroup.Undefined
            };
    }
}
