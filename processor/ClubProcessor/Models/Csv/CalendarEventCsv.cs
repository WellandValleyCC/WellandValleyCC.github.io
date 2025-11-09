using CsvHelper.Configuration.Attributes;

namespace ClubProcessor.Models.Csv
{
    public class CalendarEventCsv
    {
        [Name("Event Number")]
        public int EventNumber { get; set; }

        [Name("Date")]
        public string DateRaw { get; set; } = string.Empty;

        [Name("Start time")]
        public string StartTimeRaw { get; set; } = string.Empty;

        [Name("Event Name")]
        public string EventName { get; set; } = string.Empty;

        [Name("Miles")]
        public double Miles { get; set; }

        [Name("Location / Course")]
        public string Location { get; set; } = string.Empty;

        [Name("Hill Climb")]
        public string HillClimbRaw { get; set; } = string.Empty;

        [Name("Club Championship")]
        public string ClubChampRaw { get; set; } = string.Empty;

        [Name("Non-Standard 10")]
        public string NonStd10Raw { get; set; } = string.Empty;

        [Name("Evening 10")]
        public string Evening10Raw { get; set; } = string.Empty;

        [Name("Hard Ride Series")]
        public string HardRideRaw { get; set; } = string.Empty;

        [Name("Sheet Name")]
        public string SheetName { get; set; } = string.Empty;

        [Name("isCancelled")]
        public string CancelledRaw { get; set; } = string.Empty;
    }
}
