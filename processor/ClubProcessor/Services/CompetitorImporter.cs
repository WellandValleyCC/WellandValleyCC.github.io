using ClubProcessor.Models;
using ClubProcessor.Context;

namespace ClubProcessor.Services
{
    public class CompetitorImporter
    {
        public static void Import(string csvPath)
        {
            using var context = new ClubDbContext();
            var lines = File.ReadAllLines(csvPath).Skip(1); // Skip header

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                var competitor = new Competitor
                {
                    Name = parts[0],
                    Age = int.Parse(parts[1]),
                    Category = parts[2],
                    IsFemale = parts[3].ToLower() == "true",
                    MemberNumber = parts[4]
                };
                context.Competitors.Add(competitor);
            }

            context.SaveChanges();
        }
    }
}
