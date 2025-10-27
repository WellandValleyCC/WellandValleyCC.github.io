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
        public int? EventPosition { get; set; }
        public int? EventRoadBikePosition { get; set; }
        public int? SeniorsPosition { get; set; }
        public int? SeniorsPoints { get; set; }
        public int? WomenPosition { get; set; }
        public int? WomenPoints { get; set; }
        public TimeSpan? VeteransStandardTime { get; set; }
        public TimeSpan? VeteranssTimeDelta { get; set; }
        public int? VeteransPosition { get; set; }
        public int? VeteransPoints { get; set; }
        public int? RoadBikeMenPosition { get; set; }
        public int? RoadBikeMenPoints { get; set; }
        public int? RoadBikeWomenPosition { get; set; }
        public int? RoadBikeWomenPoints { get; set; }
        public int? JuvenilesPosition { get; set; }
        public int? JuvenilesPoints { get; set; }
        public int? JuniorsPosition { get; set; }
        public int? JuniorsPoints { get; set; }
        public TimeSpan? HandicapGenerated { get; set; }
        public TimeSpan? HandicapUsed { get; set; }
        public TimeSpan? HandicapAdjustedTime { get; set; }
        public int? HandicapPosition { get; set; }
        public int? HandicapPoints { get; set; }
        public bool IsGuest => ClubNumber == null && !string.IsNullOrWhiteSpace(Name);

        public RideEligibility Eligibility { get; set; } = RideEligibility.Undefined;
        public string? Notes { get; set; }
    }
}
