using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubProcessor.Calculators
{
    /// <summary>
    /// Scores all female riders from WVCC or any of the RoundRobin clubs based on raw ride time.
    /// </summary>
    internal class RoundRobinWomenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "RoundRobinWomen";

        public RoundRobinWomenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            HasParticipant(r) &&
            (r.Competitor is { IsFemale: true } c || r.RoundRobinRider is { IsFemale: true } rr) &&
            r.Status == RideStatus.Valid;
        
        private static bool HasParticipant(Ride r) =>
            r.Competitor is not null || r.RoundRobinClub is not null;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoundRobinWomenPosition = position;
            r.RoundRobinWomenPoints = points;
        }
    }
}


