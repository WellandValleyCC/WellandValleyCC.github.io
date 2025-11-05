using System.Text;

namespace vttaScraper
{
    public static class CsvWriter
    {
        public static void Write(string path, Dictionary<int, Dictionary<string, string>> data)
        {
            // Separate and sort male and female keys by numeric distance
            var maleKeys = data.Values
                .SelectMany(d => d.Keys)
                .Where(k => k.StartsWith("m"))
                .Distinct()
                .OrderBy(k => double.Parse(k.Substring(1)))
                .ToList();

            var femaleKeys = data.Values
                .SelectMany(d => d.Keys)
                .Where(k => k.StartsWith("f"))
                .Distinct()
                .OrderBy(k => double.Parse(k.Substring(1)))
                .ToList();

            var allKeys = maleKeys.Concat(femaleKeys).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Age," + string.Join(",", allKeys));

            foreach (var age in data.Keys.OrderBy(k => k))
            {
                var row = new List<string> { age.ToString() };
                foreach (var key in allKeys)
                    row.Add(data[age].TryGetValue(key, out var val) ? val : "");

                sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private static string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
                return $"\"{s.Replace("\"", "\"\"")}\"";
            return s;
        }
    }
}
