using ClubCore.Interfaces;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Utilities;

namespace ClubCore.Utilities
{
    public class DefaultDirectoryCopyHelper : IDirectoryCopyHelper
    {
        private readonly DirectoryCopyHelper helper;

        public DefaultDirectoryCopyHelper(
            IDirectoryProvider directoryProvider,
            IFileProvider fileProvider,
            ILog log)
        {
            helper = new DirectoryCopyHelper(directoryProvider, fileProvider, log);
        }

        public void CopyRecursive(string source, string dest, string[] excludeExtensions)
            => helper.CopyRecursive(source, dest, excludeExtensions);
    }
}