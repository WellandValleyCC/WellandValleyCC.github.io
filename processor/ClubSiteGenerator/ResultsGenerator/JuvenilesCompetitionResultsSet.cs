using ClubCore.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        public JuvenilesCompetitionResultsSet(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> eventsCalendar)
            : base(allRides, eventsCalendar) { }

        public override string DisplayName => "Juveniles Competition";
        public override string FileName => "juveniles-competition";
        public override string SubFolderName => "competitions";

        public override IEnumerable<Ride> FilteredRides()
            => AllRides.Where(r => r.Competitor?.AgeGroup == AgeGroup.Juvenile);

        protected override double? GetPoints(Ride ride) => ride.JuvenilesPoints;
    }

}
