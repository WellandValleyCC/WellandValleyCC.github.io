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
                .Where(r => r.Eligibility == RideEligibility.Valid && r.IsRoadBike)
                .OrderBy(r => r.TotalSeconds)
                .ToList();

            int lastRank = 0;
            double? lastTime = null;

            for (int i = 0; i < eligible.Count; i++)
            {
                var current = eligible[i];
                var time = current.TotalSeconds;

                int rank;
                if (i == 0)
                {
                    rank = 1;
                }
                else if (Nullable.Equals(time, lastTime))
                {
                    rank = lastRank;
                }
                else
                {
                    rank = i + 1;
                }

                current.EventRoadBikeRank = rank;
                lastRank = rank;
                lastTime = time;
            }

            return eligible.Count;
        }

        public int ProcessEvent(int eventNumber, List<Ride> eventRides)
        {
            return AssignRanks(eventNumber, eventRides);
        }
    }
}