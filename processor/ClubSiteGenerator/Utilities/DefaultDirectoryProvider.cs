using ClubSiteGenerator.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultDirectoryProvider : IDirectoryProvider
    {
        public bool Exists(string path) => Directory.Exists(path);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);

        public string[] GetDirectories(string path)
            => Directory.GetDirectories(path);

    }
}