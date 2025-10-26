using ClubProcessor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubProcessor.Models
{
    public class Competitor
    {
        [Key]
        public int Id { get; set; }  // Auto-incremented by EF/SQLite

        public required int ClubNumber { get; set; }       // e.g. 9999
        public required string Surname { get; set; }          // e.g. "Doe"
        public required string GivenName { get; set; }        // e.g. "John"
        public required ClaimStatus ClaimStatus { get; set; }      // e.g. FirstClaim
        public required bool IsFemale { get; set; }           // e.g. false
        public required bool IsJuvenile { get; set; }         // e.g. false
        public required bool IsJunior { get; set; }           // e.g. true
        public required bool IsSenior { get; set; }           // e.g. false
        public required bool IsVeteran { get; set; }          // e.g. false

        public DateTime CreatedUtc { get; set; }
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

        public void Validate()
        {
            if (ClaimStatus == ClaimStatus.Unknown)
                throw new InvalidOperationException("ClaimStatus must be explicitly set.");
        }

    }
}
