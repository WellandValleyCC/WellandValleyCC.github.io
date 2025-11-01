using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;

namespace EventProcessor.Tests.Helpers
{
    public static class TestCompetitors
    {
        public static readonly IReadOnlyList<Competitor> All = new List<Competitor>
        {
            // 24 FirstClaim competitors
            CompetitorFactory.Create(1001, "Bates", "Mia", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1002, "Carson", "Isla", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1003, "Dennison", "Zoe", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(1011, "Evans", "Liam", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1012, "Fletcher", "Noah", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1013, "Graham", "Ethan", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(1021, "Hughes", "Amelia", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(1022, "Irwin", "Sophie", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(1023, "Jackson", "Grace", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsJunior),

            CompetitorFactory.Create(1031, "King", "Oliver", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(1032, "Lewis", "Harry", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(1033, "Mason", "Jack", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsJunior),

            CompetitorFactory.Create(1041, "Nash", "Charlotte", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(1042, "Owens", "Emily", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(1043, "Price", "Lucy", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsSenior),

            CompetitorFactory.Create(1051, "Quinn", "James", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(1052, "Reid", "Thomas", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(1053, "Shaw", "Daniel", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsSenior),

            CompetitorFactory.Create(1061, "Turner", "Helen", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(1062, "Underwood", "Alison", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(1063, "Vaughn", "Janet", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.IsVeteran),

            CompetitorFactory.Create(1071, "Walker", "Peter", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(1072, "Xavier", "Martin", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(1073, "Young", "Colin", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.IsVeteran),

            // SecondClaim competitors (24)
            CompetitorFactory.Create(2001, "Abbott", "Maya", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(2002, "Barker", "Ella", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(2003, "Carter", "Ruby", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(2011, "Dixon", "Leo", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(2012, "Edwards", "Oscar", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(2013, "Foster", "George", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(2021, "Griffin", "Chloe", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(2022, "Harrison", "Millie", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(2023, "Ingram", "Lily", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsJunior),

            CompetitorFactory.Create(2031, "Johnson", "Freddie", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(2032, "Kerr", "Archie", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(2033, "Lawson", "Henry", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsJunior),

            CompetitorFactory.Create(2041, "Matthews", "Eva", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(2042, "Nelson", "Rosie", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(2043, "O'Brien", "Hannah", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsSenior),

            CompetitorFactory.Create(2051, "Patel", "Samuel", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(2052, "Roberts", "Jacob", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(2053, "Simpson", "Logan", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsSenior),

            CompetitorFactory.Create(2061, "Taylor", "Beth", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(2062, "Upton", "Rachel", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(2063, "Vincent", "Claire", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.IsVeteran),

            CompetitorFactory.Create(2071, "White", "Adam", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(2072, "Zane", "Nathan", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(2073, "Adams", "Luke", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.IsVeteran),

            // Honorary competitors (24)
            CompetitorFactory.Create(3001, "Bennett", "Tia", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(3002, "Chapman", "Nina", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(3003, "Davies", "Leah", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(3011, "Ellis", "Jay", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(3012, "Franklin", "Max", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJuvenile),
            CompetitorFactory.Create(3013, "Gibson", "Ben", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJuvenile),

            CompetitorFactory.Create(3021, "Hayes", "Zara", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(3022, "Irving", "Megan", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJunior),
            CompetitorFactory.Create(3023, "Jennings", "Amber", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsJunior),

            CompetitorFactory.Create(3031, "Kirk", "Reece", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(3032, "Lloyd", "Tyler", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJunior),
            CompetitorFactory.Create(3033, "Mitchell", "Ryan", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsJunior),

            CompetitorFactory.Create(3041, "Norris", "Ella", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(3042, "Olsen", "Katie", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsSenior),
            CompetitorFactory.Create(3043, "Parker", "Georgia", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsSenior),

            CompetitorFactory.Create(3051, "Quincy", "Aaron", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(3052, "Robinson", "Charlie", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsSenior),
            CompetitorFactory.Create(3053, "Stevens", "Joseph", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsSenior),

            CompetitorFactory.Create(3061, "Thompson", "Diana", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(3062, "Ursula", "Fiona", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsVeteran),
            CompetitorFactory.Create(3063, "Valentine", "Paula", ClaimStatus.Honorary, isFemale: true, AgeGroup.IsVeteran),

            CompetitorFactory.Create(3071, "Watson", "Graham", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(3072, "York", "Trevor", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsVeteran),
            CompetitorFactory.Create(3073, "Zimmer", "Derek", ClaimStatus.Honorary, isFemale: false, AgeGroup.IsVeteran),

            // Future-dated competitors (CreatedUtc = 50 days from now)
            CompetitorFactory.Create(4001, "Harper", "Sylvie", ClaimStatus.FirstClaim, true, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4002, "Cross", "Damon", ClaimStatus.FirstClaim, false, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4003, "Langford", "Tessa", ClaimStatus.FirstClaim, true, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4004, "Blake", "Ronan", ClaimStatus.FirstClaim, false, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4005, "Frost", "Imogen", ClaimStatus.FirstClaim, true, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4006, "Drake", "Callum", ClaimStatus.FirstClaim, false, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4007, "Winslow", "Freya", ClaimStatus.FirstClaim, true, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4008, "Thorne", "Jasper", ClaimStatus.FirstClaim, false, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),

            CompetitorFactory.Create(4011, "Marsh", "Elodie", ClaimStatus.SecondClaim, true, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4012, "Holt", "Kieran", ClaimStatus.SecondClaim, false, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4013, "Bishop", "Lara", ClaimStatus.SecondClaim, true, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4014, "Grayson", "Elliot", ClaimStatus.SecondClaim, false, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4015, "Marlow", "Isabel", ClaimStatus.SecondClaim, true, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4016, "Sutton", "Miles", ClaimStatus.SecondClaim, false, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4017, "Harrington", "Clara", ClaimStatus.SecondClaim, true, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4018, "Dalton", "Reed", ClaimStatus.SecondClaim, false, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),

            CompetitorFactory.Create(4021, "Prescott", "Nina", ClaimStatus.Honorary, true, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4022, "Bennett", "Theo", ClaimStatus.Honorary, false, AgeGroup.IsJuvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4023, "Whitaker", "Maisie", ClaimStatus.Honorary, true, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4024, "Hawthorne", "Jude", ClaimStatus.Honorary, false, AgeGroup.IsJunior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4025, "Ellington", "Poppy", ClaimStatus.Honorary, true, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4026, "Radcliffe", "Finn", ClaimStatus.Honorary, false, AgeGroup.IsSenior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4027, "Ashford", "Lillian", ClaimStatus.Honorary, true, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4028, "Tanner", "Rowan", ClaimStatus.Honorary, false, AgeGroup.IsVeteran, DateTime.UtcNow.AddDays(50)),

        }.AsReadOnly();
    }
}

