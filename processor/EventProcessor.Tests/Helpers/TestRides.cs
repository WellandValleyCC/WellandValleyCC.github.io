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
            rides.Add(RideFactory.CreateClubMemberRide(1, 1001, "Mia Bates", totalSeconds: 900, isRoadBike: true));    // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1002, "Isla Carson", totalSeconds: 920, isRoadBike: false));  // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1011, "Liam Evans", totalSeconds: 890, isRoadBike: true));   // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1021, "Amelia Hughes", totalSeconds: 940, isRoadBike: false)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1031, "Oliver King", totalSeconds: 880, isRoadBike: true));  // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1032, "Harry Lewis", totalSeconds: 895, isRoadBike: false));  // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1041, "Charlotte Nash", totalSeconds: 850, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1042, "Emily Owens", totalSeconds: 860, isRoadBike: false));   // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1051, "James Quinn", totalSeconds: 830, isRoadBike: true)); // Sen M RB
            rides.Add(RideFactory.CreateClubMemberRide(1, 1052, "Thomas Reid", totalSeconds: 845, isRoadBike: false));   // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1071, "Peter Walker", totalSeconds: 930, isRoadBike: true));  // Vet M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1062, "Alison Underwood", totalSeconds: 995, isRoadBike: false)); // Vet F

            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(1, 2001, "Maya Abbott", totalSeconds: 910, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2011, "Leo Dixon", totalSeconds: 905, isRoadBike: false));  // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2021, "Chloe Griffin", totalSeconds: 935, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2031, "Freddie Johnson", totalSeconds: 885, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2041, "Eva Matthews", totalSeconds: 870, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2051, "Samuel Patel", totalSeconds: 860, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2061, "Beth Taylor", totalSeconds: 990, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2071, "Adam White", totalSeconds: 975, isRoadBike: false)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(1, 3001, "Tia Bennett", totalSeconds: 915, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3011, "Jay Ellis", totalSeconds: 900, isRoadBike: false)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3021, "Zara Hayes", totalSeconds: 940, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3031, "Reece Kirk", totalSeconds: 890, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3041, "Ella Norris", totalSeconds: 875, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3051, "Aaron Quincy", totalSeconds: 865, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3061, "Diana Thompson", totalSeconds: 985, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3071, "Graham Watson", totalSeconds: 970, isRoadBike: false)); // Vet M

            // Event 2
            rides.Add(RideFactory.CreateClubMemberRide(2, 1003, "Zoe Dennison", totalSeconds: 940, isRoadBike: true));  // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1012, "Noah Fletcher", totalSeconds: 955, isRoadBike: false)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1013, "Ethan Graham", totalSeconds: 970, isRoadBike: true));  // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1022, "Sophie Irwin", totalSeconds: 900, isRoadBike: true)); // Jun F RB
            rides.Add(RideFactory.CreateClubMemberRide(2, 1023, "Grace Jackson", totalSeconds: 915, isRoadBike: false)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1043, "Lucy Price", totalSeconds: 840, isRoadBike: true)); // Sen F RB
            rides.Add(RideFactory.CreateClubMemberRide(2, 1053, "Daniel Shaw", totalSeconds: 855, isRoadBike: false));  // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1061, "Helen Turner", totalSeconds: 980, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1062, "Alison Underwood", totalSeconds: 995, isRoadBike: false)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1072, "Martin Xavier", totalSeconds: 965, isRoadBike: true)); // Vet M RB
            rides.Add(RideFactory.CreateClubMemberRide(2, 1051, "James Quinn", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNS)); // Sen M DNS

            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(2, 2002, "Ella Barker", totalSeconds: 925, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2012, "Oscar Edwards", totalSeconds: 940, isRoadBike: false)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2022, "Millie Harrison", totalSeconds: 920, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2032, "Archie Kerr", totalSeconds: 890, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2042, "Rosie Nelson", totalSeconds: 860, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2052, "Jacob Roberts", totalSeconds: 850, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2062, "Rachel Upton", totalSeconds: 1000, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2072, "Nathan Zane", totalSeconds: 980, isRoadBike: false)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(2, 3002, "Nina Chapman", totalSeconds: 930, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3012, "Max Franklin", totalSeconds: 915, isRoadBike: false)); // Juv M
                                                                                                                        // Honorary additions (continued)
            rides.Add(RideFactory.CreateClubMemberRide(2, 3022, "Megan Irving", totalSeconds: 925, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3032, "Tyler Lloyd", totalSeconds: 895, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(2, 3042, "Katie Olsen", totalSeconds: 855, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3052, "Charlie Robinson", totalSeconds: 845, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 3062, "Fiona Ursula", totalSeconds: 995, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3072, "Trevor York", totalSeconds: 975, isRoadBike: false)); // Vet M

            // Event 3
            rides.Add(RideFactory.CreateClubMemberRide(3, 1013, "Ethan Graham", totalSeconds: 980, isRoadBike: true)); // Juv M RB
            rides.Add(RideFactory.CreateClubMemberRide(3, 1033, "Jack Mason", totalSeconds: 910, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1031, "Oliver King", totalSeconds: 905, isRoadBike: true)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1041, "Charlotte Nash", totalSeconds: 870, isRoadBike: false)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1043, "Lucy Price", totalSeconds: 860, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1063, "Janet Vaughn", totalSeconds: 1020, isRoadBike: false)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1071, "Peter Walker", totalSeconds: 990, isRoadBike: true)); // Vet M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1073, "Colin Young", totalSeconds: 1005, isRoadBike: false)); // Vet M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1052, "Thomas Reid", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNF)); // Sen M DNF

            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(3, 2003, "Ruby Carter", totalSeconds: 935, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2013, "George Foster", totalSeconds: 950, isRoadBike: false)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2023, "Lily Ingram", totalSeconds: 910, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2033, "Henry Lawson", totalSeconds: 880, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2043, "Hannah O'Brien", totalSeconds: 870, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2053, "Logan Simpson", totalSeconds: 860, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2063, "Claire Vincent", totalSeconds: 1010, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2073, "Luke Adams", totalSeconds: 995, isRoadBike: false)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(3, 3003, "Leah Davies", totalSeconds: 940, isRoadBike: true)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3013, "Ben Gibson", totalSeconds: 925, isRoadBike: false)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3023, "Amber Jennings", totalSeconds: 920, isRoadBike: true)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3033, "Ryan Mitchell", totalSeconds: 890, isRoadBike: false)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3043, "Georgia Parker", totalSeconds: 865, isRoadBike: true)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3053, "Joseph Stevens", totalSeconds: 855, isRoadBike: false)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3063, "Paula Valentine", totalSeconds: 1025, isRoadBike: true)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3073, "Derek Zimmer", totalSeconds: 1000, isRoadBike: false)); // Vet M

            // Append a deterministic random number (0..3) of guest riders to each event
            AppendGuestsForEvent(rides, eventNumber: 1, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 2, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 3, guestCount: Rng.Next(0, 4));

            // Event 99: future-dated riders (CreatedUtc = 50 days from now)
            rides.Add(RideFactory.CreateClubMemberRide(99, 4001, "Sylvie Harper", totalSeconds: 910, isRoadBike: true)); // Juv F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4002, "Damon Cross", totalSeconds: 905, isRoadBike: false));   // Juv M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4003, "Tessa Langford", totalSeconds: 930, isRoadBike: true)); // Jun F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4004, "Ronan Blake", totalSeconds: 920, isRoadBike: false));   // Jun M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4005, "Imogen Frost", totalSeconds: 880, isRoadBike: true));  // Sen F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4006, "Callum Drake", totalSeconds: 870, isRoadBike: false));  // Sen M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4007, "Freya Winslow", totalSeconds: 990, isRoadBike: true)); // Vet F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4008, "Jasper Thorne", totalSeconds: 975, isRoadBike: false)); // Vet M FirstClaim

            rides.Add(RideFactory.CreateClubMemberRide(99, 4011, "Elodie Marsh", totalSeconds: 915, isRoadBike: true));  // Juv F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4012, "Kieran Holt", totalSeconds: 910, isRoadBike: false));   // Juv M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4013, "Lara Bishop", totalSeconds: 935, isRoadBike: true));   // Jun F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4014, "Elliot Grayson", totalSeconds: 925, isRoadBike: false)); // Jun M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4015, "Isabel Marlow", totalSeconds: 870, isRoadBike: true)); // Sen F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4016, "Miles Sutton", totalSeconds: 860, isRoadBike: false));  // Sen M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4017, "Clara Harrington", totalSeconds: 1000, isRoadBike: true)); // Vet F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4018, "Reed Dalton", totalSeconds: 985, isRoadBike: false));   // Vet M SecondClaim

            rides.Add(RideFactory.CreateClubMemberRide(99, 4021, "Nina Prescott", totalSeconds: 920, isRoadBike: true)); // Juv F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4022, "Theo Bennett", totalSeconds: 915, isRoadBike: false)); // Juv M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4023, "Maisie Whitaker", totalSeconds: 940, isRoadBike: true)); // Jun F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4024, "Jude Hawthorne", totalSeconds: 930, isRoadBike: false)); // Jun M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4025, "Poppy Ellington", totalSeconds: 875, isRoadBike: true)); // Sen F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4026, "Finn Radcliffe", totalSeconds: 865, isRoadBike: false)); // Sen M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4027, "Lillian Ashford", totalSeconds: 1020, isRoadBike: true)); // Vet F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4028, "Rowan Tanner", totalSeconds: 1005, isRoadBike: false)); // Vet M Honorary

            // Event 4: All new Veterans (5001–5040 male, 5101–5140 female)
            // Deterministic spread: 1770–1830 seconds (60s band around 1800)

            for (int i = 0; i < 40; i++)
            {
                int clubNumber = 5001 + i;
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (i % 61); // deterministic spread
                bool isRoadBike = (i % 2 == 0);

                rides.Add(RideFactory.CreateClubMemberRide(
                    4,
                    clubNumber,
                    $"{competitor.FullName}",
                    totalSeconds,
                    isRoadBike));
            }

            for (int i = 0; i < 40; i++)
            {
                int clubNumber = 5101 + i;
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + ((i + 20) % 61); // offset to mix distribution
                bool isRoadBike = (i % 2 != 0);

                rides.Add(RideFactory.CreateClubMemberRide(
                    4,
                    clubNumber,
                    $"{competitor.FullName}",
                    totalSeconds,
                    isRoadBike));
            }

            // Event 4: Add a few non-Veterans for coverage
            // Juveniles
            {
                int clubNumber = 1001; // Mia Bates
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = true;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

            {
                int clubNumber = 1011; // Liam Evans
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = false;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

            // Juniors
            {
                int clubNumber = 1021; // Amelia Hughes
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = true;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

            {
                int clubNumber = 1031; // Oliver King
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = false;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

            // Seniors
            {
                int clubNumber = 1041; // Charlotte Nash
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = true;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

            {
                int clubNumber = 1051; // James Quinn
                var competitor = TestCompetitors.All.First(c => c.ClubNumber == clubNumber);
                int totalSeconds = 1770 + (clubNumber % 61);
                bool isRoadBike = false;
                rides.Add(RideFactory.CreateClubMemberRide(
                    4, clubNumber, $"{competitor.FullName}", totalSeconds, isRoadBike));
            }

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

                rides.Add(RideFactory.CreateGuestRide(
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
