namespace ClubSiteGenerator.Interfaces
{
    public interface IResultsSet
    {
        string DisplayName { get; }
        string FileName { get; }
        string SubFolderName { get; }
        string LinkText { get; }


        string? PrevLink { get; set; }
        string? NextLink { get; set; }
        string? PrevLabel { get; set; }
        string? NextLabel { get; set; }
    }
}
