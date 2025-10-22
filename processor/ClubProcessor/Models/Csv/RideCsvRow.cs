using ClubProcessor.Models.Enums;
using CsvHelper.Configuration.Attributes;

namespace ClubProcessor.Models.Csv
{
    internal class RideCsvRow
    {
        [Name("Number/Name")]
        public string? NumberOrName { get; set; }

        [Name("H")]
        public string? Hours { get; set; }

        [Name("M")]
        public string? Minutes { get; set; }

        [Name("S")]
        public string? Seconds { get; set; }

        [Ignore]
        public double TotalSeconds =>
            (int.TryParse(Hours, out var h) ? h : 0) * 3600 +
            (int.TryParse(Minutes, out var m) ? m : 0) * 60 +
            (double.TryParse(Seconds, out var s) ? s : 0);

        [Ignore]
        public string ActualTime
        {
            get
            {
                int h = int.TryParse(Hours, out var hh) ? hh : 0;
                int m = int.TryParse(Minutes, out var mm) ? mm : 0;
                double s = double.TryParse(Seconds, out var ss) ? ss : 0;

                string secondsFormatted = s % 1 == 0
                    ? $"{(int)s:D2}"          // No decimals if whole number
                    : $"{s:00.##}";           // Up to two decimals if needed

                return $"{h:D2}:{m:D2}:{secondsFormatted}";
            }
        }

        [Name("RoadBike?")]
        public string? RoadBike { get; set; }

        [Ignore]
        public bool IsRoadBike => string.Equals(RoadBike?.Trim(), "r", StringComparison.OrdinalIgnoreCase);


        [Name("DNS/DNF/DQ")]
        public string? EligibilityRaw { get; set; }

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

        // Add more fields as needed
    }
}
