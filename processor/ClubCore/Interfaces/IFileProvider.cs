namespace ClubCore.Interfaces
{
    public interface IFileProvider
    {
        bool Exists(string path);
        void Copy(string source, string destination, bool overwrite);
        string[] GetFiles(string directory);
    }
}