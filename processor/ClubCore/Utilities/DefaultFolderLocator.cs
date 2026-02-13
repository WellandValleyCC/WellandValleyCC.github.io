using ClubCore.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultFolderLocator : IFolderLocator
    {
        private readonly FolderLocator inner;

        public DefaultFolderLocator(
            IDirectoryProvider directoryProvider,
            ILog log)
        {
            inner = new FolderLocator(directoryProvider, log);
        }

        public string FindGitRepoRoot() => inner.FindGitRepoRoot();
        public string GetDataDirectory() => inner.GetDataDirectory();
        public string GetConfigDirectory() => inner.GetConfigDirectory();

    }
}