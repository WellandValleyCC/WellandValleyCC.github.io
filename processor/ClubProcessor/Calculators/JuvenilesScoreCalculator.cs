using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;

namespace ClubProcessor.Calculators
{
    internal class JuvenilesScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Juveniles";

        public JuvenilesScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { IsJuvenile: true } c &&
            c.IsEligible() &&
            r.Eligibility == RideEligibility.Valid;


        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.JuvenilesPosition = position;
            r.JuvenilesPoints = points;
        }
    }
}
