using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class RoadBikeMenScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Road Bike Men";

        public RoadBikeMenScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { IsFemale: false } c &&
            c.IsEligible() &&
            r.IsRoadBike == true &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.RoadBikeMenPosition = position;
            r.RoadBikeMenPoints = points;
        }
    }
}
