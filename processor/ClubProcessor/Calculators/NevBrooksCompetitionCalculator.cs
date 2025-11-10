using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubProcessor.Calculators
{
    public class NevBrooksCompetitionCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Nev Brooks";

        // Lookup: ClubNumber → last generated handicap seconds
        private readonly Dictionary<int, double> previousGeneratedByClub = new();

        public NevBrooksCompetitionCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor is { ClaimStatus: ClaimStatus.FirstClaim or ClaimStatus.Honorary } &&
            r.Eligibility == RideEligibility.Valid &&
            r.CalendarEvent?.Miles == 10.0;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.NevBrooksPosition = position;
            r.NevBrooksPoints = points;
        }

        protected override double GetOrderingTime(Ride r)
        {
            if (!r.NevBrooksSecondsAdjustedTime.HasValue)
                throw new InvalidOperationException($"AdjustedTime not set for ride {r.Competitor?.ClubNumber} - {r.Name}");

            return r.NevBrooksSecondsAdjustedTime.Value;
        }

        public override int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            foreach (var r in eventRides)
            {
                if (!IsEligible(r))
                {
                    r.NevBrooksPosition = null;
                    r.NevBrooksPoints = null;
                    r.NevBrooksSecondsGenerated = null;
                    r.NevBrooksSecondsApplied = null;
                    r.NevBrooksSecondsAdjustedTime = null;
                    continue;
                }

                // Always generate handicap for this ride
                r.NevBrooksSecondsGenerated = r.TotalSeconds - 995.0;

                if (r.ClubNumber.HasValue && previousGeneratedByClub.TryGetValue(r.ClubNumber.Value, out var prevGenerated))
                {
                    r.NevBrooksSecondsApplied = prevGenerated;
                    r.NevBrooksSecondsAdjustedTime = r.TotalSeconds - prevGenerated;
                }
                else
                {
                    r.NevBrooksSecondsApplied = null;
                    r.NevBrooksSecondsAdjustedTime = null;
                }

                // Update lookup for next event
                if (r.ClubNumber.HasValue)
                    previousGeneratedByClub[r.ClubNumber.Value] = r.NevBrooksSecondsGenerated.Value;
            }

            // Score only rides with AdjustedTime
            var scoredRides = eventRides.Where(r => r.NevBrooksSecondsAdjustedTime.HasValue).ToList();
            return ApplyScores(eventNumber, scoredRides, pointsForPosition);
        }
    }
}
