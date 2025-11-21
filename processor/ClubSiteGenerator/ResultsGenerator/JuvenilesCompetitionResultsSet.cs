using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    // Juveniles
    public sealed class JuvenilesCompetitionResultsSet : CompetitionResultsSet
    {
        private JuvenilesCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides, events) { }

        public override string DisplayName => "Juveniles Competition";
        public override string FileName => "juveniles";
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

}
