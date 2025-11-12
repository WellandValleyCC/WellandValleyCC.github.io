namespace ClubSiteGenerator.Models
{
    public class HtmlTable
    {
        public List<string> Headers { get; }
        public List<List<string>> Rows { get; }

        public HtmlTable(List<string> headers, List<List<string>> rows)
        {
            Headers = headers ?? throw new ArgumentNullException(nameof(headers));
            Rows = rows ?? throw new ArgumentNullException(nameof(rows));
        }
    }
}
