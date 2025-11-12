using ClubCore.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubCore.Utilities
{
    public static class DbContextFactory
    {
        public static EventDbContext CreateEventContext(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            var path = Path.Combine(dataDir, $"club_events_{year}.db");

            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

            return new EventDbContext(options);
        }

        public static CompetitorDbContext CreateCompetitorContext(string year)
        {
            var dataDir = FolderLocator.GetDataDirectory();
            var path = Path.Combine(dataDir, $"club_competitors_{year}.db");

            var options = new DbContextOptionsBuilder<CompetitorDbContext>()
                .UseSqlite($"Data Source={path}")
                .Options;

            return new CompetitorDbContext(options);
        }
    }
}
