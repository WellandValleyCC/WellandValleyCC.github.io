using ClubProcessor.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClubProcessor.Models
{
    public class Ride
    {
        [Key]
        public int Id { get; set; }
        public int EventNumber { get; set; }
        public int? ClubNumber { get; set; }
        public string? Name { get; set; } = string.Empty;
        public bool IsRoadBike { get; set; }
        public string ActualTime { get; set; } = string.Empty;
        public double TotalSeconds { get; set; }
        public double? AvgSpeed { get; set; }
        public int? Position { get; set; }
        public int? PointsAwarded { get; set; }
        public string? StandardTime { get; set; }
        public string? TimeDelta { get; set; }
        public string? HandicapGenerated { get; set; }
        public string? HandicapUsed { get; set; }
        public string? HCAdjustedTime { get; set; }
        public int? HCRank { get; set; }
        public bool IsGuest => ClubNumber == null && !string.IsNullOrWhiteSpace(Name);

        public RideEligibility Eligibility { get; set; } = RideEligibility.Undefined;
        public string? Notes { get; set; }
    }
}
