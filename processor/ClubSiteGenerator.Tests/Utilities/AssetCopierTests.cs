using AutoFixture;
using ClubCore.Utilities;
using ClubSiteGenerator.Tests.Helpers;
using ClubSiteGenerator.Utilities;
using FluentAssertions;

namespace ClubSiteGenerator.Tests.Utilities
{
    public class AssetCopierTests
    {
        private readonly Fixture _fixture = new();

        [Fact]
        public void ExactMatch_ShouldCopyExactFile_AndReturnFilename()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();

            var assetsRoot = Path.Combine(root, PathTokens.AssetsFolder);
            var outputRoot = Path.Combine(root, "output");

            var year = 2025;
            var prefix = "roundrobin";
            var fileName = $"{prefix}{year}.css";

            TestFileSystem.CreateCssFile(assetsRoot, fileName);

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(
                assetsRoot,
                outputRoot,
                year,
                prefix
            );

            // Assert
            result.Should().Be(fileName);

            var copied = Path.Combine(outputRoot, PathTokens.AssetsFolder, fileName);
            File.Exists(copied).Should().BeTrue();
        }

        [Fact]
        public void MissingExactMatch_ShouldFallbackToLatestEarlierYear()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();

            var assetsRoot = Path.Combine(root, PathTokens.AssetsFolder);
            var outputRoot = Path.Combine(root, "output");

            var prefix = "roundrobin";

            TestFileSystem.CreateCssFile(assetsRoot, $"{prefix}2023.css");
            TestFileSystem.CreateCssFile(assetsRoot, $"{prefix}2024.css");

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(
                assetsRoot,
                outputRoot,
                2025,
                prefix
            );

            // Assert
            result.Should().Be($"{prefix}2024.css");

            var copied = Path.Combine(outputRoot, PathTokens.AssetsFolder, $"{prefix}2024.css");
            File.Exists(copied).Should().BeTrue();
        }

        [Fact]
        public void NoEarlierYear_ShouldThrow()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();

            var assetsRoot = Path.Combine(root, PathTokens.AssetsFolder);
            var outputRoot = Path.Combine(root, "output");

            var prefix = "roundrobin";

            TestFileSystem.CreateCssFile(assetsRoot, $"{prefix}2027.css");

            // Act
            var act = () => AssetCopier.CopyYearSpecificStylesheet(
                assetsRoot,
                outputRoot,
                2025,
                prefix
            );

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("*earlier year*");
        }

        [Fact]
        public void NoFilesAtAll_ShouldThrow()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();

            var assetsRoot = Path.Combine(root, PathTokens.AssetsFolder);
            var outputRoot = Path.Combine(root, "output");

            var prefix = "roundrobin";

            // Act
            var act = () => AssetCopier.CopyYearSpecificStylesheet(
                assetsRoot,
                outputRoot,
                2025,
                prefix
            );

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("*No CSS files found*");
        }

        [Fact]
        public void ShouldCopyFileToAssetsFolder()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();

            var assetsRoot = Path.Combine(root, PathTokens.AssetsFolder);
            var outputRoot = Path.Combine(root, "output");

            var prefix = "roundrobin";
            var fileName = $"{prefix}2025.css";

            TestFileSystem.CreateCssFile(assetsRoot, fileName);

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(
                assetsRoot,
                outputRoot,
                2026,
                prefix
            );

            // Assert
            var copied = Path.Combine(outputRoot, PathTokens.AssetsFolder, fileName);
            File.Exists(copied).Should().BeTrue();
        }
    }
}