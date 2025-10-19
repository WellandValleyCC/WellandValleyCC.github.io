namespace ClubProcessor.Models
{
	public class Competitor
	{
		public required string ClubNumber { get; set; }       // e.g. "9999"
		public required string Surname { get; set; }          // e.g. "Doe"
		public required string GivenName { get; set; }        // e.g. "John"
		public required string ClaimStatus { get; set; }      // e.g. "First Claim"
		public required bool IsFemale { get; set; }                    // e.g. false
		public required bool IsJuvenile { get; set; }                  // e.g. false
		public required bool IsJunior { get; set; }                    // e.g. true
		public required bool IsSenior { get; set; }                    // e.g. false
		public required bool IsVeteran { get; set; }                   // e.g. false
	}
}
