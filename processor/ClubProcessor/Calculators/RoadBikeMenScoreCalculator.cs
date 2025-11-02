using ClubProcessor.Models;
using ClubProcessor.Models.Enums;

namespace ClubProcessor.Calculators
{
    internal class RoadBikeMenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Road Bike Men";

        public RoadBikeMenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.IsRoadBike == true &&
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary, IsFemale: false } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoadBikeMenPosition = position;
            r.RoadBikeMenPoints = points;
        }
    }
}
