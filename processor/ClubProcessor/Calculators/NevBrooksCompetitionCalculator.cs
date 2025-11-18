using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClubProcessor.Calculators
{
    public class NevBrooksCompetitionCalculator : BaseCompetitionScoreCalculator
    {
        public override string CompetitionName => "Nev Brooks";

        // Lookup: ClubNumber --> last generated handicap seconds
        private readonly Dictionary<int, double> previousGeneratedByClubNumber = new();

        public NevBrooksCompetitionCalculator(Func<int, int> pointsForPosition) : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor?.IsEligible() == true &&
            r.Status == RideStatus.Valid &&
            r.CalendarEvent?.IsEvening10 == true;

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

                // Only apply if we really had a prior generated value
                if (r.ClubNumber.HasValue && previousGeneratedByClubNumber.TryGetValue(r.ClubNumber.Value, out var prevGenerated))
                {
                    r.NevBrooksSecondsApplied = prevGenerated;
                    r.NevBrooksSecondsAdjustedTime = r.TotalSeconds - prevGenerated;
                }
                else
                {
                    r.NevBrooksSecondsApplied = null;
                    r.NevBrooksSecondsAdjustedTime = null;
                }

                // Update lookup for next event — after we’ve set Generated
                if (r.ClubNumber.HasValue && r.NevBrooksSecondsGenerated.HasValue)
                {
                    previousGeneratedByClubNumber[r.ClubNumber.Value] = r.NevBrooksSecondsGenerated.Value;
                }
            }

            var scoredRides = eventRides.Where(r => r.NevBrooksSecondsAdjustedTime.HasValue).ToList();
            return ApplyScores(eventNumber, scoredRides, pointsForPosition);
        }
    }
}
