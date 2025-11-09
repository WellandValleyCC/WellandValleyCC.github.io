using ClubProcessor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ClubProcessor.Models
{
    [DebuggerDisplay("#{ClubNumber} {FullName} ({ClaimStatus}, {Gender}, {AgeGroup}), {CreatedUtcShort}")]
    public class Competitor
    {
         [Key]
        public int Id { get; set; }  // Auto-incremented by EF/SQLite

        public required int ClubNumber { get; set; }       // e.g. 9999
        public required string Surname { get; set; }          // e.g. "Doe"
        public required string GivenName { get; set; }        // e.g. "John"
        public required ClaimStatus ClaimStatus { get; set; }      // e.g. FirstClaim
        public required bool IsFemale { get; set; }           // e.g. false
 
        [NotMapped]
        public string Gender => IsFemale ? "Female" : "Male";

        private int? _vetsBucket;

        /// <summary>
        /// Vets bucket persisted by EF. Setter enforces that only veterans may have a non-null bucket.
        /// </summary>
        public int? VetsBucket
        {
            get => _vetsBucket;
            set
            {
                if (value.HasValue && !IsVeteran)
                    throw new ArgumentException("VetsBucket may only be set for veteran competitors", nameof(VetsBucket));

                if (IsVeteran && !value.HasValue)
                    throw new ArgumentException("Veteran competitors must have a VetsBucket", nameof(VetsBucket));

                _vetsBucket = value;
            }
        }

        [NotMapped]
        public bool IsJuvenile => AgeGroup == AgeGroup.Juvenile;
        [NotMapped]
        public bool IsJunior => AgeGroup == AgeGroup.Junior;
        [NotMapped]
        public bool IsSenior => AgeGroup == AgeGroup.Senior;
        [NotMapped]
        public bool IsVeteran => AgeGroup == AgeGroup.Veteran;

        public required AgeGroup AgeGroup { get; set; }

        /// <summary>
        /// The date that this Competitor record was created, in UTC.
        /// </summary>
        /// <remarks>
        /// If the competitor gets updated later , this value should remain unchanged.
        /// E.g. a name spelling correction should not modify CreatedUtc.
        /// If ClaimStatus changes then a new Competitor record should be created instead.
        /// </remarks>
        public DateTime CreatedUtc { get; set; }

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string CreatedUtcShort => CreatedUtc.ToString("dd/MM/yyyy");


        /// <summary>
        /// The date that this Competitor record was last modified, in UTC.
        /// </summary>
        /// <remarks>
        /// If the competitor gets updated later , this value indicates when that happened.
        /// E.g. a name spelling correction should not modify CreatedUtc.
        /// </remarks>
        public DateTime LastUpdatedUtc { get; set; }

        public League League { get; set; }

        [NotMapped]
        public string FullName =>
            string.Join(" ", new[] { GivenName, Surname }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim()));


        public bool MatchesName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            var normalizedInput = Normalize(name);
            var normalizedFullName = Normalize(FullName);

            return normalizedInput == normalizedFullName;
        }

        private static string Normalize(string input)
        {
            return input.Trim().ToLowerInvariant().Replace(" ", "");
        }


        /// <summary>
        /// Call to re-check full object invariants (useful for tests / factory).
        /// </summary>
        public void Validate()
        {
            if (ClaimStatus == ClaimStatus.Unknown)
                throw new InvalidOperationException("ClaimStatus must be explicitly set.");

            if (_vetsBucket.HasValue && !IsVeteran)
                throw new InvalidOperationException("VetsBucket may only be set for veteran competitors.");
            if (IsVeteran && !_vetsBucket.HasValue)
                throw new InvalidOperationException("Veteran competitors must have a VetsBucket.");
        }
    }
}
