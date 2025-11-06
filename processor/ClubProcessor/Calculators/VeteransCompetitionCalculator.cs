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
        protected override double GetOrderingTime(Ride r)
        {
            if (!r.Competitor?.VetsBucket.HasValue ?? true)
                throw new InvalidOperationException($"Veterans bucket not set for competitor {r.Competitor.ClubNumber} - {r.Name}");

            return r.TotalSeconds;

            // TO DO: Implement handicap calculation when vets provider is available
            //var year = r.CalendarEvent.EventDate.Year;
            //var handicapSeconds = _vetsProvider.GetHandicapSeconds(year, r.DistanceMiles, r.Competitor.IsFemale, r.Competitor.VetsBucket.Value);

            //r.HandicapSeconds = handicapSeconds;
            //r.HandicapTotalSeconds = r.TotalSeconds - handicapSeconds;

            //return r.HandicapTotalSeconds;
        }

    }
}
