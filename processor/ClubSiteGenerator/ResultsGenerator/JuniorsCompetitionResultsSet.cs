using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubSiteGenerator.ResultsGenerator
{
    // Juniors
    public sealed class JuniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private JuniorsCompetitionResultsSet(IEnumerable<Ride> rides, IEnumerable<CalendarEvent> events)
            : base(rides, events) { }

        public override string DisplayName => "Juniors Competition";
        public override string FileName => "juniors";
        public override string SubFolderName => "competitions";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Junior;
        public override string CompetitionCode => "JNR";

        public static JuniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            var juniorRides = allRides
                .Where(r =>
                    r.Competitor != null
                    && r.Competitor.IsJunior
                    && r.Status == RideStatus.Valid);
            return new JuniorsCompetitionResultsSet(juniorRides, events);
        }
    }

}
