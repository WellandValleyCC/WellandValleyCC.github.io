using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;

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
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.SeniorsPosition = position;
            r.SeniorsPoints = points;
        }
    }
}
