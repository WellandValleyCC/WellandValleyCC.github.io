using ClubCore.Context;
using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubCore.Models.Enums;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubProcessor.Services
{
    public class EventsImporter
    {
        private readonly EventDbContext eventContext;
        private readonly CompetitorDbContext competitorContext;
        private readonly IEnumerable<CalendarEvent> calendar;

        public EventsImporter(
            EventDbContext eventContext, 
            CompetitorDbContext competitorContext,
            IEnumerable<CalendarEvent> calendar)
        {
            this.eventContext = eventContext;
            this.competitorContext = competitorContext;
            this.calendar = calendar;
        }

        public bool ImportFromFolder(string folderPath)
        {
            var folderName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar));

            var yearMatch = Regex.Match(folderName, @"\b(20\d{2})\b");

            if (!yearMatch.Success)
            {
                Console.WriteLine($"[ERROR] Could not extract year from folder name: {folderName}");
                return false;
            }

            var year = int.Parse(yearMatch.Groups[1].Value);

            var eventsDir = Path.Combine(folderPath, "events");

            if (!Directory.Exists(eventsDir))
            {
                Console.WriteLine($"[ERROR] Events folder not found: {eventsDir}");
                return false;
            }

            var manifestPath = Path.Combine(
                folderPath,
                $"EventManifest_{year}.csv");

            if (!File.Exists(manifestPath))
            {
                Console.WriteLine($"[ERROR] Event manifest not found: {manifestPath}");
                return false;
            }

            Console.WriteLine($"[INFO] Processing year: {year}");
            Console.WriteLine($"[INFO] Processing events in: {eventsDir}");
            Console.WriteLine($"[INFO] Using manifest: {manifestPath}");

            var manifest = LoadEventManifest(manifestPath);

            ClearRidesForObsoleteEvents();

            foreach (var manifestEntry in manifest.Values
                .OrderBy(m => m.EventNumber))
            {
                var csvPath = Path.Combine(
                    folderPath,
                    manifestEntry.Filename);

                if (!File.Exists(csvPath))
                {
                    Console.WriteLine(
                        $"[WARN] Event file not found: {csvPath}");
                    continue;
                }

                Console.WriteLine(
                    $"[INFO] Processing Event {manifestEntry.EventNumber}: {csvPath}");

                ProcessEventCsv(
                    csvPath,
                    manifestEntry.EventNumber,
                    year);
            }

            eventContext.SaveChanges();

            return true;
        }

        private void ClearRidesForObsoleteEvents()
        {
            var validEventNumbers = calendar
                .Select(ev => ev.EventNumber)
                .ToHashSet();

            var staleRides = eventContext.Rides
                .Where(r => !validEventNumbers.Contains(r.EventNumber))
                .ToList();

            eventContext.Rides.RemoveRange(staleRides);
            eventContext.SaveChanges();

            Console.WriteLine($"[INFO] Removed {staleRides.Count} obsolete rides.");
        }

        private void ProcessEventCsv(string csvPath, int eventNumber, int competitionYear)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    TrimOptions = TrimOptions.Trim,
                    IgnoreBlankLines = true,
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    PrepareHeaderForMatch = args => (args.Header ?? string.Empty).Trim()
                });

            var calendarEvent = calendar
                .SingleOrDefault(ev => ev.EventNumber == eventNumber);

            if (calendarEvent == null)
            {
                Console.WriteLine(
                    $"[WARN] skipping Event {eventNumber} - no valid entry in Calendar sheet.");
                return;
            }

            ValidateHeaders(
                csv,
                calendarEvent);

            var eventMiles = calendarEvent.Miles;

            var incomingRows = csv.GetRecords<RideCsvRow>().ToList();
            var incomingRides = incomingRows
                .Select(row => ParseRide(row, eventNumber, competitionYear, calendarEvent))
                .Where(ride => ride != null)
                .Cast<Ride>() // assert non-null
                    .Select(r =>
                    {
                        r.AvgSpeed = (r.Status == RideStatus.Valid && r.TotalSeconds > 0 && eventMiles > 0)
                            ? eventMiles / (r.TotalSeconds / 3600.0)
                            : null;
                        return r;
                    })
                .ToList();

            Console.WriteLine($"[INFO] Parsed {incomingRows.Count} rows from {Path.GetFileName(csvPath)}");

            var namelessRows = incomingRows
                .Where(r => string.IsNullOrWhiteSpace(r.Name))
                .ToList();

            if (namelessRows.Any())
            {
                throw new FormatException(
                    $"Event {eventNumber} contains rows with missing Name. Examples: " +
                    string.Join(", ", namelessRows.Take(5).Select(r => r.NumberOrName)));
            }

            foreach (var group in incomingRides.GroupBy(r => r.Name!))
            {
                if (group.Count() > 1)
                {
                    Console.WriteLine($"[WARN] Duplicate ride name '{group.Key}' in Event {eventNumber}. Using first entry.");
                }
            }

            var existingRides = eventContext.Rides
                .Where(r => r.EventNumber == eventNumber)
                .ToList();

            eventContext.Rides.RemoveRange(existingRides);
            eventContext.Rides.AddRange(incomingRides);

            Console.WriteLine($"[INFO] Replaced {existingRides.Count} existing rides with {incomingRides.Count} new rides for Event {eventNumber}");
        }

        private static void ValidateHeaders(
            CsvReader csv,
            CalendarEvent calendarEvent)
        {
            csv.Read();
            csv.ReadHeader();

            var headers = (csv.Context.Reader?.HeaderRecord ?? Array.Empty<string>())
                .Select(h => h.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var requiredHeaders = new[]
            {
                "Number/Name",
                "H",
                "M",
                "S",
                "Roadbike?",
                "DNS/DNF/DQ",
                "Name",
                "Actual Time"
            };

            foreach (var required in requiredHeaders)
            {
                if (!headers.Contains(required))
                {
                    throw new FormatException(
                        $"Event {calendarEvent.EventNumber} CSV is missing required column: {required}");
                }
            }

            bool hasLeg2Columns =
                headers.Contains("H2") &&
                headers.Contains("M2") &&
                headers.Contains("S2");

            bool hasAnyLeg2Column =
                headers.Contains("H2") ||
                headers.Contains("M2") ||
                headers.Contains("S2");

            if (calendarEvent.IsTwoLegEvent)
            {
                if (!hasLeg2Columns)
                {
                    throw new FormatException(
                        $"Event {calendarEvent.EventNumber} is marked as a two-leg event " +
                        "but one or more of H2, M2, S2 are missing.");
                }
            }
            else
            {
                if (hasAnyLeg2Column)
                {
                    throw new FormatException(
                        $"Event {calendarEvent.EventNumber} is not a two-leg event " +
                        "but H2, M2, S2 columns are present.");
                }
            }
        }

        /// <remarks>
        ///   Number/Raw Name,Number/Name,H,M,S,H2,M2,S2,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed
        ///   Jen Hulme,990,,15.0,23.0,,15.0,11.0, r,, Jen Hulme,00:30:34,
        ///   Michaela Latham,1379,,13.0,23.0,,13.0,2.0, r,, Michaela Latham,00:26:25,
        ///   JenSteven Latham,1244,,13.0,17.0,,12.0,52.0, r,, Steven Latham,00:26:09,
        ///   
        /// 
        /// Note that H2, M2, S2 are optional - only present for two-leg events (e.g. 5+5 mile TT)
        /// </remarks>
        private Ride? ParseRide(
            RideCsvRow row,
            int eventNumber,
            int competitionYear,
            CalendarEvent calendarEvent)
        {
            var rawNumberOrName = row.NumberOrName?.Trim();

            bool isClubMember = int.TryParse(rawNumberOrName, out int clubNumber);

            var clubShortName = ResolveClubShortName(row, isClubMember, competitionYear);

            var leg1TotalSeconds = row.Leg1TotalSeconds;
            var leg2TotalSeconds = row.Leg2TotalSeconds;

            var totalSeconds = leg1TotalSeconds + leg2TotalSeconds;

            var leg1Miles = calendarEvent.Leg1Miles;
            var leg2Miles = calendarEvent.Leg2Miles ?? 0.0;

            var totalMiles = leg1Miles + leg2Miles;

            return new Ride
            {
                EventNumber = eventNumber,
                Name = row.Name,
                ClubNumber = isClubMember ? clubNumber : null,
                RoundRobinClub = clubShortName,

                Leg1TotalSeconds = leg1TotalSeconds,
                Leg2TotalSeconds = leg2TotalSeconds,

                AvgSpeed = CalculateSpeed(totalMiles, totalSeconds),
                Leg1AvgSpeed = CalculateSpeed(leg1Miles, leg1TotalSeconds),
                Leg2AvgSpeed = CalculateSpeed(leg2Miles, leg2TotalSeconds),

                IsRoadBike = row.IsRoadBike,
                Status = row.RideStatus,
            };
        }

        private static double? CalculateSpeed(double miles, double totalSeconds) =>
            totalSeconds > 0
                ? miles / (totalSeconds / 3600.0)
                : null;

        private string? ResolveClubShortName(RideCsvRow row, bool isClubMember, int competitionYear)
        {
            if (isClubMember)
                return "WVCC"; // default for members

            // NEW: 2025 open competition rule
            if (competitionYear == 2025)
            {
                var clubName = ExtractClubNameFromName(row.Name);

                // No suffix → Guest
                if (clubName is null)
                    return "Guest";

                // Has suffix → check if it's a valid RR club for 2025
                var shortName = LookupClubShortName(clubName, competitionYear);

                // Invalid or unknown → Guest
                return shortName ?? "Guest";
            }

            // Default behaviour for other years
            var extracted = ExtractClubNameFromName(row.Name);
            if (extracted is null)
                return null;

            return LookupClubShortName(extracted, competitionYear);
        }

        private string? ExtractClubNameFromName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            // Look for "(XYZ)" at the end of the name
            var start = name.LastIndexOf('(');
            var end = name.LastIndexOf(')');

            if (start < 0 || end < 0 || end <= start)
                return null;

            var clubName = name.Substring(start + 1, end - start - 1).Trim();
            return string.IsNullOrWhiteSpace(clubName) ? null : clubName;
        }

        private string? LookupClubShortName(string clubName, int competitionYear)
        {
            var normalised = clubName.Trim().ToUpperInvariant();

            var club = eventContext.RoundRobinClubs
                .FirstOrDefault(c =>
                    c.ShortName.ToUpper() == normalised &&
                    c.FromYear <= competitionYear);

            return club?.ShortName;
        }

        private Dictionary<string, EventManifestCsv> LoadEventManifest(string manifestPath)
        {
            using var reader = new StreamReader(manifestPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            return csv.GetRecords<EventManifestCsv>()
                .ToDictionary(
                    m => Path.GetFileName(m.Filename),
                    m => m,
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}

