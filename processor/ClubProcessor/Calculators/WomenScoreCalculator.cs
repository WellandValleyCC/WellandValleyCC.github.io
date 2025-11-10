using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class WomenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Women";

        public WomenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { IsFemale: true } c &&
            c.IsEligible() &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.WomenPosition = position;
            r.WomenPoints = points;
        }

        protected override void ClearPoints(Ride r)
        {
            r.WomenPosition = null;
            r.WomenPoints = null;
        }
    }
}
