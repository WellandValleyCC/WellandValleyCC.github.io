using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.Utilities;
using System.Text;

namespace ClubSiteGenerator.Renderers
{
    public abstract class HtmlRendererBase
    {
        protected abstract string PageTypeClass { get; }

        protected string IndexFileName { get; }

        public string CssFile { get; set; } = "";

        protected HtmlRendererBase(string indexFileName)
        {
            IndexFileName = indexFileName;
        }

        public string Render()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            sb.AppendLine($"  {TitleElement()}");
            sb.AppendLine($"  <link rel=\"stylesheet\" href=\"../assets/{CssFile}\">");
            sb.AppendLine(RenderFaviconLinks());
            var extras = HeadExtras();
            if (!string.IsNullOrEmpty(extras)) sb.AppendLine(extras);
            sb.AppendLine(GoogleAnalytics.GetAnalyticsSnippet(SiteBrand.Wvcc));
            sb.AppendLine("</head>");
            sb.AppendLine($"<body class=\"{PageTypeClass}\">");

            // Header with title, nav, and rules (includes legend if needed)
            sb.AppendLine("<header>");
            sb.AppendLine("<div class=\"wvcc-banner-header\">");
            sb.Append(HeaderHtml());
            sb.AppendLine("</div>");
            sb.AppendLine("</header>");

            sb.Append(ResultsTableHtml());
            sb.Append(FooterHtml());
            
            // Close document
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        protected abstract string TitleElement();
        protected abstract string HeaderHtml();
        protected virtual string HeadExtras() => string.Empty;
        protected abstract string ResultsTableHtml();
        protected virtual string FooterHtml()
        {
            var timestamp = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm 'UTC'");
            return $"<footer><p class=\"generated\">Generated {timestamp}</p></footer>";
        }

        /// <summary>
        /// Emits the standard favicon and touch‑icon HTML.
        /// Renderers can override if they need custom behaviour.
        /// </summary>
        protected virtual string RenderFaviconLinks() => @"
<link rel=""icon"" href=""../assets/favicon.svg"" type=""image/svg+xml"">
<link rel=""icon"" sizes=""32x32"" href=""../assets/favicon-32.png"">
<link rel=""icon"" sizes=""16x16"" href=""../assets/favicon-16.png"">
<link rel=""apple-touch-icon"" sizes=""180x180"" href=""../assets/apple-touch-icon.png"">";
    }
}

