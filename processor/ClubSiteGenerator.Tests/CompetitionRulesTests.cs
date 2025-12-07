using ClubCore.Utilities;
using ClubSiteGenerator.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClubSiteGenerator.Tests
{
    public class CompetitionRulesTests
    {
        [Fact]
        public void RulesJson_DeserializesCorrectly()
        {
            // Hardcoded JSON string for testing
            var json = @"
{
    ""2025"": {
    ""tenMile"": {
        ""count"": 8
    },
    ""mixedDistance"": {
        ""formula"": ""calendarEvents/2+1"",
        ""cap"": 11,
        ""nonTenMinimum"": 2
    }
    }
}";
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = false };
            var configs = JsonSerializer.Deserialize<Dictionary<int, YearRules>>(json, options)!;

            var yr2025 = configs[2025];
            Assert.Equal(8, yr2025.TenMile.Count);
            Assert.Equal("calendarEvents/2+1", yr2025.MixedDistance.Formula);
            Assert.Equal(11, yr2025.MixedDistance.Cap);
            Assert.Equal(2, yr2025.MixedDistance.NonTenMinimum);
        }
    }
}
