using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ClubProcessor.Context
{
    public class CompetitorDbContextFactory : IDesignTimeDbContextFactory<CompetitorDbContext>
    {
        public CompetitorDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CompetitorDbContext>();
            optionsBuilder.UseSqlite("Data Source=../../data/club_competitors_2026.db");

            return new CompetitorDbContext(optionsBuilder.Options);
        }
    }
}
