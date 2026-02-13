using ClubCore.Interfaces;
using ClubCore.Utilities;
using ClubSiteGenerator.Interfaces;
using NSubstitute;

namespace ClubSiteGenerator.Tests.Utilities
{
    public class DefaultDirectoryCopyHelperTests
    {
        [Fact]
        public void Delegates_To_Inner_Helper()
        {
            var directoryProvider = Substitute.For<IDirectoryProvider>();
            var fileProvider = Substitute.For<IFileProvider>();
            var log = Substitute.For<ILog>();

            var helper = new DefaultDirectoryCopyHelper(
                directoryProvider,
                fileProvider,
                log
            );

            helper.CopyRecursive("src", "dest", new[] { ".md" });

            directoryProvider.Received().Exists("src");
        }
    }
}
