using ClubCore.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet
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
    }
}
