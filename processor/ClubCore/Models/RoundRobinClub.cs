namespace ClubCore.Models
{
    public class RoundRobinClub
    {
        public int Id { get; set; }
        public string ShortName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string WebsiteUrl { get; set; } = default!;
        public int FromYear { get; set; }
    }
}