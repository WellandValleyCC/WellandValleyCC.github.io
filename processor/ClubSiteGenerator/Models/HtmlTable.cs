using ClubCore.Models;
using ClubSiteGenerator.ResultsGenerator;

namespace ClubSiteGenerator.Models
{
    public class HtmlTable
    {
        public List<string> Headers { get; }
        public List<HtmlRow> Rows { get; }

        public HtmlTable(IEnumerable<string> headers, IEnumerable<HtmlRow> rows)
        {
            Headers = headers.ToList();
            Rows = rows.ToList();
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
