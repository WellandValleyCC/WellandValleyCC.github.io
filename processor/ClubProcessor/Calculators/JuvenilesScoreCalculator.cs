using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System.Reflection.Metadata.Ecma335;

namespace ClubProcessor.Calculators
{
    internal class JuvenilesScoreCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Juveniles";

        public JuvenilesScoreCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary, IsJuvenile: true } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.JuvenilesPosition = position;
            r.JuvenilesPoints = points;
        }
    }
}
