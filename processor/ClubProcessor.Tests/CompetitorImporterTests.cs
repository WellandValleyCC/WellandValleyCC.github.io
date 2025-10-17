using Xunit;
using ClubProcessor.Context;
using ClubProcessor.Services;
using Microsoft.EntityFrameworkCore;

namespace ClubProcessor.Tests
{
    public class CompetitorImporterTests
    {
        [Fact]
        public void Import_ShouldRunWithoutError_ForValidCsv()
        {
            var options = new DbContextOptionsBuilder<ClubDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            using var context = new ClubDbContext(options);
            var importer = new CompetitorImporter(context);

            var testCsvPath = "test-data/competitors_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(testCsvPath, "Name,Age,Category,IsFemale,MemberNumber\nAlice,30,Senior,true,123");

            importer.Import(testCsvPath);

            Assert.Single(context.Competitors);
        }
    }
}
