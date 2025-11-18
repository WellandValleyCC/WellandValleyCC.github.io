using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

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
            r.Status == RideStatus.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoadBikeWomenPosition = position;
            r.RoadBikeWomenPoints = points;
        }
    }
}
