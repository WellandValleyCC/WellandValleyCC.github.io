using ClubCore.Interfaces;
using ClubCore.Utilities;
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

        private AssetPipelineResult RunPipeline(AssetPipeline pipeline)
        {
            // Arrange: create fake absolute paths for assets and output
            var assetsRoot = Path.Combine("root", PathTokens.RoundRobinAssetsFolder);
            var outputRoot = Path.Combine("root", PathTokens.RoundRobinOutputFolder);

            return pipeline.CopyRoundRobinAssets(
                assetsRoot,
                outputRoot,
                2025,
                PathTokens.RoundRobinCssPrefix,
                "Round Robin"
            );
        }

        [Fact]
        public void Returns_CssFile_From_AssetCopier()
        {
            assetCopier
                .CopyYearSpecificStylesheet(
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    2025,
                    Arg.Any<string>())
                .Returns("roundrobin-2025.css");

            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            var result = RunPipeline(pipeline);

            result.CssFile.Should().Be("roundrobin-2025.css");
        }

        [Fact]
        public void Copies_Logos_Folder()
        {
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            RunPipeline(pipeline);

            copyHelper.Received().CopyRecursive(
                Path.Combine("root", PathTokens.RoundRobinAssetsFolder, PathTokens.LogosFolder),
                Path.Combine("root", PathTokens.RoundRobinOutputFolder, PathTokens.LogosFolder),
                Arg.Is<string[]>(x => x.Contains(PathTokens.MarkdownExtension))
            );
        }

        [Fact]
        public void Copies_Additional_Asset_Folders()
        {
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(new[]
                {
                    Path.Combine("root", PathTokens.RoundRobinAssetsFolder, "extra"),
                    Path.Combine("root", PathTokens.RoundRobinAssetsFolder, PathTokens.AssetsFolder),
                    Path.Combine("root", PathTokens.RoundRobinAssetsFolder, PathTokens.LogosFolder)
                });

            var pipeline = CreatePipeline();

            RunPipeline(pipeline);

            copyHelper.Received().CopyRecursive(
                Path.Combine("root", PathTokens.RoundRobinAssetsFolder, "extra"),
                Path.Combine("root", PathTokens.RoundRobinOutputFolder, "extra"),
                Arg.Any<string[]>()
            );
        }

        [Fact]
        public void Logs_Startup_And_Completion()
        {
            directoryProvider.GetDirectories(Arg.Any<string>())
                .Returns(Array.Empty<string>());

            var pipeline = CreatePipeline();

            RunPipeline(pipeline);

            log.Received().Info(Arg.Is<string>(s => s.Contains("Starting Round Robin asset pipeline")));
            log.Received().Info("Asset pipeline complete.");
        }
    }
}