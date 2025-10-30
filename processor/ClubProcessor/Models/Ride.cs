using ClubProcessor.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ClubProcessor.Models
{
    [DebuggerDisplay("Event {EventNumber} - {ClubNumber?.ToString() ?? Name} - {Time} - {RoadBikeIndicator} {ClaimStatusDisplay} {EventDateDisplay}")]
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
        /// Overall position in the event, used for Seniors scoring.
        /// </summary>
        public int? EventPosition { get; set; }

        /// <summary>
        /// Gender-neutral RoadBike position for display and filtering.
        /// </summary>
        public int? EventRoadBikePosition { get; set; }

        /// <summary>
        /// Alias for EventPosition used in Seniors scoring context.
        /// Not persisted in the database.
        /// </summary>
        [NotMapped]
        public int? SeniorsPosition
        {
            get => EventPosition;
            set => EventPosition = value;
        }
        public int? SeniorsPoints { get; set; }

        public int? WomenPosition { get; set; }
        public int? WomenPoints { get; set; }

        [NotMapped]
        public TimeSpan? VeteransStandardTime { get; set; }

        [NotMapped]
        public TimeSpan? VeteransTimeDelta { get; set; }

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

        public int? PremPosition { get; set; }
        public int? PremPoints { get; set; }

        public int? League1Position { get; set; }
        public int? League1Points { get; set; }

        public int? League2Position { get; set; }
        public int? League2Points { get; set; }

        public int? League3Position { get; set; }
        public int? League3Points { get; set; }

        public int? League4Position { get; set; }
        public int? League4Points { get; set; }

        public int? NevBrooksPosition { get; set; }
        public int? NevBrooksPoints { get; set; }

        public int? NevBrooksSecondsGenerated { get; set; }     // Raw handicap value
        public int? NevBrooksSecondsApplied { get; set; }       // Capped or adjusted handicap
        public int? NevBrooksSecondsAdjustedTime { get; set; }  // Final time after handicap

        public bool IsGuest => ClubNumber == null && !string.IsNullOrWhiteSpace(Name);

        public RideEligibility Eligibility { get; set; } = RideEligibility.Undefined;
        public string? Notes { get; set; }

        [NotMapped]
        public Competitor? Competitor { get; set; }

        [NotMapped] 
        public CalendarEvent? CalendarEvent { get; set; }

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string RoadBikeIndicator => IsRoadBike ? "Road Bike" : "TT Bike";

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string ClaimStatusDisplay => Competitor != null ? Competitor.ClaimStatus.ToString() : string.Empty;

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string EventDateDisplay => CalendarEvent != null ? CalendarEvent.EventDate.ToShortDateString() : string.Empty;

    }
}
