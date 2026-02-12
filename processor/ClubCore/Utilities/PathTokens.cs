namespace ClubCore.Utilities
{
    /// <summary>
    /// Centralised canonical folder names used across generators,
    /// processors, and asset pipelines. Keeps string literals out of code.
    /// </summary>
    public static class PathTokens
    {
        // Folder names
        public const string ClubSiteOutputFolder = "SiteOutput";
        public const string ClubAssetsFolder = "ClubSiteAssets";
        
        public const string RoundRobinOutputFolder = "RoundRobinSiteOutput";
        public const string RoundRobinAssetsFolder = "RoundRobinSiteAssets";

        public const string DataFolder = "data";
        public const string ConfigFolder = "config";

        public const string AssetsFolder = "assets";
        public const string LogosFolder = "logos";

        // Filename prefixes
        public const string RoundRobinCssPrefix = "roundrobin";

        // File extensions
        public const string CssExtension = ".css";
        public const string MarkdownExtension = ".md";
    }
}
