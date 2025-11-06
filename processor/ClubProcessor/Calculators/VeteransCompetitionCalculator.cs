using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubProcessor.Calculators
{
    internal class VeteransCompetitionCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Veterans";

        public VeteransCompetitionCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary, AgeGroup: AgeGroup.Veteran } &&
            r.Eligibility == RideEligibility.Valid;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.VeteransPosition = position;
            r.VeteransPoints = points;
        }
    }
}
