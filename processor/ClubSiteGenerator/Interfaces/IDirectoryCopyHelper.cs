namespace ClubSiteGenerator.Interfaces
{
    public interface IDirectoryCopyHelper
    {
        void CopyRecursive(string source, string dest, string[] excludeExtensions);
    }
}
