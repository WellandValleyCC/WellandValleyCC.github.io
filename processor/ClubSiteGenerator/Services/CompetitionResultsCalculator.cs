using ClubCore.Models;
using ClubSiteGenerator.Models;

namespace ClubSiteGenerator.Services
{
    public static class CompetitionResultsCalculator
    {
        /// <summary>
        /// Builds a CompetitorResult for a single competitor group,
        /// calculating Best‑8 Ten‑mile and Scoring‑11 totals.
        /// </summary>
        public static CompetitorResult BuildCompetitorResult(
            IGrouping<Competitor, Ride> group,
            IEnumerable<CalendarEvent> calendar)
        {
            // Best 8 ten‑mile rides
            var best8TenMileRides = group
                .Where(r => calendar.First(ev => ev.EventNumber == r.EventNumber).IsEvening10)
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(8)
                .ToList();

            var best8TenMile = best8TenMileRides.Sum(r => r.JuvenilesPoints);

            // Best 2 non‑10 rides
            var nonTenMileBest2Rides = group
                .Where(r => !calendar.Single(ev => ev.EventNumber == r.EventNumber).IsEvening10)
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(2)
                .ToList();

            var nonTenMileBest2 = nonTenMileBest2Rides.Sum(r => r.JuvenilesPoints);
            var consumedEventNumbers = nonTenMileBest2Rides.Select(r => r.EventNumber).ToHashSet();

            // Best 9 of remaining rides
            var remainingBest9Rides = group
                .Where(r => !consumedEventNumbers.Contains(r.EventNumber))
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(9)
                .ToList();

            var remainingBest9 = remainingBest9Rides.Sum(r => r.JuvenilesPoints);

            // Combine into Scoring‑11
            var scoring11Rides = nonTenMileBest2Rides.Concat(remainingBest9Rides).ToList();
            var scoring11 = nonTenMileBest2 + remainingBest9;

            return new CompetitorResult
            {
                Competitor = group.Key,
                Rides = group.ToList(),
                Best8TenMile = best8TenMile,
                Best8TenMileRides = best8TenMileRides,
                Scoring11 = scoring11,
                Scoring11Rides = scoring11Rides
            };
        }
    }
}
