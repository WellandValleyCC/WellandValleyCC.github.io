using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubCore.Utilities;
using ClubSiteGenerator.Models;
using ClubSiteGenerator.Rules;

namespace ClubSiteGenerator.Services
{
    public static class CompetitionResultsCalculator
    {
        /// <summary>
        /// Builds a CompetitorResult for a single competitor's rides,
        /// calculating Best‑8 Ten‑mile and Scoring‑11 totals (actually, year appropriate totals based on supplied rules).
        /// </summary>
        public static CompetitorResult BuildCompetitorResult(
            IReadOnlyList<Ride> rides,
            IEnumerable<CalendarEvent> calendar,
            Func<Ride, double?> pointsSelector,
            ICompetitionRules rules)
        {
            if (rides == null || rides.Count == 0)
                throw new ArgumentException("Rides collection must not be null or empty.", nameof(rides));

            // validate all rides have the same ClubNumber
            var firstClubNumber = rides[0].Competitor?.ClubNumber;
            if (firstClubNumber == null)
                throw new ArgumentException("Rides must be hydrated with Competitors having a ClubNumber.", nameof(rides));

            bool mismatch = rides.Any(r => r.Competitor?.ClubNumber != firstClubNumber);
            if (mismatch)
                throw new ArgumentException("All rides must belong to the same competitor (same ClubNumber).", nameof(rides));

            // Determine competition year from the first event
            var competitionYear = calendar.First().EventDate.Year;

            // Use the last ride's competitor - they are all the same person.  This one
            // has the most up-to-date Competitor data.
            var competitor = rides.Last().Competitor!;

            // Precompute lookup for event type
            var isTenMileByEvent = calendar
                .ToDictionary(ev => ev.EventNumber, ev => ev.IsEvening10);

            // Only valid rides contribute to scoring
            var validRides = rides
                .Where(r => r.Status == RideStatus.Valid)
                .ToList();

            // Split counts: only valid rides
            var eventsCompletedTens = validRides.Count(r =>
                isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && isTen);

            var eventsCompletedOther = validRides.Count(r =>
                isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && !isTen);

            // Ten‑mile rides - e.g. best 8 in 2025
            var bestTenMileRides = validRides
                .Where(r => isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && isTen)
                .OrderByDescending(r => pointsSelector(r) ?? 0)
                .Take(rules.TenMileCount)
                .ToList();

            // If no ten‑mile rides, mark as null (n/a)
            double? bestTenMilePoints = bestTenMileRides.Any()
                ? bestTenMileRides.Sum(r => pointsSelector(r) ?? 0)
                : (double?)null;

            // Best non‑ten rides (valid only)
            // - these are the minimum required to make competitor eligible
            // - e.g. best 2 non-ten in 2025
            var nonTenMileRequiredRides = validRides
                .Where(r => isTenMileByEvent.TryGetValue(r.EventNumber, out var isTen) && !isTen)
                .OrderByDescending(r => pointsSelector(r) ?? 0)
                .Take(rules.NonTenMinimum)
                .ToList();

            var nonTenMileRequiredPoints = nonTenMileRequiredRides.Sum(r => pointsSelector(r) ?? 0);
            var consumedEventNumbers = nonTenMileRequiredRides.Select(r => r.EventNumber).ToHashSet();

            // Best of remaining valid rides to get required scoring limit
            // (excluding the min required non‑ten already consumed)
            // - e.g. best 9 of remaining in 2025 (2 + 9 = 11 scoring rides)
            var remainingBestRides = validRides
                .Where(r => !consumedEventNumbers.Contains(r.EventNumber))
                .OrderByDescending(r => pointsSelector(r) ?? 0)
                .Take(rules.MixedEventCount - rules.NonTenMinimum)
                .ToList();

            var remainingBestRidesPoints = remainingBestRides.Sum(r => pointsSelector(r) ?? 0);

            // Combine into mixed distance score
            var scoringRides = nonTenMileRequiredRides.Concat(remainingBestRides).ToList();
            var scoringPoints = nonTenMileRequiredRides.Count == 2 
                ? nonTenMileRequiredPoints + remainingBestRidesPoints
                : (double?)null;

            // Per‑event data for rendering
            var eventPoints = rides.ToDictionary(
                r => r.EventNumber,
                r => r.Status == RideStatus.Valid ? pointsSelector(r) : null
            );

            var eventStatuses = rides.ToDictionary(
                r => r.EventNumber,
                r => r.Status
            );

            return new CompetitorResult
            {
                Competitor = competitor,
                Rides = rides,
                EventPoints = eventPoints,
                EventStatuses = eventStatuses,

                // Split counts: only valid rides
                EventsCompletedTens = eventsCompletedTens,
                EventsCompletedOther = eventsCompletedOther,

                // All events view
                AllEvents = new CompetitionScore
                {
                    Points = validRides.Sum(r => pointsSelector(r) ?? 0),
                    Rides = validRides
                },

                // Ten‑mile competition view
                TenMileCompetition = new CompetitionScore
                {
                    Points = bestTenMilePoints,
                    Rides =  bestTenMileRides
                },

                // Full competition view
                FullCompetition = new CompetitionScore
                {
                    Points = scoringPoints,
                    Rides =  scoringRides
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
