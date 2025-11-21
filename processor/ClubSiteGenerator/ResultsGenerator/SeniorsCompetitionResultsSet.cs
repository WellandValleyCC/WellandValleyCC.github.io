using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
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
