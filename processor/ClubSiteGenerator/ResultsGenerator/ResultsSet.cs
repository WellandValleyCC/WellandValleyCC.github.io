using ClubCore.Models;
using ClubSiteGenerator.Interfaces;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet : IResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> Calendar;

        protected ResultsSet(IEnumerable<CalendarEvent> calendar)
        {
            Calendar = calendar ?? throw new ArgumentNullException(nameof(calendar));
            Year = calendar.First().EventDate.Year;
        }

        public int Year { get; }
        public abstract string DisplayName { get; }
        public abstract string FileName { get; }
        public abstract string SubFolderName { get; }
        public abstract string LinkText { get; }
        public string? PrevLink { get; set; }
        public string? NextLink { get; set; }
        public string? PrevLabel { get; set; }
        public string? NextLabel { get; set; }

        protected static bool HasMissingCompetitors(IEnumerable<Ride> rides) =>
            rides.Any(r => r.ClubNumber != null && r.Competitor is null);

        protected static bool HasMissingCalendarEvents(IEnumerable<Ride> rides) =>
            rides.Any(r => r.CalendarEvent is null);

        protected static bool HasNonChampionshipEvents(IEnumerable<CalendarEvent> calendar) =>
            calendar.Any(ev => !ev.IsClubChampionship);

        protected static bool HasNonRoundRobinEvents(IEnumerable<CalendarEvent> calendar) =>
    calendar.Any(ev => !ev.IsRoundRobinEvent);

    }
}
