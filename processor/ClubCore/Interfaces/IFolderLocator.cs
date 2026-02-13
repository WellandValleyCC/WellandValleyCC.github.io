namespace ClubCore.Interfaces
{
    public interface IFolderLocator
    {
        string FindGitRepoRoot();
        string GetDataDirectory();
        string GetConfigDirectory();
    }
}