using ClubCore.Models;

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

        public static SeniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
        {
            if (allRides.Any(r => r.ClubNumber != null && r.Competitor is null))
            {
                throw new ArgumentException(
                    $"{nameof(allRides)} collection must be hydrated with Competitors.",
                    nameof(allRides));
            }

            if (allRides.Any(r => r.CalendarEvent is null))
            {
                throw new ArgumentException(
                    $"{nameof(allRides)} collection must be hydrated with CalendarEvents.",
                    nameof(allRides));
            }

            return new SeniorsCompetitionResultsSet(allRides, events);
        }
    }

}
