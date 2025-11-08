using ClubProcessor.Interfaces;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubProcessor.Calculators
{
    public class VeteransCompetitionCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Veterans";

        private readonly int competitionYear;
        private readonly IVetsHandicapProvider handicapProvider;

        public VeteransCompetitionCalculator(
            Func<int, int> pointsForPosition,
            int competitionYear,
            IVetsHandicapProvider handicapProvider)
            : base(pointsForPosition)
        {
            this.competitionYear = competitionYear;
            this.handicapProvider = handicapProvider;
        }

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
            // Only validate if we need to calculate
            if (!r.HandicapSeconds.HasValue)
            {
                if (r.Competitor == null)
                    throw new InvalidOperationException($"Competitor not set for ride {r.Name}");

                if (!r.Competitor.VetsBucket.HasValue)
                    throw new InvalidOperationException($"Veterans bucket not set for competitor {r.Competitor.ClubNumber} - {r.Name}");

                if (r.CalendarEvent == null)
                    throw new InvalidOperationException($"CalendarEvent not set for ride {r.Competitor.ClubNumber} - {r.Name}");

                r.HandicapSeconds = handicapProvider.GetHandicapSeconds(
                    competitionYear,
                    r.CalendarEvent.Miles,
                    r.Competitor.IsFemale,
                    r.Competitor.VetsBucket.Value);
            }

            return r.HandicapTotalSeconds
                ?? throw new InvalidOperationException($"HandicapTotalSeconds could not be calculated for ride {r.Competitor?.ClubNumber} - {r.Name}");
        }

        public override int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            foreach (var r in eventRides)
            {
                r.HandicapSeconds = null;
            }

            return base.ProcessEvent(eventNumber, eventRides);
        }

    }
}
