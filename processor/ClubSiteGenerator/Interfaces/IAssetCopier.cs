namespace ClubSiteGenerator.Interfaces
{
    public interface IAssetCopier
    {
        string CopyYearSpecificStylesheet(string source, string dest, int year, string prefix);
    }
}
