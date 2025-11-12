using ClubCore.Models.Enums;
using CsvHelper.Configuration.Attributes;

namespace ClubCore.Models.Csv
{
    /// <remarks>
    ///   Number/Name,H,M,S,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed
    ///   9999,0.0,24.0,18.0,r,DNF,John Doe,00:24:18,
    ///   Johnny Doe,0.0,27.0,30.0,,,Johnny Doe,00:27:30,X             * 
    /// </remarks>
    public class RideCsvRow
    {
        [Name("Number/Name")]
        public string? NumberOrName { get; set; }

        [Name("H")]
        public string? Hours { get; set; }

        [Name("M")]
        public string? Minutes { get; set; }

        [Name("S")]
        public string? Seconds { get; set; }

        [Name("Roadbike?")]
        public string? RoadBike { get; set; }

        [Name("DNS/DNF/DQ")]
        public string? EligibilityRaw { get; set; }

        [Name("Name")]
        public string? Name { get; set; }

        [Ignore]
        public double TotalSeconds
        {
            get
            {
                int h = double.TryParse(Hours, out var hh) ? (int)hh : 0;
                int m = double.TryParse(Minutes, out var mm) ? (int)mm : 0;
                double s = double.TryParse(Seconds, out var ss) ? ss : 0;

                return h * 3600 + m * 60 + s;
            }
        }

        [Ignore]
        public string ActualTime
        {
            get
            {
                var actualTime = TimeSpan.FromSeconds(TotalSeconds);

                string secondsFormatted = actualTime.Seconds % 1 == 0
                    ? $"{actualTime.Seconds:D2}"
                    : $"{actualTime.Seconds + actualTime.Milliseconds / 1000.0:00.##}";

                return $"{actualTime.Hours:D2}:{actualTime.Minutes:D2}:{secondsFormatted}";
            }
        }

        [Ignore]
        public bool IsRoadBike => string.Equals(RoadBike?.Trim(), "r", StringComparison.OrdinalIgnoreCase);

        [Ignore]
        public RideEligibility Eligibility
        {
            get
            {
                var raw = EligibilityRaw?.Trim().ToUpperInvariant();

                return raw switch
                {
                    "DNS" => RideEligibility.DNS,
                    "DNF" => RideEligibility.DNF,
                    "DQ" => RideEligibility.DQ,
                    "" => RideEligibility.Valid,
                    null => RideEligibility.Valid,
                    _ => RideEligibility.Undefined
                };
            }
        }
    }
}
