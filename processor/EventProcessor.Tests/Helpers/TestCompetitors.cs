using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;

namespace EventProcessor.Tests.Helpers
{
    public static class TestCompetitors
    {
        // Static pool: three of each age-group/gender combination (24 competitors total).
        // ClubNumber chosen to be unique and easy to read.
        public static readonly IReadOnlyList<Competitor> All = new List<Competitor>
        {
            // Juvenile female
            CompetitorFactory.Create(1001, "Bates", "Mia", isFemale: true, ageGroup: AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1002, "Carson", "Isla", isFemale: true, ageGroup: AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1003, "Dennison", "Zoe", isFemale: true, ageGroup: AgeGroup.IsJuvenile),

            // Juvenile male
            CompetitorFactory.Create(1011, "Evans", "Liam", isFemale: false, ageGroup: AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1012, "Fletcher", "Noah", isFemale: false, ageGroup: AgeGroup.IsJuvenile),
            CompetitorFactory.Create(1013, "Graham", "Ethan", isFemale: false, ageGroup: AgeGroup.IsJuvenile),

            // Junior female
            CompetitorFactory.Create(1021, "Hughes", "Amelia", isFemale: true, ageGroup: AgeGroup.IsJunior),
            CompetitorFactory.Create(1022, "Irwin", "Sophie", isFemale: true, ageGroup: AgeGroup.IsJunior),
            CompetitorFactory.Create(1023, "Jackson", "Grace", isFemale: true, ageGroup: AgeGroup.IsJunior),

            // Junior male
            CompetitorFactory.Create(1031, "King", "Oliver", isFemale: false, ageGroup: AgeGroup.IsJunior),
            CompetitorFactory.Create(1032, "Lewis", "Harry", isFemale: false, ageGroup: AgeGroup.IsJunior),
            CompetitorFactory.Create(1033, "Mason", "Jack", isFemale: false, ageGroup: AgeGroup.IsJunior),

            // Senior female
            CompetitorFactory.Create(1041, "Nash", "Charlotte", isFemale: true, ageGroup: AgeGroup.IsSenior),
            CompetitorFactory.Create(1042, "Owens", "Emily", isFemale: true, ageGroup: AgeGroup.IsSenior),
            CompetitorFactory.Create(1043, "Price", "Lucy", isFemale: true, ageGroup: AgeGroup.IsSenior),

            // Senior male
            CompetitorFactory.Create(1051, "Quinn", "James", isFemale: false, ageGroup: AgeGroup.IsSenior),
            CompetitorFactory.Create(1052, "Reid", "Thomas", isFemale: false, ageGroup: AgeGroup.IsSenior),
            CompetitorFactory.Create(1053, "Shaw", "Daniel", isFemale: false, ageGroup: AgeGroup.IsSenior),

            // Veteran female
            CompetitorFactory.Create(1061, "Turner", "Helen", isFemale: true, ageGroup: AgeGroup.IsVeteran),
            CompetitorFactory.Create(1062, "Underwood", "Alison", isFemale: true, ageGroup: AgeGroup.IsVeteran),
            CompetitorFactory.Create(1063, "Vaughn", "Janet", isFemale: true, ageGroup: AgeGroup.IsVeteran),

            // Veteran male
            CompetitorFactory.Create(1071, "Walker", "Peter", isFemale: false, ageGroup: AgeGroup.IsVeteran),
            CompetitorFactory.Create(1072, "Xavier", "Martin", isFemale: false, ageGroup: AgeGroup.IsVeteran),
            CompetitorFactory.Create(1073, "Young", "Colin", isFemale: false, ageGroup: AgeGroup.IsVeteran)
        }.AsReadOnly();
    }
}
