using AutoFixture;
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
            Directory.SetCurrentDirectory(root);

            var year = 2025;
            var prefix = "roundrobin";
            var fileName = $"{prefix}{year}.css";

            TestFileSystem.CreateCssFile(root, fileName);

            var outputDir = Path.Combine(root, "output");

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(outputDir, year, prefix);

            // Assert
            result.Should().Be(fileName);

            var copied = Path.Combine(outputDir, "assets", fileName);
            File.Exists(copied).Should().BeTrue();
        }

        [Fact]
        public void MissingExactMatch_ShouldFallbackToLatestEarlierYear()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();
            Directory.SetCurrentDirectory(root);

            var prefix = "roundrobin";

            TestFileSystem.CreateCssFile(root, $"{prefix}2023.css");
            TestFileSystem.CreateCssFile(root, $"{prefix}2024.css");

            var outputDir = Path.Combine(root, "output");

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(outputDir, 2025, prefix);

            // Assert
            result.Should().Be($"{prefix}2024.css");

            var copied = Path.Combine(outputDir, "assets", $"{prefix}2024.css");
            File.Exists(copied).Should().BeTrue();
        }

        [Fact]
        public void NoEarlierYear_ShouldThrow()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();
            Directory.SetCurrentDirectory(root);

            var prefix = "roundrobin";

            TestFileSystem.CreateCssFile(root, $"{prefix}2027.css");

            var outputDir = Path.Combine(root, "output");

            // Act
            var act = () => AssetCopier.CopyYearSpecificStylesheet(outputDir, 2025, prefix);

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("*earlier year*");
        }

        [Fact]
        public void NoFilesAtAll_ShouldThrow()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();
            Directory.SetCurrentDirectory(root);

            var prefix = "roundrobin";
            var outputDir = Path.Combine(root, "output");

            // Act
            var act = () => AssetCopier.CopyYearSpecificStylesheet(outputDir, 2025, prefix);

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("*No CSS files found*");
        }

        [Fact]
        public void ShouldCopyFileToAssetsFolder()
        {
            // Arrange
            var root = TestFileSystem.CreateTempAssetsFolder();
            Directory.SetCurrentDirectory(root);

            var prefix = "roundrobin";
            var fileName = $"{prefix}2025.css";

            TestFileSystem.CreateCssFile(root, fileName);

            var outputDir = Path.Combine(root, "output");

            // Act
            var result = AssetCopier.CopyYearSpecificStylesheet(outputDir, 2026, prefix);

            // Assert
            var copied = Path.Combine(outputDir, "assets", fileName);
            File.Exists(copied).Should().BeTrue();
        }
    }
}