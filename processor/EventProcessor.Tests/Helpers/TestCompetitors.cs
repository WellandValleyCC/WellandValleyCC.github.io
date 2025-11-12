using ClubCore.Models;
using ClubCore.Models.Enums;
using System;
using System.Collections.Generic;

namespace EventProcessor.Tests.Helpers
{
    public static class TestCompetitors
    {
        public static readonly IReadOnlyList<Competitor> All = new List<Competitor>
        {
            // 24 FirstClaim competitors
            CompetitorFactory.Create(1001, "Bates", "Mia", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(1002, "Carson", "Isla", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(1003, "Dennison", "Zoe", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Juvenile),

            CompetitorFactory.Create(1011, "Evans", "Liam", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(1012, "Fletcher", "Noah", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(1013, "Graham", "Ethan", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Juvenile),

            CompetitorFactory.Create(1021, "Hughes", "Amelia", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(1022, "Irwin", "Sophie", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(1023, "Jackson", "Grace", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Junior),

            CompetitorFactory.Create(1031, "King", "Oliver", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(1032, "Lewis", "Harry", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(1033, "Mason", "Jack", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Junior),

            CompetitorFactory.Create(1041, "Nash", "Charlotte", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(1042, "Owens", "Emily", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(1043, "Price", "Lucy", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Senior),

            CompetitorFactory.Create(1051, "Quinn", "James", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(1052, "Reid", "Thomas", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(1053, "Shaw", "Daniel", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Senior),

            CompetitorFactory.Create(1061, "Turner", "Helen", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Veteran, vetsBucket: 4),
            CompetitorFactory.Create(1062, "Underwood", "Alison", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Veteran, vetsBucket: 4),
            CompetitorFactory.Create(1063, "Vaughn", "Janet", ClaimStatus.FirstClaim, isFemale: true, AgeGroup.Veteran, vetsBucket: 4),

            CompetitorFactory.Create(1071, "Walker", "Peter", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 5),
            CompetitorFactory.Create(1072, "Xavier", "Martin", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 5),
            CompetitorFactory.Create(1073, "Young", "Colin", ClaimStatus.FirstClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 5),

            // SecondClaim competitors (24)
            CompetitorFactory.Create(2001, "Abbott", "Maya", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(2002, "Barker", "Ella", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(2003, "Carter", "Ruby", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Juvenile),

            CompetitorFactory.Create(2011, "Dixon", "Leo", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(2012, "Edwards", "Oscar", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(2013, "Foster", "George", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Juvenile),

            CompetitorFactory.Create(2021, "Griffin", "Chloe", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(2022, "Harrison", "Millie", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(2023, "Ingram", "Lily", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Junior),

            CompetitorFactory.Create(2031, "Johnson", "Freddie", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(2032, "Kerr", "Archie", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(2033, "Lawson", "Henry", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Junior),

            CompetitorFactory.Create(2041, "Matthews", "Eva", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(2042, "Nelson", "Rosie", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(2043, "O'Brien", "Hannah", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Senior),

            CompetitorFactory.Create(2051, "Patel", "Samuel", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(2052, "Roberts", "Jacob", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(2053, "Simpson", "Logan", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Senior),

            CompetitorFactory.Create(2061, "Taylor", "Beth", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Veteran   , vetsBucket: 1),
            CompetitorFactory.Create(2062, "Upton", "Rachel", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Veteran  , vetsBucket: 2),
            CompetitorFactory.Create(2063, "Vincent", "Claire", ClaimStatus.SecondClaim, isFemale: true, AgeGroup.Veteran, vetsBucket: 4),

            CompetitorFactory.Create(2071, "White", "Adam", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 4),
            CompetitorFactory.Create(2072, "Zane", "Nathan", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 6),
            CompetitorFactory.Create(2073, "Adams", "Luke", ClaimStatus.SecondClaim, isFemale: false, AgeGroup.Veteran, vetsBucket: 8),

            // Honorary competitors (24)
            CompetitorFactory.Create(3001, "Bennett", "Tia", ClaimStatus.Honorary, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(3002, "Chapman", "Nina", ClaimStatus.Honorary, isFemale: true, AgeGroup.Juvenile),
            CompetitorFactory.Create(3003, "Davies", "Leah", ClaimStatus.Honorary, isFemale: true, AgeGroup.Juvenile),

            CompetitorFactory.Create(3011, "Ellis", "Jay", ClaimStatus.Honorary, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(3012, "Franklin", "Max", ClaimStatus.Honorary, isFemale: false, AgeGroup.Juvenile),
            CompetitorFactory.Create(3013, "Gibson", "Ben", ClaimStatus.Honorary, isFemale: false, AgeGroup.Juvenile),

            CompetitorFactory.Create(3021, "Hayes", "Zara", ClaimStatus.Honorary, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(3022, "Irving", "Megan", ClaimStatus.Honorary, isFemale: true, AgeGroup.Junior),
            CompetitorFactory.Create(3023, "Jennings", "Amber", ClaimStatus.Honorary, isFemale: true, AgeGroup.Junior),

            CompetitorFactory.Create(3031, "Kirk", "Reece", ClaimStatus.Honorary, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(3032, "Lloyd", "Tyler", ClaimStatus.Honorary, isFemale: false, AgeGroup.Junior),
            CompetitorFactory.Create(3033, "Mitchell", "Ryan", ClaimStatus.Honorary, isFemale: false, AgeGroup.Junior),

            CompetitorFactory.Create(3041, "Norris", "Ella", ClaimStatus.Honorary, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(3042, "Olsen", "Katie", ClaimStatus.Honorary, isFemale: true, AgeGroup.Senior),
            CompetitorFactory.Create(3043, "Parker", "Georgia", ClaimStatus.Honorary, isFemale: true, AgeGroup.Senior),

            CompetitorFactory.Create(3051, "Quincy", "Aaron", ClaimStatus.Honorary, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(3052, "Robinson", "Charlie", ClaimStatus.Honorary, isFemale: false, AgeGroup.Senior),
            CompetitorFactory.Create(3053, "Stevens", "Joseph", ClaimStatus.Honorary, isFemale: false, AgeGroup.Senior),

            CompetitorFactory.Create(3061, "Thompson", "Diana", ClaimStatus.Honorary, isFemale: true, AgeGroup.Veteran, vetsBucket: 1),
            CompetitorFactory.Create(3062, "Ursula", "Fiona", ClaimStatus.Honorary, isFemale: true, AgeGroup.Veteran, vetsBucket: 5),
            CompetitorFactory.Create(3063, "Valentine", "Paula", ClaimStatus.Honorary, isFemale: true, AgeGroup.Veteran, vetsBucket: 10),

            CompetitorFactory.Create(3071, "Watson", "Graham", ClaimStatus.Honorary, isFemale: false, AgeGroup.Veteran, vetsBucket: 15),
            CompetitorFactory.Create(3072, "York", "Trevor", ClaimStatus.Honorary, isFemale: false, AgeGroup.Veteran, vetsBucket: 9),
            CompetitorFactory.Create(3073, "Zimmer", "Derek", ClaimStatus.Honorary, isFemale: false, AgeGroup.Veteran, vetsBucket: 5),

            // Future-dated competitors (CreatedUtc = 50 days from now)
            CompetitorFactory.Create(4001, "Harper", "Sylvie", ClaimStatus.FirstClaim, true, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4002, "Cross", "Damon", ClaimStatus.FirstClaim, false, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4003, "Langford", "Tessa", ClaimStatus.FirstClaim, true, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4004, "Blake", "Ronan", ClaimStatus.FirstClaim, false, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4005, "Frost", "Imogen", ClaimStatus.FirstClaim, true, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4006, "Drake", "Callum", ClaimStatus.FirstClaim, false, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4007, "Winslow", "Freya", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 12, createdUtc: DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4008, "Thorne", "Jasper", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 5, createdUtc: DateTime.UtcNow.AddDays(50)),

            CompetitorFactory.Create(4011, "Marsh", "Elodie", ClaimStatus.SecondClaim, true, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4012, "Holt", "Kieran", ClaimStatus.SecondClaim, false, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4013, "Bishop", "Lara", ClaimStatus.SecondClaim, true, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4014, "Grayson", "Elliot", ClaimStatus.SecondClaim, false, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4015, "Marlow", "Isabel", ClaimStatus.SecondClaim, true, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4016, "Sutton", "Miles", ClaimStatus.SecondClaim, false, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4017, "Harrington", "Clara", ClaimStatus.SecondClaim, true, AgeGroup.Veteran, vetsBucket: 17, createdUtc: DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4018, "Dalton", "Reed", ClaimStatus.SecondClaim, false, AgeGroup.Veteran, vetsBucket: 18, createdUtc: DateTime.UtcNow.AddDays(50)),

            CompetitorFactory.Create(4021, "Prescott", "Nina", ClaimStatus.Honorary, true, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4022, "Bennett", "Theo", ClaimStatus.Honorary, false, AgeGroup.Juvenile, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4023, "Whitaker", "Maisie", ClaimStatus.Honorary, true, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4024, "Hawthorne", "Jude", ClaimStatus.Honorary, false, AgeGroup.Junior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4025, "Ellington", "Poppy", ClaimStatus.Honorary, true, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4026, "Radcliffe", "Finn", ClaimStatus.Honorary, false, AgeGroup.Senior, DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4027, "Ashford", "Lillian", ClaimStatus.Honorary, true, AgeGroup.Veteran, vetsBucket: 7, createdUtc: DateTime.UtcNow.AddDays(50)),
            CompetitorFactory.Create(4028, "Tanner", "Rowan", ClaimStatus.Honorary, false, AgeGroup.Veteran, vetsBucket: 8, createdUtc: DateTime.UtcNow.AddDays(50)),

            // Male Veterans: Club numbers 5001–5040, VetsBucket 1–40
            CompetitorFactory.Create(5001, "Anderson", "Mark", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 1),
            CompetitorFactory.Create(5002, "Bennett", "Simon", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 2),
            CompetitorFactory.Create(5003, "Chapman", "Victor", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 3),
            CompetitorFactory.Create(5004, "Dalton", "Hugh", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 4),
            CompetitorFactory.Create(5005, "Ellison", "George", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 5),
            CompetitorFactory.Create(5006, "Foster", "Ian", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 6),
            CompetitorFactory.Create(5007, "Grayson", "Philip", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 7),
            CompetitorFactory.Create(5008, "Holt", "Adrian", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 8),
            CompetitorFactory.Create(5009, "Ingram", "Douglas", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 9),
            CompetitorFactory.Create(5010, "Jennings", "Colin", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 10),
            CompetitorFactory.Create(5011, "Kirk", "Stephen", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 11),
            CompetitorFactory.Create(5012, "Lawson", "Patrick", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 12),
            CompetitorFactory.Create(5013, "Murray", "Gareth", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 13),
            CompetitorFactory.Create(5014, "Norton", "Craig", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 14),
            CompetitorFactory.Create(5015, "Owens", "Darren", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 15),
            CompetitorFactory.Create(5016, "Parker", "Neil", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 16),
            CompetitorFactory.Create(5017, "Quinn", "Jason", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 17),
            CompetitorFactory.Create(5018, "Reid", "Anthony", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 18),
            CompetitorFactory.Create(5019, "Stevens", "Martin", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 19),
            CompetitorFactory.Create(5020, "Turner", "Paul", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 20),
            CompetitorFactory.Create(5021, "Underwood", "Gavin", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 21),
            CompetitorFactory.Create(5022, "Vaughn", "Stuart", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 22),
            CompetitorFactory.Create(5023, "Walker", "Dean", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 23),
            CompetitorFactory.Create(5024, "Xavier", "Leon", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 24),
            CompetitorFactory.Create(5025, "Young", "Malcolm", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 25),
            CompetitorFactory.Create(5026, "Zimmer", "Harvey", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 26),
            CompetitorFactory.Create(5027, "Abbott", "Trevor", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 27),
            CompetitorFactory.Create(5028, "Barker", "Nigel", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 28),
            CompetitorFactory.Create(5029, "Carter", "Russell", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 29),
            CompetitorFactory.Create(5030, "Dixon", "Clive", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 30),
            CompetitorFactory.Create(5031, "Edwards", "Alan", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 31),
            CompetitorFactory.Create(5032, "Fletcher", "Barry", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 32),
            CompetitorFactory.Create(5033, "Gibson", "Howard", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 33),
            CompetitorFactory.Create(5034, "Harrison", "Keith", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 34),
            CompetitorFactory.Create(5035, "Irving", "Rodney", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 35),
            CompetitorFactory.Create(5036, "Johnson", "Eric", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 36),
            CompetitorFactory.Create(5037, "Kerr", "Leslie", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 37),
            CompetitorFactory.Create(5038, "Lewis", "Norman", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 38),
            CompetitorFactory.Create(5039, "Matthews", "Geoffrey", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 39),
            CompetitorFactory.Create(5040, "Nelson", "Edward", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 40),

            // Female Veterans: Club numbers 5101–5140, VetsBucket 1–40
            CompetitorFactory.Create(5101, "Kendall", "Alice", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 1),
            CompetitorFactory.Create(5102, "Lawrence", "Sophie", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 2),
            CompetitorFactory.Create(5103, "Mitchell", "Clara", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 3),
            CompetitorFactory.Create(5104, "Norris", "Emily", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 4),
            CompetitorFactory.Create(5105, "Osborne", "Rachel", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 5),
            CompetitorFactory.Create(5106, "Peters", "Hannah", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 6),
            CompetitorFactory.Create(5107, "Quinn", "Laura", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 7),
            CompetitorFactory.Create(5108, "Reeves", "Charlotte", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 8),
            CompetitorFactory.Create(5109, "Stevens", "Grace", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 9),
            CompetitorFactory.Create(5110, "Turner", "Bethany", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 10),
            CompetitorFactory.Create(5111, "Underwood", "Megan", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 11),
            CompetitorFactory.Create(5112, "Vaughn", "Isabel", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 12),
            CompetitorFactory.Create(5113, "Walker", "Naomi", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 13),
            CompetitorFactory.Create(5114, "Xavier", "Phoebe", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 14),
            CompetitorFactory.Create(5115, "Young", "Claudia", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 15),
            CompetitorFactory.Create(5116, "Zimmer", "Harriet", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 16),
            CompetitorFactory.Create(5117, "Abbott", "Lydia", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 17),
            CompetitorFactory.Create(5118, "Barker", "Eliza", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 18),
            CompetitorFactory.Create(5119, "Carter", "Julia", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 19),
            CompetitorFactory.Create(5120, "Dixon", "Samantha", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 20),
            CompetitorFactory.Create(5121, "Edwards", "Natalie", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 21),
            CompetitorFactory.Create(5122, "Fletcher", "Rebecca", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 22),
            CompetitorFactory.Create(5123, "Gibson", "Angela", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 23),
            CompetitorFactory.Create(5124, "Harrison", "Melissa", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 24),
            CompetitorFactory.Create(5125, "Irving", "Joanna", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 25),
            CompetitorFactory.Create(5126, "Johnson", "Patricia", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 26),
            CompetitorFactory.Create(5127, "Kerr", "Vanessa", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 27),
            CompetitorFactory.Create(5128, "Lewis", "Monica", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 28),
            CompetitorFactory.Create(5129, "Matthews", "Eleanor", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 29),
            CompetitorFactory.Create(5130, "Nelson", "Audrey", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 30),
            CompetitorFactory.Create(5131, "O'Brien", "Catherine", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 31),
            CompetitorFactory.Create(5132, "Patel", "Louise", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 32),
            CompetitorFactory.Create(5133, "Roberts", "Deborah", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 33),
            CompetitorFactory.Create(5134, "Simpson", "Janice", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 34),
            CompetitorFactory.Create(5135, "Taylor", "Denise", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 35),
            CompetitorFactory.Create(5136, "Upton", "Margaret", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 36),
            CompetitorFactory.Create(5137, "Vincent", "Helen", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 37),
            CompetitorFactory.Create(5138, "White", "Joanne", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 38),
            CompetitorFactory.Create(5139, "Zane", "Frances", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 39),
            CompetitorFactory.Create(5140, "Adams", "Theresa", ClaimStatus.FirstClaim, true, AgeGroup.Veteran, vetsBucket: 40),

            // Nev Brooks test competitors (ClubNumbers 6001–6010)
            CompetitorFactory.Create(6001, "Harris", "Tom", ClaimStatus.FirstClaim, false, AgeGroup.Senior),
            CompetitorFactory.Create(6002, "Lewis", "Emma", ClaimStatus.Honorary, true, AgeGroup.Senior),
            CompetitorFactory.Create(6003, "Morris", "Jack", ClaimStatus.FirstClaim, false, AgeGroup.Junior),
            CompetitorFactory.Create(6004, "Nelson", "Sophie", ClaimStatus.FirstClaim, true, AgeGroup.Junior),
            CompetitorFactory.Create(6005, "Owen", "Daniel", ClaimStatus.SecondClaim, false, AgeGroup.Senior),
            CompetitorFactory.Create(6006, "Perry", "Grace", ClaimStatus.Honorary, true, AgeGroup.Veteran, vetsBucket: 2),
            CompetitorFactory.Create(6007, "Quinn", "Luke", ClaimStatus.FirstClaim, false, AgeGroup.Veteran, vetsBucket: 3),
            CompetitorFactory.Create(6008, "Reed", "Hannah", ClaimStatus.FirstClaim, true, AgeGroup.Senior),
            CompetitorFactory.Create(6009, "Smith", "Oliver", ClaimStatus.SecondClaim, false, AgeGroup.Junior),
            CompetitorFactory.Create(6010, "Taylor", "Chloe", ClaimStatus.Honorary, true, AgeGroup.Junior),

        }.AsReadOnly();
    }
}

