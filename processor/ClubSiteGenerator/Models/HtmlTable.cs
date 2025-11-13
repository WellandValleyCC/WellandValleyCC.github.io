using ClubCore.Models;

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
        public Ride Ride { get; }

        public HtmlRow(IEnumerable<string> cells, Ride ride)
        {
            Cells = cells.ToList();
            Ride = ride;
        }
    }
}
