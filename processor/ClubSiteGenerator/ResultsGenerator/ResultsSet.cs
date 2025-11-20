using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public abstract class ResultsSet
    {
        protected readonly IEnumerable<Ride> Rides;

        protected ResultsSet(IEnumerable<Ride> rides)
        {
            Rides = rides;
        }

        public abstract string DisplayName { get; }
        public abstract string FileName { get; }
        public abstract string SubFolderName { get; }

        public abstract HtmlTable CreateTable();
    }

    // -------------------- Event Results --------------------

    public sealed class EventResultsSet : ResultsSet
    {
        private readonly CalendarEvent _calendarEvent;

        private EventResultsSet(CalendarEvent ev, IEnumerable<Ride> rides) : base(rides)
        {
            _calendarEvent = ev;
        }

        public override string DisplayName => $"Results for {_calendarEvent.EventName}";
        public override string FileName => $"event-{_calendarEvent.EventNumber}.html";
        public override string SubFolderName => "events";

        public override HtmlTable CreateTable() => EventTableBuilder.Build(_calendarEvent, Rides);

        // Factory: single CalendarEvent
        public static EventResultsSet CreateFrom(CalendarEvent ev, IEnumerable<Ride> allRides)
        {
            var ridesForEvent = allRides.Where(r => r.EventNumber == ev.EventNumber);
            return new EventResultsSet(ev, ridesForEvent);
        }
    }

    // -------------------- Competition Results --------------------

    public abstract class CompetitionResultsSet : ResultsSet
    {
        protected readonly IEnumerable<CalendarEvent> CalendarEvents;

        protected CompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides)
        {
            CalendarEvents = events;
        }

        public abstract AgeGroup? AgeGroupFilter { get; }
        public abstract string CompetitionCode { get; }
    }

    // Juveniles
    public sealed class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        private JuvenilesCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides, events) { }

        public override string DisplayName => "Juveniles Competition";
        public override string FileName => "juveniles.html";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Juvenile;
        public override string CompetitionCode => "JUV";

        public override HtmlTable CreateTable() =>
            CompetitionTableBuilder.Build(Rides, CalendarEvents, AgeGroupFilter);

        // Factory: full calendar
        public static JuvenilesCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            var juvenileRides = allRides.Where(r => r.Competitor.AgeGroup == AgeGroup.Juvenile);
            return new JuvenilesCompetitionResultsSet(juvenileRides, events);
        }
    }

    // Juniors
    public sealed class JuniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private JuniorsCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides, events) { }

        public override string DisplayName => "Juniors Competition";
        public override string FileName => "juniors.html";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Junior;
        public override string CompetitionCode => "JNR";

        public override HtmlTable CreateTable() =>
            CompetitionTableBuilder.Build(Rides, CalendarEvents, AgeGroupFilter);

        public static JuniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            var juniorRides = allRides.Where(r => r.Competitor.AgeGroup == AgeGroup.Junior);
            return new JuniorsCompetitionResultsSet(juniorRides, events);
        }
    }

    // Seniors
    public sealed class SeniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private SeniorsCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides, events) { }

        public override string DisplayName => "Seniors Competition";
        public override string FileName => "seniors.html";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => null;
        public override string CompetitionCode => "SNR";

        public override HtmlTable CreateTable() =>
            CompetitionTableBuilder.Build(Rides, CalendarEvents, AgeGroupFilter);

        public static SeniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            return new SeniorsCompetitionResultsSet(allRides, events);
        }
    }

}
