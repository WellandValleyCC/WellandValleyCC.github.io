using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Utilities;
using NSubstitute;

namespace ClubSiteGenerator.Tests.Utilities
{
    public class DirectoryCopyHelperTests
    {
        private readonly IDirectoryProvider directoryProvider = Substitute.For<IDirectoryProvider>();
        private readonly IFileProvider fileProvider = Substitute.For<IFileProvider>();
        private readonly ILog log = Substitute.For<ILog>();

        private DirectoryCopyHelper CreateHelper()
        {
            return new DirectoryCopyHelper(directoryProvider, fileProvider, log);
        }

        [Fact]
        public void Skips_When_Source_Directory_Does_Not_Exist()
        {
            directoryProvider.Exists("src").Returns(false);

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", new[] { ".md" });

            log.Received().Info(Arg.Is<string>(s => s.Contains("does not exist")));
            directoryProvider.DidNotReceive().CreateDirectory(Arg.Any<string>());
        }

        [Fact]
        public void Creates_Destination_Directory()
        {
            directoryProvider.Exists("src").Returns(true);
            fileProvider.GetFiles("src").Returns(Array.Empty<string>());
            directoryProvider.GetDirectories("src").Returns(Array.Empty<string>());

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", Array.Empty<string>());

            directoryProvider.Received().CreateDirectory("dest");
        }

        [Fact]
        public void Copies_Files_Not_In_Exclusion_List()
        {
            directoryProvider.Exists("src").Returns(true);

            fileProvider.GetFiles("src").Returns(new[]
            {
                Path.Combine("src", "file.txt")
            });

            directoryProvider.GetDirectories("src").Returns(Array.Empty<string>());

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", new[] { ".md" });

            fileProvider.Received().Copy(
                Path.Combine("src", "file.txt"),
                Path.Combine("dest", "file.txt"),
                true
            );
        }

        [Fact]
        public void Skips_Excluded_Files()
        {
            directoryProvider.Exists("src").Returns(true);
            fileProvider.GetFiles("src").Returns(new[] { "src/readme.md" });
            directoryProvider.GetDirectories("src").Returns(Array.Empty<string>());

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", new[] { ".md" });

            fileProvider.DidNotReceive().Copy(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>());
            log.Received().Info(Arg.Is<string>(s => s.Contains("Skipping excluded file")));
        }

        [Fact]
        public void Logs_Png_Files_Differently()
        {
            directoryProvider.Exists("src").Returns(true);
            fileProvider.GetFiles("src").Returns(new[] { "src/logo.png" });
            directoryProvider.GetDirectories("src").Returns(Array.Empty<string>());

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", Array.Empty<string>());

            log.Received().Info(Arg.Is<string>(s => s.Contains("Copied PNG asset")));
        }

        [Fact]
        public void Recurses_Into_Subdirectories()
        {
            directoryProvider.Exists("src").Returns(true);

            fileProvider.GetFiles("src").Returns(Array.Empty<string>());
            directoryProvider.GetDirectories("src").Returns(new[] { Path.Combine("src", "sub") });

            directoryProvider.Exists(Path.Combine("src", "sub")).Returns(true);
            fileProvider.GetFiles(Path.Combine("src", "sub")).Returns(Array.Empty<string>());
            directoryProvider.GetDirectories(Path.Combine("src", "sub")).Returns(Array.Empty<string>());

            var helper = CreateHelper();
            helper.CopyRecursive("src", "dest", Array.Empty<string>());

            directoryProvider.Received().CreateDirectory("dest");
            directoryProvider.Received().CreateDirectory(Path.Combine("dest", "sub"));

            log.Received().Info(Arg.Is<string>(s => s.Contains("Entering subfolder")));
        }
    }
}