using ClubCore.Utilities;
using FluentAssertions;

namespace ClubSiteGenerator.Tests.Utilities
{
    public class DefaultFileProviderTests
    {
        [Fact]
        public void Copy_Creates_Destination_File()
        {
            var provider = new DefaultFileProvider();

            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temp);

            var src = Path.Combine(temp, "a.txt");
            var dest = Path.Combine(temp, "b.txt");

            File.WriteAllText(src, "hello");

            provider.Copy(src, dest, overwrite: true);

            File.Exists(dest).Should().BeTrue();
            File.ReadAllText(dest).Should().Be("hello");

            Directory.Delete(temp, recursive: true);
        }

        [Fact]
        public void GetFiles_Returns_Files()
        {
            var provider = new DefaultFileProvider();

            var temp = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(temp);

            var file = Path.Combine(temp, "x.txt");
            File.WriteAllText(file, "data");

            var result = provider.GetFiles(temp);

            result.Should().Contain(file);

            Directory.Delete(temp, recursive: true);
        }
    }
}
