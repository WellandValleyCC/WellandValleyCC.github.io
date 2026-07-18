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

        public NevBrooksCompetitionCalculator(Func<int, int> pointsForPosition)
            : base(pointsForPosition) { }

        protected override bool IsEligible(Ride r) =>
            r.Competitor?.IsEligible() == true &&
            r.Status == RideStatus.Valid &&
            r.CalendarEvent?.IsEvening10 == true;

        private bool IsShortened(Ride r) =>
            r.CalendarEvent?.IsShortenedTen == true;

        protected override void AssignPoints(Ride r, int position, double points)
        {
            r.NevBrooksPosition = position;
            r.NevBrooksPoints = points;
        }

        protected override double GetOrderingTime(Ride r)
        {
            if (!r.NevBrooksSecondsAdjustedTime.HasValue)
                throw new InvalidOperationException(
                    $"AdjustedTime not set for ride {r.Competitor?.ClubNumber} - {r.Name}");

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

                // --- RULE: Shortened Ten → DO NOT generate handicap ---
                if (IsShortened(r))
                {
                    r.NevBrooksSecondsGenerated = null;
                }
                else
                {
                    r.NevBrooksSecondsGenerated = r.TotalSeconds - 995.0;
                }

                // Apply previous handicap if available
                if (r.ClubNumber.HasValue &&
                    previousGeneratedByClubNumber.TryGetValue(r.ClubNumber.Value, out var prevGenerated))
                {
                    r.NevBrooksSecondsApplied = prevGenerated;
                    r.NevBrooksSecondsAdjustedTime = r.TotalSeconds - prevGenerated;
                }
                else
                {
                    r.NevBrooksSecondsApplied = null;
                    r.NevBrooksSecondsAdjustedTime = null;
                }

                // --- RULE: Shortened Ten → DO NOT update handicap baseline ---
                if (!IsShortened(r) &&
                    r.ClubNumber.HasValue &&
                    r.NevBrooksSecondsGenerated.HasValue)
                {
                    var clubNumber = r.ClubNumber.Value;
                    var newHandicap = r.NevBrooksSecondsGenerated.Value;

                    if (previousGeneratedByClubNumber.TryGetValue(clubNumber, out var oldHandicap))
                    {
                        if (newHandicap < oldHandicap)
                        {
                            previousGeneratedByClubNumber[clubNumber] = newHandicap;
                        }
                    }
                    else
                    {
                        previousGeneratedByClubNumber[clubNumber] = newHandicap;
                    }
                }
            }

            var scoredRides = eventRides
                .Where(r => r.NevBrooksSecondsAdjustedTime.HasValue)
                .ToList();

            return ApplyScores(eventNumber, scoredRides, pointsForPosition);
        }
    }
}
