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

            // If no ten‑mile rides, mark as null (n/a)
            double? best8TenMile = best8TenMileRides.Any()
                ? best8TenMileRides.Sum(r => r.JuvenilesPoints)
                : (double?)null;

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

                // All events view
                AllEvents = new CompetitionScore
                {
                    Points = validRides.Sum(r => r.JuvenilesPoints),
                    Rides = validRides
                },

                // Ten‑mile competition view
                TenMileCompetition = new CompetitionScore
                {
                    Points = best8TenMile,
                    Rides = best8TenMileRides
                },

                // Full competition view
                FullCompetition = new CompetitionScore
                {
                    Points = scoring11,
                    Rides = scoring11Rides
                }
            };
        }

        private static void AssignRanksByScore(
            List<CompetitorResult> results,
            Func<CompetitorResult, double?> scoreSelector,
            Action<CompetitorResult, int?> assignRank)
        {
            if (assignRank is null) throw new ArgumentNullException(nameof(assignRank));

            int currentRank = 1;
            double? lastScore = null;
            int? lastAssignedRank = null;

            for (int i = 0; i < results.Count; i++)
            {
                var score = scoreSelector(results[i]);

                if (score == null)
                {
                    assignRank(results[i], null);
                    continue;
                }

                if (lastScore != null && score == lastScore)
                {
                    // tie: reuse last assigned rank
                    assignRank(results[i], lastAssignedRank);
                }
                else
                {
                    assignRank(results[i], currentRank);
                    lastAssignedRank = currentRank;
                }

                lastScore = score;
                currentRank++;
            }
        }

        public static IList<CompetitorResult> SortResults(IList<CompetitorResult> results)
        {
            // 1. All-events ordering + ranks
            var allEventsOrdered = OrderByAllEvents(results).ToList();
            AssignRanksByScore(allEventsOrdered,
                r => r.AllEvents.Points,
                (r, rank) => r.AllEvents.Rank = rank);

            // 2. Ten-mile ordering + ranks
            var tensOrdered = OrderByTenMile(results).ToList();
            AssignRanksByScore(tensOrdered,
                r => r.TenMileCompetition.Points,
                (r, rank) => r.TenMileCompetition.Rank = rank);

            // 3. Full competition ordering + ranks
            var fullOrdered = OrderByFull(results).ToList();
            AssignRanksByScore(fullOrdered,
                r => r.FullCompetition.Points,
                (r, rank) => r.FullCompetition.Rank = rank);

            // 4. Return final order for rendering (Full competition)
            return fullOrdered;
        }

        private static IEnumerable<CompetitorResult> OrderByAllEvents(IEnumerable<CompetitorResult> results) =>
            results.OrderByDescending(r => r.AllEvents.Points.HasValue)
                   .ThenByDescending(r => r.AllEvents.Points)
                   .ThenBy(r => r.Competitor.Surname)
                   .ThenBy(r => r.Competitor.GivenName);

        private static IEnumerable<CompetitorResult> OrderByTenMile(IEnumerable<CompetitorResult> results) =>
            results.OrderByDescending(r => r.TenMileCompetition.Points.HasValue)
                   .ThenByDescending(r => r.TenMileCompetition.Points)
                   .ThenBy(r => r.Competitor.Surname)
                   .ThenBy(r => r.Competitor.GivenName);

        private static IEnumerable<CompetitorResult> OrderByFull(IEnumerable<CompetitorResult> results) =>
            results
                // First: eligible competitors (those with a full score) come before ineligible
                .OrderByDescending(r => r.FullCompetition.Points.HasValue)
                // Then: order by full competition points (for eligible competitors)
                .ThenByDescending(r => r.FullCompetition.Points)
                // For ineligible competitors, use ten-mile points as a secondary key
                .ThenByDescending(r => r.TenMileCompetition.Points.HasValue)
                .ThenByDescending(r => r.TenMileCompetition.Points)
                // Finally: stable tie-break by name
                .ThenBy(r => r.Competitor.Surname)
                .ThenBy(r => r.Competitor.GivenName);
    }
}
