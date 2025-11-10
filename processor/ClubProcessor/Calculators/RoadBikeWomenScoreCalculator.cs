using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class RoadBikeWomenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Road Bike Women";

        public RoadBikeWomenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { IsFemale: true } c &&
            c.IsEligible() &&
            r.IsRoadBike == true &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoadBikeWomenPosition = position;
            r.RoadBikeWomenPoints = points;
        }

        protected override void ClearPoints(Ride r)
        {
            r.RoadBikeWomenPosition = null;
            r.RoadBikeWomenPoints = null;
        }
    }
}
