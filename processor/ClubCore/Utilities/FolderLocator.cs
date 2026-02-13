using ClubCore.Interfaces;

namespace ClubCore.Utilities
{
    public class FolderLocator : IFolderLocator
    {
        private readonly IDirectoryProvider directoryProvider;
        private readonly ILog log;

        public FolderLocator(IDirectoryProvider directoryProvider, ILog log)
        {
            this.directoryProvider = directoryProvider;
            this.log = log;
        }

        public string FindGitRepoRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null)
            {
                var gitPath = Path.Combine(dir.FullName, ".git");

                if (directoryProvider.Exists(gitPath))
                {
                    log.Info($"Git repo root found at: {dir.FullName}");
                    return dir.FullName;
                }

                dir = dir.Parent;
            }

            throw new DirectoryNotFoundException(
                "Could not locate Git repo root (no .git folder found)");
        }

        public string GetDataDirectory()
        {
            var repoRoot = FindGitRepoRoot();
            var dataDir = Path.Combine(repoRoot, PathTokens.DataFolder);

            if (!directoryProvider.Exists(dataDir))
                throw new DirectoryNotFoundException(
                    $"Data directory not found at {dataDir}");

            return dataDir;
        }

        public string GetConfigDirectory()
        {
            var repoRoot = FindGitRepoRoot();
            var configDir = Path.Combine(repoRoot, PathTokens.ConfigFolder);

            if (!directoryProvider.Exists(configDir))
                throw new DirectoryNotFoundException(
                    $"Config directory not found at {configDir}");

            return configDir;
        }
    }
}
