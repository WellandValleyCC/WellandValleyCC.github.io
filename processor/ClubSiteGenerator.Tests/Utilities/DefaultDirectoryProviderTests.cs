using ClubCore.Utilities;
using FluentAssertions;

namespace ClubSiteGenerator.Tests.Utilities
{
    public class DefaultDirectoryProviderTests
    {
        [Fact]
        public void Exists_Returns_True_For_Current_Directory()
        {
            var provider = new DefaultDirectoryProvider();
            provider.Exists(".").Should().BeTrue();
        }

        [Fact]
        public void Can_Create_And_Detect_Directory()
        {
            var provider = new DefaultDirectoryProvider();

            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            provider.CreateDirectory(temp);

            provider.Exists(temp).Should().BeTrue();

            Directory.Delete(temp);
        }

        [Fact]
        public void GetDirectories_Returns_Subdirectories()
        {
            var provider = new DefaultDirectoryProvider();

            var root = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var sub = Path.Combine(root, "child");

            Directory.CreateDirectory(sub);

            var result = provider.GetDirectories(root);

            result.Should().Contain(sub);

            Directory.Delete(root, recursive: true);
        }
    }
}
