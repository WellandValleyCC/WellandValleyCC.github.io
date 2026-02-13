using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Utilities;
using FluentAssertions;
using NSubstitute;

namespace ClubCore.Tests.Utilities
{
    public class AssetPipelineTests
    {
        private readonly IAssetCopier assetCopier = Substitute.For<IAssetCopier>();
        private readonly IDirectoryCopyHelper copyHelper = Substitute.For<IDirectoryCopyHelper>();
        private readonly IDirectoryProvider directoryProvider = Substitute.For<IDirectoryProvider>();
        private readonly ILog log = Substitute.For<ILog>();

        private AssetPipeline CreatePipeline()
        {
            return new AssetPipeline(
                assetCopier,
                copyHelper,
                directoryProvider,
                log
            );
        }

        [Fact]
        public void Returns_CssFile_From_AssetCopier()
        {
            assetCopier
                .CopyYearSpecificStylesheet(Arg.Any<string>(), Arg.Any<string>(), 2025, Arg.Any<string>())
                .Returns("roundrobin-2025.css");

            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            var result = pipeline.CopyRoundRobinAssets("root", 2025);

            result.CssFile.Should().Be("roundrobin-2025.css");
        }

        [Fact]
        public void Copies_Logos_Folder()
        {
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            pipeline.CopyRoundRobinAssets("root", 2025);

            copyHelper.Received().CopyRecursive(
                Path.Combine("root", "RoundRobinSiteAssets", "logos"),
                Path.Combine("root", "RoundRobinSiteOutput", "logos"),
                Arg.Is<string[]>(x => x.Contains(".md"))
            );
        }

        [Fact]
        public void Copies_Additional_Asset_Folders()
        {
            // Assemble
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(new[]
                {
                    Path.Combine("root", "RoundRobinSiteAssets", "extra"),
                    Path.Combine("root", "RoundRobinSiteAssets", "assets"),
                    Path.Combine("root", "RoundRobinSiteAssets", "logos")
                });

            var pipeline = CreatePipeline();

            // Act
            pipeline.CopyRoundRobinAssets("root", 2025);

            // Assert
            copyHelper.Received().CopyRecursive(
                Path.Combine("root", "RoundRobinSiteAssets", "extra"),
                Path.Combine("root", "RoundRobinSiteOutput", "extra"),
                Arg.Any<string[]>()
            );
        }

        [Fact]
        public void Logs_Startup_And_Completion()
        {
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            pipeline.CopyRoundRobinAssets("root", 2025);

            log.Received().Info(Arg.Is<string>(s => s.Contains("Starting Round Robin asset pipeline")));
            log.Received().Info("Asset pipeline complete.");
        }
    }
}