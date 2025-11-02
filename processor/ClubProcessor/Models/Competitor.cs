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

        // Backing fields
        private bool _isJuvenile;
        private bool _isJunior;
        private bool _isSenior;
        private bool _isVeteran;
        private int? _vetsBucket;

        /// <summary>
        /// Age flags persisted by EF (one-hot enforced by setters)
        /// </summary>
        public required bool IsJuvenile
        {
            get => _isJuvenile;
            set
            {
                if (value && (_isJunior || _isSenior || _isVeteran))
                    throw new ArgumentException("Only one age group flag may be true at a time", nameof(IsJuvenile));
                _isJuvenile = value;
            }
        }
        public required bool IsJunior
        {
            get => _isJunior;
            set
            {
                if (value && (_isJuvenile || _isSenior || _isVeteran))
                    throw new ArgumentException("Only one age group flag may be true at a time", nameof(IsJunior));
                _isJunior = value;
            }
        }
        public required bool IsSenior
        {
            get => _isSenior;
            set
            {
                if (value && (_isJuvenile || _isJunior || _isVeteran))
                    throw new ArgumentException("Only one age group flag may be true at a time", nameof(IsSenior));
                _isSenior = value;
            }
        }
        public required bool IsVeteran
        {
            get => _isVeteran;
            set
            {
                if (value && (_isJuvenile || _isJunior || _isSenior))
                    throw new ArgumentException("Only one age group flag may be true at a time", nameof(IsVeteran));
                // If demoting from veteran to non-veteran, ensure VetsBucket is null first (fail-fast)
                if (!value && _vetsBucket.HasValue)
                    throw new InvalidOperationException("Cannot unset IsVeteran while VetsBucket is set. Clear VetsBucket first.");
                _isVeteran = value;
            }
        }

        /// <summary>
        /// Computed AgeGroup with setter to switch flags atomically
        /// </summary>
        [NotMapped]
        public AgeGroup AgeGroup
        {
            get
            {
                if (IsJuvenile) return AgeGroup.IsJuvenile;
                if (IsJunior) return AgeGroup.IsJunior;
                if (IsSenior) return AgeGroup.IsSenior;
                if (IsVeteran) return AgeGroup.IsVeteran;
                return AgeGroup.Undefined;
            }
            set
            {
                // clear all then set the requested one
                _isJuvenile = false;
                _isJunior = false;
                _isSenior = false;
                _isVeteran = false;

                switch (value)
                {
                    case AgeGroup.IsJuvenile:
                        _isJuvenile = true;
                        break;
                    case AgeGroup.IsJunior:
                        _isJunior = true;
                        break;
                    case AgeGroup.IsSenior:
                        _isSenior = true;
                        break;
                    case AgeGroup.IsVeteran:
                        _isVeteran = true;
                        break;
                    default:
                        // leave all false for Undefined
                        break;
                }
            }
        }

        /// <summary>
        /// Vets bucket persisted by EF. Setter enforces that only veterans may have a non-null bucket.
        /// </summary>
        public int? VetsBucket
        {
            get => _vetsBucket;
            set
            {
                if (value.HasValue && !_isVeteran)
                    throw new ArgumentException("VetsBucket may only be set for veteran competitors", nameof(VetsBucket));
                // Require veterans to have a bucket.
                if (_isVeteran && !value.HasValue)
                    throw new ArgumentException("Veteran competitors must have a VetsBucket", nameof(VetsBucket));
                _vetsBucket = value;
            }
        }

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

            // Exactly one age-flag or allow Undefined? Here we allow Undefined if business rules permit.
            int trueCount = (IsJuvenile ? 1 : 0) + (IsJunior ? 1 : 0) + (IsSenior ? 1 : 0) + (IsVeteran ? 1 : 0);
            if (trueCount > 1)
                throw new InvalidOperationException("Competitor must not have more than one age group flag set.");

            if (_vetsBucket.HasValue && !IsVeteran)
                throw new InvalidOperationException("VetsBucket may only be set for veteran competitors.");
            if (IsVeteran && !_vetsBucket.HasValue)
                throw new InvalidOperationException("Veteran competitors must have a VetsBucket.");
        }
    }
}
