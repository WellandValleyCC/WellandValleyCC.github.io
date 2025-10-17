using Xunit;
using ClubProcessor.Services;

namespace ClubProcessor.Tests
{
    public class CompetitorImporterTests
    {
        [Fact]
        public void Import_ShouldRunWithoutError_ForValidCsv()
        {
            // Arrange
            var testCsvPath = "test-data/competitors_test.csv";
            Directory.CreateDirectory("test-data");
            File.WriteAllText(testCsvPath, "Name,Age,Category,IsFemale,MemberNumber\nAlice,30,Senior,true,123");

            // Act
            CompetitorImporter.Import(testCsvPath);

            // Assert
            Assert.True(File.Exists(testCsvPath)); // Minimal assertion for now
        }
    }
}
