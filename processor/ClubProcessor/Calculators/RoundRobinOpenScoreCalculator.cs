using ClubCore.Models;
using ClubCore.Models.Enums;

namespace ClubProcessor.Calculators
{
    /// <summary>
    /// Scores all riders from WVCC or any of the RoundRobin clubs based on raw ride time.
    /// </summary>
    internal class RoundRobinOpenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "RoundRobinOpen";

        public RoundRobinOpenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            HasParticipant(r) &&
            r.Status == RideStatus.Valid;

        private static bool HasParticipant(Ride r) =>
            r.Competitor is not null || r.RoundRobinClub is not null;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoundRobinPosition = position;
            r.RoundRobinPoints = points;
        }
    }
}


