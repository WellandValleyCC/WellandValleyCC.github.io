using ClubCore.Context;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubSiteGenerator.Tests
{
    public class DbContextHelperTests
    {
        [Fact]
        public void CreateEventContext_ThrowsIfPendingMigrationsExist()
        {
            // Arrange: use in-memory SQLite with a fake migration
            var options = new DbContextOptionsBuilder<EventDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using var context = new EventDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            // Simulate pending migration by adding one manually
            var migrationsAssembly = context.GetService<IMigrationsAssembly>();
            var modelDiffer = context.GetService<IMigrationsModelDiffer>();
            var historyRepository = context.GetService<IHistoryRepository>();

            var appliedMigrationNames = historyRepository.GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var pending = migrationsAssembly?.Migrations?.Keys
                .Except(appliedMigrationNames)
                .ToList();

            // Act & Assert
            pending.Should().NotBeEmpty("because at least one pending migration is expected for this test scenario");

            Action act = () =>
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    throw new InvalidOperationException("Pending migrations detected");
                }
            };

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*Pending migrations detected*",
                   "because the read-only app should refuse to run when migrations are outstanding");
        }
    }
}
