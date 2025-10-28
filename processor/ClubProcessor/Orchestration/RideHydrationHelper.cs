using ClubProcessor.Models;

namespace ClubProcessor.Orchestration
{
    public static class RideHydrationHelper
    {
        public static void HydrateCompetitors(List<Ride> rides, List<Competitor> competitors)
        {
            var lookup = competitors.ToDictionary(c => c.ClubNumber);

            var missing = new List<int>();

            foreach (var ride in rides)
            {
                if (ride.ClubNumber.HasValue && lookup.TryGetValue(ride.ClubNumber.Value, out var competitor))
                {
                    ride.Competitor = competitor;
                }
                else if (ride.ClubNumber.HasValue)
                {
                    missing.Add(ride.ClubNumber.Value);
                }
            }

            if (missing.Any())
            {
                Console.WriteLine("[ERROR] The following ClubNumbers were found in rides but missing from the competitor list:");
                foreach (var clubNumber in missing.Distinct())
                {
                    Console.WriteLine($"  - ClubNumber {clubNumber}");
                }

                throw new InvalidOperationException("Scoring aborted: missing competitors detected. Please check the membership list.");
            }
        }
    }
}
