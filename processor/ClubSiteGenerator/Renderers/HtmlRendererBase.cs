using ClubSiteGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            sb.AppendLine($"  {TitleElement()}");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"../assets/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            // Header with title, date, and navigation
            sb.AppendLine("<header>");
            sb.Append(HeaderHtml());
            sb.AppendLine("</header>");

            // Class specific page legend
            sb.Append(LegendHtml());

            sb.Append(ResultsTableHtml());

            // Close document
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        protected abstract string TitleElement();
        protected abstract string HeaderHtml();
        protected abstract string LegendHtml();
        protected abstract string ResultsTableHtml();
    }
}

