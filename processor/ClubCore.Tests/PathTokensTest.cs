using ClubCore.Utilities;
using FluentAssertions;

namespace ClubCore.Tests.Utilities
{
    public class PathTokensTests
    {
        [Fact]
        public void ClubSiteOutputFolder_HasExpectedValue()
        {
            PathTokens.ClubSiteOutputFolder
                .Should().Be("SiteOutput");
        }

        [Fact]
        public void ClubAssetsFolder_HasExpectedValue()
        {
            PathTokens.ClubAssetsFolder
                .Should().Be("ClubSiteAssets");
        }

        [Fact]
        public void RoundRobinOutputFolder_HasExpectedValue()
        {
            PathTokens.RoundRobinOutputFolder
                .Should().Be("RoundRobinSiteOutput");
        }

        [Fact]
        public void RoundRobinAssetsFolder_HasExpectedValue()
        {
            PathTokens.RoundRobinAssetsFolder
                .Should().Be("RoundRobinSiteAssets");
        }

        [Fact]
        public void DataFolder_HasExpectedValue()
        {
            PathTokens.DataFolder
                .Should().Be("data");
        }

        [Fact]
        public void ConfigFolder_HasExpectedValue()
        {
            PathTokens.ConfigFolder
                .Should().Be("config");
        }

        [Fact]
        public void AssetsFolder_HasExpectedValue()
        {
            PathTokens.AssetsFolder
                .Should().Be("assets");
        }

        [Fact]
        public void LogosFolder_HasExpectedValue()
        {
            PathTokens.LogosFolder
                .Should().Be("logos");
        }

        [Fact]
        public void RoundRobinCssPrefix_HasExpectedValue()
        {
            PathTokens.RoundRobinCssPrefix
                .Should().Be("roundrobin");
        }

        [Fact]
        public void CssExtension_HasExpectedValue()
        {
            PathTokens.CssExtension
                .Should().Be(".css");
        }

        [Fact]
        public void MarkdownExtension_HasExpectedValue()
        {
            PathTokens.MarkdownExtension
                .Should().Be(".md");
        }
    }
}