using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.Models
{
    public class HtmlTable
    {
        public List<HtmlHeaderRow> Headers { get; }
        public List<HtmlRow> Rows { get; }

        public HtmlTable(IEnumerable<HtmlHeaderRow> headers, IEnumerable<HtmlRow> rows)
        {
            Headers = headers.ToList();
            Rows = rows.ToList();
        }
    }

    public class HtmlHeaderRow
    {
        public List<string> Cells { get; }

        public HtmlHeaderRow(IEnumerable<string> cells)
        {
            Cells = cells.ToList();
        }
    }

    public class HtmlRow
    {
        public List<string> Cells { get; }
        public object Payload { get; }

        public HtmlRow(IEnumerable<string> cells, object payload)
        { 
            Cells = cells.ToList();
            Payload = payload;
        }
    }
}
