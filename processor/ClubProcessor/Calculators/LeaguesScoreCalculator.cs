using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    internal class LeaguesScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Leagues";

        public LeaguesScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary } &&
            r.Competitor is not { League: League.Undefined } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.LeaguePosition = position;
            r.LeaguePoints = points;
        }

        public override int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            // Partition rides by league
            var ridesByLeague = eventRides
                .Where(r => r.Competitor != null)
                .GroupBy(r => r.Competitor!.League);

            int totalProcessed = 0;

            foreach (var leagueGroup in ridesByLeague)
            {
                // Apply scoring independently within each league
                totalProcessed += ApplyScores(eventNumber, leagueGroup.ToList(), pointsForPosition);
            }

            return totalProcessed;
        }
    }
}
