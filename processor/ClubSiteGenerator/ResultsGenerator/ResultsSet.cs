using ClubCore.Models;
using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet : IResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> Calendar;

        protected ResultsSet(IEnumerable<CalendarEvent> calendar)
        {
            Calendar = calendar;
            Year = calendar.First().EventDate.Year;
        }

        protected int Year { get; }
        public abstract string DisplayName { get; }
        public abstract string FileName { get; }
        public abstract string SubFolderName { get; }
        public string? PrevLink { get; set; }
        public string? NextLink { get; set; }
    }
}
