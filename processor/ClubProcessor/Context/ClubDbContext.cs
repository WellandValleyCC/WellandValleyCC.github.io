using Microsoft.EntityFrameworkCore;
using ClubProcessor.Models;

namespace ClubProcessor.Context
{
    public class ClubDbContext : DbContext
    {
        public DbSet<Competitor> Competitors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=../data/results.db");
    }
}
