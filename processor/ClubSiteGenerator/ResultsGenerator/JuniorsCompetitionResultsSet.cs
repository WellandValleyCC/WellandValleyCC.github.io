using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.ResultsGenerator
{
    public sealed class JuniorsCompetitionResultsSet : CompetitionResultsSet
    {
        private JuniorsCompetitionResultsSet(IEnumerable<CalendarEvent> calendar, IEnumerable<CompetitorResult> scoredRides)
            : base(calendar, scoredRides) 
        { 
        }

        public override string DisplayName => "Juniors Championship";
        public override string FileName => "juniors";
        public override string SubFolderName => "competitions";

        public override string GenericName => "Juniors";
        public override AgeGroup? AgeGroupFilter => AgeGroup.Junior;
        public override string CompetitionType => "JNR";

        public override string EligibilityStatement => "All first claim junior members of the club are eligible for this championship.";

        public static JuniorsCompetitionResultsSet CreateFrom(IEnumerable<Ride> allRides, IEnumerable<CalendarEvent> events)
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

            var juniorRides = allRides
                .Where(r =>
                    r.Competitor != null
                    && r.Competitor.IsJunior
                    && r.Status == RideStatus.Valid);

            return null; // new JuniorsCompetitionResultsSet(juniorRides, events);
        }
    }

}
