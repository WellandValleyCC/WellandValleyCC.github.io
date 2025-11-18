using System.Text;

namespace ClubSiteGenerator.Renderers
{
    public abstract class HtmlRendererBase
    {
        public string Render()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            sb.AppendLine($"  {TitleElement()}");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"../assets/styles.css\">");
            var extras = HeadExtras();
            if (!string.IsNullOrEmpty(extras)) sb.AppendLine(extras);
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header with title, date, and navigation
            sb.AppendLine("<header>");
            sb.Append(HeaderHtml());
            sb.AppendLine("</header>");

            // Class specific page legend
            sb.Append(LegendHtml());

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
        protected abstract string LegendHtml();
        protected abstract string ResultsTableHtml();
        protected virtual string FooterHtml()
        {
            var timestamp = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm 'UTC'");
            return $"<footer><p class=\"generated\">Generated {timestamp}</p></footer>";
        }

    }
}

