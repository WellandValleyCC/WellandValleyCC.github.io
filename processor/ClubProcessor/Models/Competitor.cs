namespace ClubProcessor.Models
{
	public class Competitor
	{
		public int Id { get; set; }

		public required string Name { get; set; }
		public int Age { get; set; }
		public required string Category { get; set; }
		public bool IsFemale { get; set; }
		public required string MemberNumber { get; set; }
	}
}
