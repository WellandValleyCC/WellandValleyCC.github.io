using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Utilities;

namespace ClubCore.Utilities
{
    public class DefaultAssetCopier : IAssetCopier
    {
        public string CopyYearSpecificStylesheet(string source, string dest, int year, string prefix)
            => AssetCopier.CopyYearSpecificStylesheet(source, dest, year, prefix);
    }
}