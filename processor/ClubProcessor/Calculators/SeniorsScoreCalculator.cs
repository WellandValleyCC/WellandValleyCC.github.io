using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

namespace ClubProcessor.Calculators
{
    /// <summary>
    /// Scores all FirstClaim and Honorary members based on raw ride time.
    /// </summary>
    internal class SeniorsScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Seniors";

        public SeniorsScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor?.IsEligible() == true &&
            r.Status == RideStatus.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.SeniorsPosition = position;
            r.SeniorsPoints = points;
        }
    }
}
