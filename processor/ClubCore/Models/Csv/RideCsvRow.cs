using ClubCore.Models.Enums;
using CsvHelper.Configuration.Attributes;

namespace ClubCore.Models.Csv
{
    /// <remarks>
    ///   Number/Raw Name,Number/Name,H,M,S,H2,M2,S2,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed
    ///   Jen Hulme,990,,15.0,23.0,,15.0,11.0, r,, Jen Hulme,00:30:34,
    ///   Michaela Latham,1379,,13.0,23.0,,13.0,2.0, r,, Michaela Latham,00:26:25,
    ///   JenSteven Latham,1244,,13.0,17.0,,12.0,52.0, r,, Steven Latham,00:26:09,
    ///   
    /// 
    /// Note that H2, M2, S2 are optional - only present for two-leg events (e.g. 5+5 mile TT)
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

        [Name("H2")]
        public string? Leg2Hours { get; set; }

        [Name("M2")]
        public string? Leg2Minutes { get; set; }

        [Name("S2")]
        public string? Leg2Seconds { get; set; }

        [Name("Roadbike?")]
        public string? RoadBike { get; set; }

        [Name("DNS/DNF/DQ")]
        public string? RideStatusRaw { get; set; }

        [Name("Name")]
        public string? Name { get; set; }

        [Ignore]
        public double Leg1TotalSeconds
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
        public double Leg2TotalSeconds
        {
            get
            {
                int h = double.TryParse(Leg2Hours, out var hh) ? (int)hh : 0;
                int m = double.TryParse(Leg2Minutes, out var mm) ? (int)mm : 0;
                double s = double.TryParse(Leg2Seconds, out var ss) ? ss : 0;

                return h * 3600 + m * 60 + s;
            }
        }

        [Ignore]
        public double TotalSeconds =>
            Leg1TotalSeconds + Leg2TotalSeconds;

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
        public RideStatus RideStatus
        {
            get
            {
                var raw = RideStatusRaw?.Trim().ToUpperInvariant();

                // Explicit statuses always win
                if (raw == "DNS") return RideStatus.DNS;
                if (raw == "DNF") return RideStatus.DNF;
                if (raw == "DQ") return RideStatus.DQ;

                // Detect future event: no status AND no time entered
                bool noTimeEntered =
                    string.IsNullOrWhiteSpace(Hours) &&
                    string.IsNullOrWhiteSpace(Minutes) &&
                    string.IsNullOrWhiteSpace(Seconds);

                if (noTimeEntered)
                    return RideStatus.Ready;

                // No explicit status, but time is present → valid result
                if (string.IsNullOrWhiteSpace(raw))
                    return RideStatus.Valid;

                // Anything else is malformed
                return RideStatus.Undefined;
            }
        }
    }
}
