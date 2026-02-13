using ClubCore.Interfaces;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ClubCore.Tests.Utilities
{
    public class DbPathResolverTests
    {
        private readonly IFolderLocator folderLocator = Substitute.For<IFolderLocator>();

        private IDbPathResolver CreateResolver() =>
            new DbPathResolver(folderLocator);

        [Fact]
        public void Builds_Competitor_Db_Path()
        {
            folderLocator.FindGitRepoRoot().Returns("root");

            var resolver = CreateResolver();
            var result = resolver.GetCompetitorDbPath("2024");

            result.Should().Be(
                Path.Combine("root", PathTokens.DataFolder, "club_competitors_2024.db")
            );
        }

        [Fact]
        public void Builds_Event_Db_Path()
        {
            folderLocator.FindGitRepoRoot().Returns("root");

            var resolver = CreateResolver();
            var result = resolver.GetEventDbPath("2024");

            result.Should().Be(
                Path.Combine("root", PathTokens.DataFolder, "club_events_2024.db")
            );
        }
    }
}