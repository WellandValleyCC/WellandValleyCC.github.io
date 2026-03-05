using ClubSiteGenerator.Models.Enums;

namespace ClubSiteGenerator.Utilities
{
    public static class GoogleAnalytics
    {
        // Measurement IDs for each brand
        private const string WvccMeasurementId = "";          // TODO: replace with real WVCC ID
        private const string RoundRobinMeasurementId = "G-LHTWNBQEQ8";    // Round Robin ID

        public static string GetAnalyticsSnippet(SiteBrand site)
        {
            var id = site switch
            {
                SiteBrand.Wvcc => WvccMeasurementId,
                SiteBrand.RoundRobin => RoundRobinMeasurementId,
                SiteBrand.Undefined => string.Empty,
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(id))
                return string.Empty;

            return $@"
<!-- Google tag (gtag.js) -->
<script async src=""https://www.googletagmanager.com/gtag/js?id={id}""></script>
<script>
  window.dataLayer = window.dataLayer || [];
  function gtag(){{dataLayer.push(arguments);}}
  gtag('js', new Date());
  gtag('config', '{id}');
</script>";
        }
    }
}