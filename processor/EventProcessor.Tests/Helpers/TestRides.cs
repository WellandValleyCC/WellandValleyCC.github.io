using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventProcessor.Tests.Helpers
{
    public static class TestRides
    {
        // Seeded Random so test data is deterministic across runs
        private static readonly Random Rng = new Random(12345);

        // Helper to pick competitors by ClubNumber from TestCompetitors.All
        private static Competitor? FindCompetitor(int clubNumber) =>
            TestCompetitors.All.FirstOrDefault(c => c.ClubNumber == clubNumber);

        // Build a base list of rides per event, then append a random number of guest riders (0..3)
        public static readonly IReadOnlyList<Ride> All = BuildAll();

        private static IReadOnlyList<Ride> BuildAll()
        {
            var rides = new List<Ride>();

            // Event 1: mix of competitors (0/1/2 per category) - deterministic choices
            rides.Add(RideFactory.CreateClubMember(1, 1001, "Mia Bates", totalSeconds: 900));    // Juv F
            rides.Add(RideFactory.CreateClubMember(1, 1002, "Isla Carson", totalSeconds: 920));  // Juv F
            rides.Add(RideFactory.CreateClubMember(1, 1011, "Liam Evans", totalSeconds: 890));   // Juv M
            rides.Add(RideFactory.CreateClubMember(1, 1021, "Amelia Hughes", totalSeconds: 940)); // Jun F
            rides.Add(RideFactory.CreateClubMember(1, 1031, "Oliver King", totalSeconds: 880));  // Jun M
            rides.Add(RideFactory.CreateClubMember(1, 1032, "Harry Lewis", totalSeconds: 895));  // Jun M
            rides.Add(RideFactory.CreateClubMember(1, 1041, "Charlotte Nash", totalSeconds: 850)); // Sen F
            rides.Add(RideFactory.CreateClubMember(1, 1042, "Emily Owens", totalSeconds: 860));   // Sen F
            rides.Add(RideFactory.CreateClubMember(1, 1051, "James Quinn", totalSeconds: 830, isRoadBike: true)); // Sen M RB
            rides.Add(RideFactory.CreateClubMember(1, 1052, "Thomas Reid", totalSeconds: 845));   // Sen M
            rides.Add(RideFactory.CreateClubMember(1, 1071, "Peter Walker", totalSeconds: 930));  // Vet M

            // Event 2
            rides.Add(RideFactory.CreateClubMember(2, 1003, "Zoe Dennison", totalSeconds: 940));  // Juv F
            rides.Add(RideFactory.CreateClubMember(2, 1012, "Noah Fletcher", totalSeconds: 955)); // Juv M
            rides.Add(RideFactory.CreateClubMember(2, 1013, "Ethan Graham", totalSeconds: 970));  // Juv M
            rides.Add(RideFactory.CreateClubMember(2, 1022, "Sophie Irwin", totalSeconds: 900, isRoadBike: true)); // Jun F RB
            rides.Add(RideFactory.CreateClubMember(2, 1023, "Grace Jackson", totalSeconds: 915)); // Jun F
            rides.Add(RideFactory.CreateClubMember(2, 1043, "Lucy Price", totalSeconds: 840, isRoadBike: true)); // Sen F RB
            rides.Add(RideFactory.CreateClubMember(2, 1053, "Daniel Shaw", totalSeconds: 855));  // Sen M
            rides.Add(RideFactory.CreateClubMember(2, 1061, "Helen Turner", totalSeconds: 980)); // Vet F
            rides.Add(RideFactory.CreateClubMember(2, 1062, "Alison Underwood", totalSeconds: 995)); // Vet F
            rides.Add(RideFactory.CreateClubMember(2, 1072, "Martin Xavier", totalSeconds: 965, isRoadBike: true)); // Vet M RB
            // One DNS to test non-valid eligibility
            rides.Add(RideFactory.CreateClubMember(2, 1051, "James Quinn", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNS));

            // Event 3
            rides.Add(RideFactory.CreateClubMember(3, 1013, "Ethan Graham", totalSeconds: 980, isRoadBike: true)); // Juv M RB
            rides.Add(RideFactory.CreateClubMember(3, 1033, "Jack Mason", totalSeconds: 910)); // Jun M
            rides.Add(RideFactory.CreateClubMember(3, 1031, "Oliver King", totalSeconds: 905)); // Jun M
            rides.Add(RideFactory.CreateClubMember(3, 1041, "Charlotte Nash", totalSeconds: 870)); // Sen F
            rides.Add(RideFactory.CreateClubMember(3, 1043, "Lucy Price", totalSeconds: 860)); // Sen F
            rides.Add(RideFactory.CreateClubMember(3, 1063, "Janet Vaughn", totalSeconds: 1020)); // Vet F
            rides.Add(RideFactory.CreateClubMember(3, 1071, "Peter Walker", totalSeconds: 990)); // Vet M
            rides.Add(RideFactory.CreateClubMember(3, 1073, "Colin Young", totalSeconds: 1005)); // Vet M
            // One DNF to test non-valid eligibility
            rides.Add(RideFactory.CreateClubMember(3, 1052, "Thomas Reid", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNF));

            // Append a deterministic random number (0..3) of guest riders to each event
            AppendGuestsForEvent(rides, eventNumber: 1, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 2, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 3, guestCount: Rng.Next(0, 4));

            return rides.AsReadOnly();
        }

        private static void AppendGuestsForEvent(List<Ride> rides, int eventNumber, int guestCount)
        {
            for (int i = 0; i < guestCount; i++)
            {
                var guestName = $"Guest_{eventNumber}_{i + 1}";
                // Vary total seconds slightly and roadbike flag deterministically
                var totalSeconds = 880 + (eventNumber * 10) + (i * 7);
                var isRoadBike = ((eventNumber + i) % 3 == 0);

                rides.Add(RideFactory.CreateGuest(
                    eventNumber: eventNumber,
                    guestName: guestName,
                    totalSeconds: totalSeconds,
                    isRoadBike: isRoadBike,
                    eligibility: RideEligibility.Valid,
                    actualTime: TimeSpan.FromSeconds(totalSeconds).ToString(@"hh\:mm\:ss")));
            }
        }
    }
}
