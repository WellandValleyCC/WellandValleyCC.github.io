namespace ClubProcessor.Models
{
    public class Competitor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Category { get; set; } // e.g. Juvenile, Junior, Senior
        public bool IsFemale { get; set; }
        public string MemberNumber { get; set; } // Used for lookups from event sheets
    }
}
