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
					ClubNumber = parts[0],
					Surname = parts[1],
					GivenName = parts[2],
					ClaimStatus = parts[3],
					IsFemale = bool.Parse(parts[4]),
					IsJuvenile = bool.Parse(parts[5]),
					IsJunior = bool.Parse(parts[6]),
					IsSenior = bool.Parse(parts[7]),
					IsVeteran = bool.Parse(parts[8])
				};

                _context.Competitors.Add(competitor);
            }

            _context.SaveChanges();
        }
    }
}
