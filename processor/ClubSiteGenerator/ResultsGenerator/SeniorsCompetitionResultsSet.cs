using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    // Seniors
    public sealed class SeniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private SeniorsCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides) 
        { }

        public override string DisplayName => "Seniors Championship";
        public override string FileName => "seniors.html";
        public override string SubFolderName => "competitions";
        public override string GenericName => "Seniors";
        public override AgeGroup? AgeGroupFilter => null;
        public override string CompetitionType => "SNR";
        public override string EligibilityStatement => "All first claim members of the club are eligible for this championship - all age groups.";

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

            return null; // new SeniorsCompetitionResultsSet(allRides, events);
        }
    }

}
