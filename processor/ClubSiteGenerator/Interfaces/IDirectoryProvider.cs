namespace ClubSiteGenerator.Interfaces
{
    public interface IDirectoryProvider
    {
        bool Exists(string path);
        void CreateDirectory(string path);
        string[] GetDirectories(string path);
    }
}
