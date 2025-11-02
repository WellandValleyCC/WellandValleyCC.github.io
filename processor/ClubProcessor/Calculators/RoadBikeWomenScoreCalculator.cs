using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    internal class RoadBikeWomenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Road Bike Women";

        public RoadBikeWomenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.IsRoadBike == true &&
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary, IsFemale: true } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoadBikeWomenPosition = position;
            r.RoadBikeWomenPoints = points;
        }
    }
}
