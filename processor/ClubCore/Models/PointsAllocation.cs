namespace ClubCore.Models
{
    public class PointsAllocation
    {
        public int Id { get; set; }         // Primary key
        public int Position { get; set; }   // Rider's finishing position
        public int Points { get; set; }     // Points awarded for that position
    }
}