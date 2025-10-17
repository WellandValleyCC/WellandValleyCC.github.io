using ClubProcessor.Models;
using ClubProcessor.Context;

namespace ClubProcessor.Services
{
    public class CompetitorImporter
    {
        private readonly ClubDbContext _context;

        public CompetitorImporter(ClubDbContext context)
        {
            _context = context;
        }

        public void Import(string csvPath)
        {
            var lines = File.ReadAllLines(csvPath).Skip(1);

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
                _context.Competitors.Add(competitor);
            }

            _context.SaveChanges();
        }
    }
}
