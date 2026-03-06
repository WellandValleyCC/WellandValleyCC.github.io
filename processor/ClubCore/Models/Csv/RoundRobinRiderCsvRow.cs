using CsvHelper.Configuration.Attributes;

namespace ClubCore.Models.Csv
{
    public class RoundRobinRiderCsvRow
    {
        [Name("Name")]
        public string Name { get; set; } = string.Empty;

        [Name("Club")]
        public string Club { get; set; } = string.Empty;

        [Name("Decorated Name")]
        public string DecoratedName { get; set; } = string.Empty;

        [Name("isFemale")]
        public string IsFemale { get; set; } = string.Empty;
    }
}
