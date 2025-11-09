using ClubProcessor.Models.Enums;
using ClubProcessor.Models.Extensions;

namespace ClubProcessor.Tests
{
    public class LeagueExtensionsTests
    {
        [Theory]
        [InlineData("Prem", League.Premier)]
        [InlineData("premier", League.Premier)]
        [InlineData("2", League.League2)]
        [InlineData("unknown", League.Undefined)]
        [InlineData(null, League.Undefined)]
        public void ParseFromCsv_MapsCorrectly(string? input, League expected)
        {
            var result = LeagueExtensions.ParseFromCsv(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(League.Premier, "Prem")]
        [InlineData(League.League3, "3")]
        [InlineData(League.Undefined, "")]
        public void ToCsvValue_MapsCorrectly(League league, string expected)
        {
            var result = league.ToCsvValue();
            Assert.Equal(expected, result);
        }
    }
}
