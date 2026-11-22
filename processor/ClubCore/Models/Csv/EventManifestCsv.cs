using CsvHelper.Configuration.Attributes;

namespace ClubCore.Models.Csv
{
    public sealed class EventManifestCsv
    {
        [Name("EventNumber")]
        public int EventNumber { get; set; }

        [Name("SheetName")]
        public string SheetName { get; set; } = string.Empty;

        [Name("Filename")]
        public string Filename { get; set; } = string.Empty;
    }
}
