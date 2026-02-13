using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Utilities;

namespace ClubCore.Utilities
{
    public class DefaultDirectoryCopyHelper : IDirectoryCopyHelper
    {
        public void CopyRecursive(string source, string dest, string[] excludeExtensions)
            => DirectoryCopyHelper.CopyRecursive(source, dest, excludeExtensions);
    }
}