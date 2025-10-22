namespace ClubProcessor.Models.Csv
{
    using CsvHelper.Configuration.Attributes;
    using global::ClubProcessor.Models.Enums;

    namespace ClubProcessor.Models.Csv
    {
        internal class CompetitorCsvRow
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

            [Name("isJuvenile")]
            public bool IsJuvenile { get; set; }

            [Name("isJunior")]
            public bool IsJunior { get; set; }

            [Name("isSenior")]
            public bool IsSenior { get; set; }

            [Name("isVeteran")]
            public bool IsVeteran { get; set; }

            [Name("ImportDate")]
            public DateTime ImportDate { get; set; }

            [Ignore]
            public string FullName => $"{GivenName} {Surname}".Trim();

            [Ignore]
            public ClaimStatus ClaimStatus =>
                ClaimStatusRaw?.Trim().ToLowerInvariant() switch
                {
                    "first claim" => ClaimStatus.FirstClaim,
                    "second claim" => ClaimStatus.SecondClaim,
                    "honorary" => ClaimStatus.Honorary,
                    _ => ClaimStatus.Unknown
                };
        }
    }
}
