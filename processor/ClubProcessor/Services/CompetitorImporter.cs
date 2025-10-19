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
					ClubNumber = row[0],
					Surname = row[1],
					GivenName = row[2],
					ClaimStatus = row[3],
					IsFemale = bool.Parse(row[4]),
					IsJuvenile = bool.Parse(row[5]),
					IsJunior = bool.Parse(row[6]),
					IsSenior = bool.Parse(row[7]),
					IsVeteran = bool.Parse(row[8])
				};

                _context.Competitors.Add(competitor);
            }

            _context.SaveChanges();
        }
    }
}
