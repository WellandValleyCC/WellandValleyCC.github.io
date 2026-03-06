using ClubCore.Interfaces;

namespace ClubCore.Utilities
{
    public class DefaultFileProvider : IFileProvider
    {
        public bool Exists(string path) => File.Exists(path);

        public void Copy(string source, string destination, bool overwrite)
            => File.Copy(source, destination, overwrite);

        public string[] GetFiles(string directory)
            => Directory.GetFiles(directory);
    }
}