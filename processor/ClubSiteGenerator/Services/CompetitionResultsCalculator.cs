using ClubCore.Models;
using ClubCore.Models.Enums;
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
            // Precompute lookup for event type
            var isTenMileByEvent = calendar
                .ToDictionary(ev => ev.EventNumber, ev => ev.IsEvening10);

            // Only valid rides contribute to scoring
            var validRides = group
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Best 8 ten‑mile rides (valid only)
            var best8TenMileRides = validRides
                .Where(r => isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && isTen)
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(8)
                .ToList();

            var best8TenMile = best8TenMileRides.Sum(r => r.JuvenilesPoints);

            // Best 2 non‑ten rides (valid only)
            var nonTenMileBest2Rides = validRides
                .Where(r => isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && !isTen)
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(2)
                .ToList();

            var nonTenMileBest2 = nonTenMileBest2Rides.Sum(r => r.JuvenilesPoints);
            var consumedEventNumbers = nonTenMileBest2Rides.Select(r => r.EventNumber).ToHashSet();

            // Best 9 of remaining valid rides (excluding the 2 non‑ten already consumed)
            var remainingBest9Rides = validRides
                .Where(r => !consumedEventNumbers.Contains(r.EventNumber))
                .OrderByDescending(r => r.JuvenilesPoints)
                .Take(9)
                .ToList();

            var remainingBest9 = remainingBest9Rides.Sum(r => r.JuvenilesPoints);

            // Combine into Scoring‑11 (only if 2 valid non‑ten exist)
            var scoring11Rides = nonTenMileBest2Rides.Concat(remainingBest9Rides).ToList();
            var scoring11 = nonTenMileBest2Rides.Count == 2 ? nonTenMileBest2 + remainingBest9 : (double?)null;

            // Per‑event data for rendering
            var eventPoints = group.ToDictionary(
                r => r.EventNumber,
                r => r.Status == RideStatus.Valid ? r.JuvenilesPoints : null
            );

            var eventStatuses = group.ToDictionary(
                r => r.EventNumber,
                r => r.Status
            );

            return new CompetitorResult
            {
                Competitor = group.Key,
                Rides = group.ToList(),
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,
                // Count only valid rides as completed
                EventsCompleted = validRides.Count,
                TenMileCompetitionPoints = best8TenMile,
                TenMileCompetitionRides = best8TenMileRides,
                FullCompetitionPoints = scoring11,
                FullCompetitionRides = scoring11Rides
            };
        }

        /// <summary>
        /// Assigns ranks to a list of results, tie‑aware, stopping once Scoring11 is null.
        /// </summary>
        public static void AssignRanks(List<CompetitorResult> results)
        {
            int currentRank = 1;
            double? lastScore = null;

            for (int i = 0; i < results.Count; i++)
            {
                var score = results[i].FullCompetitionPoints;

                if (score == null)
                {
                    results[i].FullCompetitionRank = null;
                    continue;
                }

                if (lastScore != null && score == lastScore)
                {
                    results[i].FullCompetitionRank = results[i - 1].FullCompetitionRank;
                }
                else
                {
                    results[i].FullCompetitionRank = currentRank;
                }

                lastScore = score;
                currentRank++;
            }
        }

        /// <summary>
        /// Sorts results: Scoring11 first (desc), then Best8TenMile (desc), then surname/given name.
        /// </summary>
        public static IEnumerable<CompetitorResult> SortResults(IEnumerable<CompetitorResult> results)
        {
            return results
                .OrderByDescending(r => r.FullCompetitionPoints.HasValue)
                .ThenByDescending(r => r.FullCompetitionPoints)
                .ThenByDescending(r => r.TenMileCompetitionPoints.HasValue)
                .ThenByDescending(r => r.TenMileCompetitionPoints)
                .ThenBy(r => r.Competitor.Surname)
                .ThenBy(r => r.Competitor.GivenName);
        }
    }
}
