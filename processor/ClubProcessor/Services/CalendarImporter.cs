using ClubCore.Context;
using ClubCore.Models;
using ClubCore.Models.Csv;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace ClubProcessor.Services
{
    public class CalendarImporter
    {
        private readonly EventDbContext _db;

        public CalendarImporter(EventDbContext db)
        {
            _db = db;
        }

        public (int addedCount, int updatedCount, int deletedCount) ImportFromCsv(string csvPath)
        {
            var incoming = ParseCsv(csvPath);

            var existing = _db.CalendarEvents.ToList();
            var incomingByNumber = incoming.ToDictionary(e => e.EventNumber);
            var existingByNumber = existing.ToDictionary(e => e.EventNumber);

            var toAdd = new List<CalendarEvent>();
            var toUpdate = new List<CalendarEvent>();
            var toDelete = new List<CalendarEvent>();

            foreach (var evt in incoming)
            {
                if (existingByNumber.TryGetValue(evt.EventNumber, out var match))
                {
                    if (!EventsAreEqual(match, evt))
                    {
                        Console.WriteLine($"[UPDATE] Event {evt.EventNumber}: {match.EventName} → {evt.EventName}");
                        UpdateEvent(match, evt);
                        toUpdate.Add(match);
                    }
                }
                else
                {
                    Console.WriteLine($"[ADD] Event {evt.EventNumber}: {evt.EventName}");
                    toAdd.Add(evt);
                }
            }

            foreach (var evt in existing)
            {
                if (!incomingByNumber.ContainsKey(evt.EventNumber))
                {
                    Console.WriteLine($"[DELETE] Event {evt.EventNumber}: {evt.EventName}");
                    toDelete.Add(evt);
                }
            }

            _db.CalendarEvents.RemoveRange(toDelete);
            _db.CalendarEvents.UpdateRange(toUpdate);
            _db.CalendarEvents.AddRange(toAdd);
            _db.SaveChanges();

            return (toAdd.Count, toUpdate.Count, toDelete.Count);
        }

        private static void ValidateHeaders(CsvReader csv)
        {
            csv.Read();
            csv.ReadHeader();

            var headers = (csv.Context.Reader?.HeaderRecord ?? Array.Empty<string>())
                .Select(h => h.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var requiredHeaders = new[]
            {
        "Event Number",
        "Date",
        "Start time",
        "Event Name",
        "Round Robin Club",
        "Miles",
        "Location / Course",
        "Hill Climb",
        "Club Championship",
        "Non-Standard 10",
        "Evening 10",
        "Hard Ride Series",
        "Round Robin Event",
        "isCancelled"
    };

            foreach (var required in requiredHeaders)
            {
                if (!headers.Contains(required))
                {
                    Console.WriteLine(
                        "[INFO] Headers: " +
                        string.Join(", ", headers));

                    throw new FormatException(
                        $"Calendar CSV is missing required column: {required}. " +
                        $"Expected columns: {string.Join(", ", requiredHeaders)}");
                }
            }

            ValidateOptionalHeader(
                headers,
                "isPostponed",
                "defaulting all events to not postponed - i.e. blank");

            ValidateOptionalHeader(
                headers,
                "Shortened Ten",
                "defaulting all evening 10 events to handicap-setting Tens - i.e. blank");

            ValidateOptionalHeader(
                headers,
                "Two Leg Event",
                "defaulting all events to single leg events - i.e. blank");

            ValidateOptionalHeader(
                headers,
                "Leg 1 Name",
                "defaulting all events to single leg events - i.e. blank");

            ValidateOptionalHeader(
                headers,
                "Leg 2 Name",
                "defaulting all events to single leg events - i.e. blank");

            ValidateOptionalHeader(
                headers,
                "Leg 1 Miles",
                "defaulting all events to use Miles value");

            ValidateOptionalHeader(
                headers,
                "Leg 2 Miles",
                "defaulting all events to single leg events");
        }

        private static void ValidateOptionalHeader(
            HashSet<string> headers,
            string headerName,
            string defaultBehaviour)
        {
            if (!headers.Contains(headerName))
            {
                Console.WriteLine(
                    $"[INFO] '{headerName}' column missing — {defaultBehaviour}");
            }

            if (!headers.Any(h => string.Equals(h, "NevBrooksBenchmarkSeconds", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("[INFO] 'NevBrooksBenchmarkSeconds' column missing — defaulting all events to null - i.e. blank.");
            }
        }

        private bool EventsAreEqual(CalendarEvent a, CalendarEvent b)
        {
            return a.EventDate == b.EventDate &&
                   a.StartTime == b.StartTime &&
                   a.EventName == b.EventName &&
                   a.RoundRobinClub == b.RoundRobinClub &&
                   a.Miles == b.Miles &&
                   a.Location == b.Location &&
                   a.NevBrooksBenchmarkSeconds == b.NevBrooksBenchmarkSeconds &&
                   a.IsHillClimb == b.IsHillClimb &&
                   a.IsClubChampionship == b.IsClubChampionship &&
                   a.IsNonStandard10 == b.IsNonStandard10 &&
                   a.IsEvening10 == b.IsEvening10 &&
                   a.IsShortenedTen == b.IsShortenedTen &&
                   a.IsHardRideSeries == b.IsHardRideSeries &&
                   a.IsRoundRobinEvent == b.IsRoundRobinEvent &&
                   a.IsTwoLegEvent == b.IsTwoLegEvent &&
                   a.Leg1Name == b.Leg1Name &&
                   a.Leg2Name == b.Leg2Name &&
                   a.Leg1Miles == b.Leg1Miles &&
                   a.Leg2Miles == b.Leg2Miles &&  
                   a.IsCancelled == b.IsCancelled &&
                   a.IsPostponed == b.IsPostponed;
        }

        private void UpdateEvent(CalendarEvent target, CalendarEvent source)
        {
            target.EventDate = source.EventDate;
            target.StartTime = source.StartTime;
            target.EventName = source.EventName;
            target.RoundRobinClub = source.RoundRobinClub;
            target.Miles = source.Miles;
            target.Location = source.Location;
            target.NevBrooksBenchmarkSeconds = source.NevBrooksBenchmarkSeconds;
            target.IsHillClimb = source.IsHillClimb;
            target.IsClubChampionship = source.IsClubChampionship;
            target.IsNonStandard10 = source.IsNonStandard10;
            target.IsEvening10 = source.IsEvening10;
            target.IsShortenedTen = source.IsShortenedTen;
            target.IsHardRideSeries = source.IsHardRideSeries;
            target.IsRoundRobinEvent = source.IsRoundRobinEvent;
            target.IsTwoLegEvent = source.IsTwoLegEvent;
            target.Leg1Name = source.Leg1Name;
            target.Leg2Name = source.Leg2Name;
            target.Leg1Miles = source.Leg1Miles;
            target.Leg2Miles = source.Leg2Miles;
            target.IsCancelled = source.IsCancelled;
            target.IsPostponed = source.IsPostponed;
        }

        private IReadOnlyList<CalendarEvent> ParseCsv(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                PrepareHeaderForMatch = args => (args.Header ?? string.Empty).Trim()
            });

            ValidateHeaders(csv);

            var rows = csv.GetRecords<CalendarEventCsv>().ToList();

            ValidateNoDuplicateEvents(rows);

            return rows
                .Select(r => MapRowToCalendarEvent(r))
                .Where(ev => ev != null)
                .ToList()!;
        }

        private static void ValidateNoDuplicateEvents(IEnumerable<CalendarEventCsv> rows)
        {
            var duplicates = rows.GroupBy(r => r.EventNumber).Where(g => g.Count() > 1).ToList();
            if (duplicates.Any())
            {
                var dupList = string.Join(", ", duplicates.Select(d => d.Key));
                throw new FormatException($"Calendar CSV contains duplicate EventNumber rows: {dupList}");
            }
        }

        private CalendarEvent? MapRowToCalendarEvent(CalendarEventCsv row)
        {
            if (!DateTime.TryParse(row.DateRaw, out var eventDate))
            {
                Console.WriteLine($"[WARN] Skipping Event {row.EventNumber}: invalid date '{row.DateRaw}'");
                return null;
            }

            if (!TimeSpan.TryParse(row.StartTimeRaw, out var startTime))
            {
                Console.WriteLine($"[WARN] Skipping Event {row.EventNumber}: invalid start time '{row.StartTimeRaw}'");
                return null;
            }

            if (!double.TryParse(row.MilesRaw, out var miles))
            {
                Console.WriteLine($"[WARN] Skipping Event {row.EventNumber}: invalid miles '{row.MilesRaw}'");
                return null;
            }

            double leg1Miles;

            if (!string.IsNullOrWhiteSpace(row.Leg1MilesRaw))
            {
                if (!double.TryParse(row.Leg1MilesRaw, out leg1Miles))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: invalid Leg 1 Miles '{row.Leg1MilesRaw}'");

                    return null;
                }
            }
            else
            {
                // Legacy CSV support
                leg1Miles = miles;
            }

            double? leg2Miles = null;

            if (!string.IsNullOrWhiteSpace(row.Leg2MilesRaw))
            {
                if (!double.TryParse(row.Leg2MilesRaw, out var parsedLeg2Miles))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: invalid leg2 miles '{row.Leg2MilesRaw}'");

                    return null;
                }

                leg2Miles = parsedLeg2Miles;
            }

            bool isTwoLegEvent = IsYes(row.TwoLegEventRaw);

            if (isTwoLegEvent)
            {
                if (string.IsNullOrWhiteSpace(row.Leg1Name))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: two-leg event is missing Leg 1 Name");

                    return null;
                }

                if (string.IsNullOrWhiteSpace(row.Leg2Name))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: two-leg event is missing Leg 2 Name");

                    return null;
                }

                if (!leg2Miles.HasValue)
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: two-leg event is missing Leg 2 Miles");

                    return null;
                }
            }
            else
            {
                if (leg2Miles.HasValue)
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: single-leg event has Leg 2 Miles populated");

                    return null;
                }

                if (!string.IsNullOrWhiteSpace(row.Leg2Name))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: single-leg event has Leg 2 Name populated");

                    return null;
                }
            }

            var expectedMiles = leg1Miles + (leg2Miles ?? 0.0);

            if (Math.Abs(expectedMiles - miles) > 0.001)
            {
                Console.WriteLine(
                    $"[WARN] Skipping Event {row.EventNumber}: " +
                    $"Miles ({miles}) does not equal " +
                    $"Leg 1 Miles ({leg1Miles}) + Leg 2 Miles ({leg2Miles ?? 0.0})");

                return null;
            }

            if (string.IsNullOrWhiteSpace(row.EventName))
            {
                Console.WriteLine($"[WARN] Skipping Event {row.EventNumber}: missing event name");
                return null;
            }

            var rrClub = row.RoundRobinClub?.Trim() ?? string.Empty;
            ValidateRoundRobinClub(rrClub, row.EventNumber);

            // Console.WriteLine($"[DEBUG] Event {row.EventNumber}: ClubChamp='{row.ClubChampRaw}', NonStd10='{row.NonStd10Raw}', Evening10='{row.Evening10Raw}', HardRide='{row.HardRideRaw}', Cancelled='{row.CancelledRaw}'");

            int? nevBrooksBenchmarkSeconds = null;

            if (!string.IsNullOrWhiteSpace(row.NevBrooksBenchmarkSecondsRaw))
            {
                if (!double.TryParse(row.NevBrooksBenchmarkSecondsRaw, out var parsedValue))
                {
                    Console.WriteLine(
                        $"[WARN] Skipping Event {row.EventNumber}: invalid NevBrooksBenchmarkSeconds '{row.NevBrooksBenchmarkSecondsRaw}'");
                    return null;
                }

                nevBrooksBenchmarkSeconds = (int)Math.Round(parsedValue);
            }

            return new CalendarEvent
            {
                EventNumber = row.EventNumber,
                EventDate = DateTime.SpecifyKind(eventDate, DateTimeKind.Utc),
                StartTime = startTime,
                EventName = row.EventName,
                RoundRobinClub = row.RoundRobinClub ?? string.Empty,
                Miles = miles,
                Location = row.Location,
                NevBrooksBenchmarkSeconds = nevBrooksBenchmarkSeconds,
                IsHillClimb = IsYes(row.HillClimbRaw),
                IsClubChampionship = IsYes(row.ClubChampRaw),
                IsNonStandard10 = IsYes(row.NonStd10Raw),
                IsEvening10 = IsYes(row.Evening10Raw),
                IsShortenedTen = IsYes(row.ShortenedTenRaw),
                IsHardRideSeries = IsYes(row.HardRideRaw),
                IsRoundRobinEvent = IsYes(row.RoundRobinEventRaw),
                IsTwoLegEvent = isTwoLegEvent,
                Leg1Name = row.Leg1Name ?? string.Empty,
                Leg2Name = row.Leg2Name ?? string.Empty,
                Leg1Miles = leg1Miles,
                Leg2Miles = leg2Miles,
                IsCancelled = IsYes(row.CancelledRaw),
                IsPostponed = IsYes(row.PostponedRaw)
            };
        }

        private void ValidateRoundRobinClub(string rrClub, int eventNumber)
        {
            if (string.IsNullOrWhiteSpace(rrClub))
                return; // Empty allowed

            // Split CSV list: "Ratae, WVCC" --> ["Ratae", "WVCC"]
            var clubs = rrClub
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToList();

            foreach (var club in clubs)
            {
                var normalised = club.ToUpperInvariant();

                // WVCC always allowed
                if (normalised == "WVCC")
                    continue;

                var exists = _db.RoundRobinClubs
                    .Any(c => c.ShortName.ToUpper() == normalised);

                if (!exists)
                {
                    throw new InvalidOperationException(
                        $"Event {eventNumber}: Round Robin Club '{club}' is not a known round robin club.");
                }
            }
        }

        private static bool IsYes(string? raw) =>
            !string.IsNullOrWhiteSpace(raw) &&
            new[] { "Y", "YES", "TRUE" }.Contains(raw.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
