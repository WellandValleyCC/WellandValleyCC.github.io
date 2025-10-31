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
    internal class RoadBikeEventRankCalculator : IEventRankCalculator, IRideProcessor
    {
        public int AssignRanks(int eventNumber, List<Ride> rides)
        {
            var eligible = rides
                .Where(r => r.Eligibility == RideEligibility.Valid && r.IsRoadBike == true)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            for (int i = 0; i < eligible.Count; i++)
            {
                eligible[i].EventRank = i + 1;
            }

            return eligible.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return AssignRanks(eventNumber, eventRides);
        }
    }
}