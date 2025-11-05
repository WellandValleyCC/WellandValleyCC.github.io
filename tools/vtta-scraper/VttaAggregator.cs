using System.Collections.Generic;
using System.Linq;

namespace vttaScraper
{
    public class VttaAggregator
    {
        private readonly Dictionary<int, Dictionary<string, string>> data = new();

        public void Add(string distance, List<VttaRow> rows)
        {
            foreach (var row in rows)
            {
                if (!data.ContainsKey(row.Age))
                    data[row.Age] = new Dictionary<string, string>();

                data[row.Age][$"m{distance}"] = row.Open;
                data[row.Age][$"f{distance}"] = row.Female;
            }
        }

        public Dictionary<int, Dictionary<string, string>> GetMerged() => data;
    }
}