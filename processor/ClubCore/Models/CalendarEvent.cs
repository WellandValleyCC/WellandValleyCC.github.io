using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace ClubCore.Models
{
    [DebuggerDisplay("#{EventNumber} {EventDateShort}")]
    public class CalendarEvent
    {
        [Key]
        public int Id { get; set; } // EF-generated primary key

        public int EventNumber { get; set; } // From calendar sheet or filename

        [NotMapped]
        public int RoundRobinEventNumber { get; set; } // Sequential number within RR subset

        public string SheetName => $"Event_{EventNumber:D2}";

        [Required]
        public DateTime EventDate { get; set; }

        [NotMapped]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string EventDateShort => EventDate.ToString("yyyy-MM-dd");

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        public string RoundRobinClub { get; set; } = string.Empty;

        [Column(TypeName = "REAL")]
        public double Miles { get; set; }

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsHillClimb { get; set; }
        public bool IsClubChampionship { get; set; }
        public bool IsNonStandard10 { get; set; }
        public bool IsEvening10 { get; set; }
        public bool IsHardRideSeries { get; set; }
        public bool IsRoundRobinEvent { get; set; }

        public bool IsCancelled { get; set; }

        public bool IsPostponed { get; set; }

        /// <summary>
        /// Indicates whether this is a shortened ten mile event.  If so, then Nev Brooks handicaps for furture 
        /// events should be not be calculated.
        /// Set this to true (Y in the CSV) for shortened evening tens - e.g. 11/07/2026, Gilmorton 10 was 
        /// reduced to 6.35 miles due to roadworks.
        /// </summary>
        public bool IsShortenedTen { get; set; }
    }
}
