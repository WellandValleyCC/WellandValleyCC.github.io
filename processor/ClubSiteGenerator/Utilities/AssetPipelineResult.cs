namespace ClubSiteGenerator.Utilities
{
    /// <summary>
    /// Result of running the asset pipeline for the Round Robin site.
    /// Currently exposes the selected CSS filename, but can be extended
    /// with additional metadata in the future.
    /// </summary>
    public sealed class AssetPipelineResult
    {
        public string CssFile { get; init; } = default!;
    }
}