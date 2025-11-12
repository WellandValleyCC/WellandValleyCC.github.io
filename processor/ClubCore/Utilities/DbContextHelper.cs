using ClubCore.Context;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubCore.Utilities
{
    public static class DbContextHelper
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

        public static void Migrate(DbContext context)
        {
            context.Database.Migrate();

            var relational = context.Database.GetDbConnection() as SqliteConnection;
            var dbPath = relational?.DataSource ?? "(unknown)";
            var folderName = new FileInfo(dbPath).Directory?.Name;
            var dbName = new FileInfo(dbPath).Name;

            Console.WriteLine($"[INFO] Migration complete for: {folderName}/{dbName}");
        }
    }
}
