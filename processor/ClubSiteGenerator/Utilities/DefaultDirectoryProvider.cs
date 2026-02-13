using ClubSiteGenerator.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultDirectoryProvider : IDirectoryProvider
    {
        public string[] GetDirectories(string path)
            => Directory.GetDirectories(path);
    }
}