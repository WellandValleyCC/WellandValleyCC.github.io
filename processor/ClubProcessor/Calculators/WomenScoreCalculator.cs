using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    internal class WomenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Women";

        public WomenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary, IsFemale: true } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.WomenPosition = position;
            r.WomenPoints = points;
        }
    }
}
