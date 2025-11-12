using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class JuniorsScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Juniors";

        public JuniorsScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { IsJunior: true } c &&
            c.IsEligible() &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.JuniorsPosition = position;
            r.JuniorsPoints = points;
        }
    }
}
