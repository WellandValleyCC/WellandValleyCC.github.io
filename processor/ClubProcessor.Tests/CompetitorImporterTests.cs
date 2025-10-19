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
			File.WriteAllText(
				testCsvPath,
				"ClubNumber,Surname,GivenName,ClaimStatus,isFemale,isJuvenile,isJunior,isSenior,isVeteran\n" +
				"9999,Doe,John,First Claim,false,false,false,true,false");

            importer.Import(testCsvPath);

            Assert.Single(context.Competitors);
        }
    }
}
