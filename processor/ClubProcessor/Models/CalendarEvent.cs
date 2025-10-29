using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClubProcessor.Models
{
    public class CalendarEvent
    {
        [Key]
        public int Id { get; set; } // EF-generated primary key

        public int EventNumber { get; set; } // From calendar sheet or filename

        public string SheetName => $"Event_{EventNumber:D2}";

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Column(TypeName = "REAL")]
        public double Miles { get; set; }

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsHillClimb { get; set; }
        public bool IsClubChampionship { get; set; }
        public bool IsNonStandard10 { get; set; }
        public bool IsEvening10 { get; set; }
        public bool IsHardRideSeries { get; set; }

        public bool IsCancelled { get; set; }
    }
}
