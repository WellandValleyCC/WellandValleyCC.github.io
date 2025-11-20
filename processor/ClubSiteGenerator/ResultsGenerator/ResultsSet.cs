using ClubCore.Models;
using ClubCore.Models.Enums;
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
        private readonly CalendarEvent calendarEvent;

        private EventResultsSet(CalendarEvent ev, IEnumerable<Ride> rides) : base(rides)
        {
            this.calendarEvent = ev;
        }

        public CalendarEvent CalendarEvent => calendarEvent;
        public int EventNumber => calendarEvent.EventNumber;
        public DateOnly EventDate => DateOnly.FromDateTime(calendarEvent.EventDate);

        public override string DisplayName => $"Results for {calendarEvent.EventName}";
        public override string FileName => $"event-{calendarEvent.EventNumber}.html";
        public override string SubFolderName => "events";

        public override HtmlTable CreateTable()
        {
            var headers = new List<string>
        {
            "Name", "Position", "Road Bike", "Actual Time", "Avg. mph"
        };

            var rides = Rides.ToList();

            var ranked = rides.Where(r => r.Status == RideStatus.Valid)
                              .OrderBy(r => r.EventRank);

            var dnfs = OrderedIneligibleRides(rides, RideStatus.DNF);
            var dnss = OrderedIneligibleRides(rides, RideStatus.DNS);
            var dqs = OrderedIneligibleRides(rides, RideStatus.DQ);

            var ordered = ranked.Concat(dnfs).Concat(dnss).Concat(dqs);

            var rows = ordered.Select(r =>
            {
                var miles = r.CalendarEvent?.Miles ?? 0;
                var avgMph = r.AvgSpeed?.ToString("0.00") ?? string.Empty;

                var timeCell = r.Status switch
                {
                    RideStatus.DNF => "DNF",
                    RideStatus.DNS => "DNS",
                    RideStatus.DQ => "DQ",
                    _ => TimeSpan.FromSeconds(r.TotalSeconds).ToString(@"hh\:mm\:ss")
                };

                var cells = new List<string>
            {
                r.Name ?? "Unknown",
                r.EventRank?.ToString() ?? "",
                r.EventRoadBikeRank?.ToString() ?? "",
                timeCell,
                avgMph
            };

                return new HtmlRow(cells, r);
            });

            return new HtmlTable(headers, rows);
        }

        private static IEnumerable<Ride> OrderedIneligibleRides(IEnumerable<Ride> rides, RideStatus eligibility)
        {
            var firstClaim = rides
                .Where(r => r.Status == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.FirstClaim)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var secondClaim = rides
                .Where(r => r.Status == eligibility && r.ClubNumber != null && r.Competitor?.ClaimStatus == ClaimStatus.SecondClaim)
                .OrderBy(r => r.Competitor!.Surname)
                .ThenBy(r => r.Competitor!.GivenName);

            var guests = rides
                .Where(r => r.Status == eligibility && r.ClubNumber == null)
                .OrderBy(r =>
                {
                    var parts = (r.Name ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    return parts.Length == 2
                        ? $"{parts[1]} {parts[0]}"   // "Smith Alice"
                        : r.Name ?? "";              // fallback if not exactly two parts
                });

            return firstClaim.Concat(secondClaim).Concat(guests);
        }

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

        public override HtmlTable CreateTable() => throw new NotImplementedException();

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

        public override HtmlTable CreateTable() => throw new NotImplementedException();

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

        public override HtmlTable CreateTable() => throw new NotImplementedException();

        public static SeniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            return new SeniorsCompetitionResultsSet(allRides, events);
        }
    }

}
