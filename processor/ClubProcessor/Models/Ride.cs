using ClubProcessor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ClubProcessor.Models
{
    [DebuggerDisplay("Event {EventNumber} - {ClubNumber?.ToString() ?? Name} - {TotalSeconds}s - {Gender} {RoadBikeIndicator} {ClaimStatusDisplay} {AgeGroupDisplay} {EventDateDisplay}")]
    public class Ride
    {
        [Key]
        public int Id { get; set; }
        public int EventNumber { get; set; }
        public int? ClubNumber { get; set; }
        public string? Name { get; set; } = string.Empty;
        public bool IsRoadBike { get; set; }
        public double TotalSeconds { get; set; }

        [NotMapped]
        public TimeSpan? Time => TotalSeconds > 0 ? TimeSpan.FromSeconds(TotalSeconds) : null;

        public double? AvgSpeed { get; set; }

        /// <summary>
        /// Overall rank in the event, used for Event page rendering
        /// </summary>
        public int? EventRank { get; set; }

        /// <summary>
        /// Gender-neutral RoadBike rank for display and filtering.
        /// </summary>
        public int? EventRoadBikeRank { get; set; }

        public int? SeniorsPosition { get; set; }
        public double? SeniorsPoints { get; set; }

        public int? WomenPosition { get; set; }
        public double? WomenPoints { get; set; }

        [NotMapped]
        public TimeSpan? VeteransStandardTime { get; set; }

        [NotMapped]
        public TimeSpan? VeteransTimeDelta { get; set; }

        public double? VeteransHandicapSeconds { get; set; }
        [NotMapped]
        public double? VeteransHandicapTotalSeconds => VeteransHandicapSeconds == null ? null : TotalSeconds - VeteransHandicapSeconds.Value;
        public int? VeteransPosition { get; set; }
        public double? VeteransPoints { get; set; }

        public int? RoadBikeMenPosition { get; set; }
        public double? RoadBikeMenPoints { get; set; }

        public int? RoadBikeWomenPosition { get; set; }
        public double? RoadBikeWomenPoints { get; set; }

        public int? JuvenilesPosition { get; set; }
        public double? JuvenilesPoints { get; set; }

        public int? JuniorsPosition { get; set; }
        public double? JuniorsPoints { get; set; }

        public int? LeaguePosition { get; set; }
        public double? LeaguePoints { get; set; }

        public int? NevBrooksPosition { get; set; }
        public double? NevBrooksPoints { get; set; }

        public double? NevBrooksSecondsGenerated { get; set; }     // Raw handicap value
        public double? NevBrooksSecondsApplied { get; set; }       // Capped or adjusted handicap
        public double? NevBrooksSecondsAdjustedTime { get; set; }  // Final time after handicap

        public bool IsGuest => ClubNumber == null && !string.IsNullOrWhiteSpace(Name);

        public RideEligibility Eligibility { get; set; } = RideEligibility.Undefined;
        public string? Notes { get; set; }

        [NotMapped]
        public Competitor? Competitor { get; set; }

        [NotMapped]
        public CalendarEvent? CalendarEvent { get; set; }

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string RoadBikeIndicator => IsRoadBike ? "Road" : "TT";

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Gender => Competitor != null ? Competitor.Gender : string.Empty;

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ClaimStatusDisplay => Competitor != null ? Competitor.ClaimStatus.ToString() : string.Empty;

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string AgeGroupDisplay => Competitor != null ? Competitor.AgeGroup.ToString() : string.Empty;

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string EventDateDisplay => CalendarEvent != null ? CalendarEvent.EventDate.ToShortDateString() : string.Empty;
    }
}
