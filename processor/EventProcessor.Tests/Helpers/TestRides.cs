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
            rides.Add(RideFactory.CreateClubMemberRide(1, 1001, "Mia Bates", totalSeconds: 900));    // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1002, "Isla Carson", totalSeconds: 920));  // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1011, "Liam Evans", totalSeconds: 890));   // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1021, "Amelia Hughes", totalSeconds: 940)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1031, "Oliver King", totalSeconds: 880));  // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1032, "Harry Lewis", totalSeconds: 895));  // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1041, "Charlotte Nash", totalSeconds: 850)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1042, "Emily Owens", totalSeconds: 860));   // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 1051, "James Quinn", totalSeconds: 830, isRoadBike: true)); // Sen M RB
            rides.Add(RideFactory.CreateClubMemberRide(1, 1052, "Thomas Reid", totalSeconds: 845));   // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 1071, "Peter Walker", totalSeconds: 930));  // Vet M
            
            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(1, 2001, "Maya Abbott", totalSeconds: 910)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2011, "Leo Dixon", totalSeconds: 905));  // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2021, "Chloe Griffin", totalSeconds: 935)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2031, "Freddie Johnson", totalSeconds: 885)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2041, "Eva Matthews", totalSeconds: 870)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2051, "Samuel Patel", totalSeconds: 860)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 2061, "Beth Taylor", totalSeconds: 990)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(1, 2071, "Adam White", totalSeconds: 975)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(1, 3001, "Tia Bennett", totalSeconds: 915)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3011, "Jay Ellis", totalSeconds: 900)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3021, "Zara Hayes", totalSeconds: 940)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3031, "Reece Kirk", totalSeconds: 890)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3041, "Ella Norris", totalSeconds: 875)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3051, "Aaron Quincy", totalSeconds: 865)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(1, 3061, "Diana Thompson", totalSeconds: 985)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(1, 3071, "Graham Watson", totalSeconds: 970)); // Vet M

            // Event 2
            rides.Add(RideFactory.CreateClubMemberRide(2, 1003, "Zoe Dennison", totalSeconds: 940));  // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1012, "Noah Fletcher", totalSeconds: 955)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1013, "Ethan Graham", totalSeconds: 970));  // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1022, "Sophie Irwin", totalSeconds: 900, isRoadBike: true)); // Jun F RB
            rides.Add(RideFactory.CreateClubMemberRide(2, 1023, "Grace Jackson", totalSeconds: 915)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1043, "Lucy Price", totalSeconds: 840, isRoadBike: true)); // Sen F RB
            rides.Add(RideFactory.CreateClubMemberRide(2, 1053, "Daniel Shaw", totalSeconds: 855));  // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 1061, "Helen Turner", totalSeconds: 980)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1062, "Alison Underwood", totalSeconds: 995)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 1072, "Martin Xavier", totalSeconds: 965, isRoadBike: true)); // Vet M RB
            // One DNS to test non-valid eligibility
            rides.Add(RideFactory.CreateClubMemberRide(2, 1051, "James Quinn", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNS));
            
            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(2, 2002, "Ella Barker", totalSeconds: 925)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2012, "Oscar Edwards", totalSeconds: 940)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2022, "Millie Harrison", totalSeconds: 920)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2032, "Archie Kerr", totalSeconds: 890)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2042, "Rosie Nelson", totalSeconds: 860)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2052, "Jacob Roberts", totalSeconds: 850)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 2062, "Rachel Upton", totalSeconds: 1000)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 2072, "Nathan Zane", totalSeconds: 980)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(2, 3002, "Nina Chapman", totalSeconds: 930)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3012, "Max Franklin", totalSeconds: 915)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(2, 3022, "Megan Irving", totalSeconds: 925)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3032, "Tyler Lloyd", totalSeconds: 895)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(2, 3042, "Katie Olsen", totalSeconds: 855)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3052, "Charlie Robinson", totalSeconds: 845)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(2, 3062, "Fiona Ursula", totalSeconds: 995)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(2, 3072, "Trevor York", totalSeconds: 975)); // Vet M

            // Event 3
            rides.Add(RideFactory.CreateClubMemberRide(3, 1013, "Ethan Graham", totalSeconds: 980, isRoadBike: true)); // Juv M RB
            rides.Add(RideFactory.CreateClubMemberRide(3, 1033, "Jack Mason", totalSeconds: 910)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1031, "Oliver King", totalSeconds: 905)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1041, "Charlotte Nash", totalSeconds: 870)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1043, "Lucy Price", totalSeconds: 860)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1063, "Janet Vaughn", totalSeconds: 1020)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 1071, "Peter Walker", totalSeconds: 990)); // Vet M
            rides.Add(RideFactory.CreateClubMemberRide(3, 1073, "Colin Young", totalSeconds: 1005)); // Vet M
            // One DNF to test non-valid eligibility
            rides.Add(RideFactory.CreateClubMemberRide(3, 1052, "Thomas Reid", totalSeconds: 0, isRoadBike: false, eligibility: RideEligibility.DNF));

            // SecondClaim additions
            rides.Add(RideFactory.CreateClubMemberRide(3, 2003, "Ruby Carter", totalSeconds: 935)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2013, "George Foster", totalSeconds: 950)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2023, "Lily Ingram", totalSeconds: 910)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2033, "Henry Lawson", totalSeconds: 880)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2043, "Hannah O'Brien", totalSeconds: 870)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2053, "Logan Simpson", totalSeconds: 860)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(3, 2063, "Claire Vincent", totalSeconds: 1010)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 2073, "Luke Adams", totalSeconds: 995)); // Vet M

            // Honorary additions
            rides.Add(RideFactory.CreateClubMemberRide(3, 3003, "Leah Davies", totalSeconds: 940)); // Juv F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3013, "Ben Gibson", totalSeconds: 925)); // Juv M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3023, "Amber Jennings", totalSeconds: 920)); // Jun F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3033, "Ryan Mitchell", totalSeconds: 890)); // Jun M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3043, "Georgia Parker", totalSeconds: 865)); // Sen F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3053, "Joseph Stevens", totalSeconds: 855)); // Sen M
            rides.Add(RideFactory.CreateClubMemberRide(3, 3063, "Paula Valentine", totalSeconds: 1025)); // Vet F
            rides.Add(RideFactory.CreateClubMemberRide(3, 3073, "Derek Zimmer", totalSeconds: 1000)); // Vet M

            // Append a deterministic random number (0..3) of guest riders to each event
            AppendGuestsForEvent(rides, eventNumber: 1, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 2, guestCount: Rng.Next(0, 4));
            AppendGuestsForEvent(rides, eventNumber: 3, guestCount: Rng.Next(0, 4));

            // Event 99: future-dated riders (CreatedUtc = 50 days from now)
            rides.Add(RideFactory.CreateClubMemberRide(99, 4001, "Sylvie Harper", totalSeconds: 910)); // Juv F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4002, "Damon Cross", totalSeconds: 905));   // Juv M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4003, "Tessa Langford", totalSeconds: 930)); // Jun F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4004, "Ronan Blake", totalSeconds: 920));   // Jun M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4005, "Imogen Frost", totalSeconds: 880));  // Sen F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4006, "Callum Drake", totalSeconds: 870));  // Sen M FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4007, "Freya Winslow", totalSeconds: 990)); // Vet F FirstClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4008, "Jasper Thorne", totalSeconds: 975)); // Vet M FirstClaim

            rides.Add(RideFactory.CreateClubMemberRide(99, 4011, "Elodie Marsh", totalSeconds: 915));  // Juv F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4012, "Kieran Holt", totalSeconds: 910));   // Juv M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4013, "Lara Bishop", totalSeconds: 935));   // Jun F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4014, "Elliot Grayson", totalSeconds: 925)); // Jun M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4015, "Isabel Marlow", totalSeconds: 870)); // Sen F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4016, "Miles Sutton", totalSeconds: 860));  // Sen M SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4017, "Clara Harrington", totalSeconds: 1000)); // Vet F SecondClaim
            rides.Add(RideFactory.CreateClubMemberRide(99, 4018, "Reed Dalton", totalSeconds: 985));   // Vet M SecondClaim

            rides.Add(RideFactory.CreateClubMemberRide(99, 4021, "Nina Prescott", totalSeconds: 920)); // Juv F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4022, "Theo Bennett", totalSeconds: 915));  // Juv M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4023, "Maisie Whitaker", totalSeconds: 940)); // Jun F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4024, "Jude Hawthorne", totalSeconds: 930)); // Jun M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4025, "Poppy Ellington", totalSeconds: 875)); // Sen F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4026, "Finn Radcliffe", totalSeconds: 865)); // Sen M Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4027, "Lillian Ashford", totalSeconds: 1020)); // Vet F Honorary
            rides.Add(RideFactory.CreateClubMemberRide(99, 4028, "Rowan Tanner", totalSeconds: 1005)); // Vet M Honorary


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
