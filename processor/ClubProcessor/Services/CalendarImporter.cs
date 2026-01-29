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
                .ToArray();

            var requiredHeaders = new[]
            {
                "Event Number","Date","Start time","Event Name","Round Robin Club", "Miles","Location / Course",
                "Hill Climb","Club Championship","Non-Standard 10","Evening 10","Hard Ride Series","Round Robin Event", "isCancelled"
            };

            foreach (var required in requiredHeaders)
            {
                if (!headers.Any(h => string.Equals(h, required, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("[INFO] Headers: " + string.Join(", ", headers));

                    throw new FormatException(
                        $"Calendar CSV is missing required column: {required}. " +
                        $"Expected columns: {string.Join(", ", requiredHeaders)}");
                }
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
                   a.IsHillClimb == b.IsHillClimb &&
                   a.IsClubChampionship == b.IsClubChampionship &&
                   a.IsNonStandard10 == b.IsNonStandard10 &&
                   a.IsEvening10 == b.IsEvening10 &&
                   a.IsHardRideSeries == b.IsHardRideSeries &&
                   a.IsRoundRobinEvent == b.IsRoundRobinEvent &&
                   a.IsCancelled == b.IsCancelled;
        }

        private void UpdateEvent(CalendarEvent target, CalendarEvent source)
        {
            target.EventDate = source.EventDate;
            target.StartTime = source.StartTime;
            target.EventName = source.EventName;
            target.RoundRobinClub = source.RoundRobinClub;
            target.Miles = source.Miles;
            target.Location = source.Location;
            target.IsHillClimb = source.IsHillClimb;
            target.IsClubChampionship = source.IsClubChampionship;
            target.IsNonStandard10 = source.IsNonStandard10;
            target.IsEvening10 = source.IsEvening10;
            target.IsHardRideSeries = source.IsHardRideSeries;
            target.IsRoundRobinEvent = source.IsRoundRobinEvent;
            target.IsCancelled = source.IsCancelled;
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

            if (string.IsNullOrWhiteSpace(row.EventName))
            {
                Console.WriteLine($"[WARN] Skipping Event {row.EventNumber}: missing event name");
                return null;
            }

            var rrClub = row.RoundRobinClub?.Trim() ?? string.Empty;
            ValidateRoundRobinClub(rrClub, row.EventNumber);

            // Console.WriteLine($"[DEBUG] Event {row.EventNumber}: ClubChamp='{row.ClubChampRaw}', NonStd10='{row.NonStd10Raw}', Evening10='{row.Evening10Raw}', HardRide='{row.HardRideRaw}', Cancelled='{row.CancelledRaw}'");

            return new CalendarEvent
            {
                EventNumber = row.EventNumber,
                EventDate = DateTime.SpecifyKind(eventDate, DateTimeKind.Utc),
                StartTime = startTime,
                EventName = row.EventName,
                RoundRobinClub = row.RoundRobinClub ?? string.Empty,
                Miles = miles,
                Location = row.Location,
                IsHillClimb = IsYes(row.HillClimbRaw),
                IsClubChampionship = IsYes(row.ClubChampRaw),
                IsNonStandard10 = IsYes(row.NonStd10Raw),
                IsEvening10 = IsYes(row.Evening10Raw),
                IsHardRideSeries = IsYes(row.HardRideRaw),
                IsRoundRobinEvent = IsYes(row.RoundRobinEventRaw),
                IsCancelled = IsYes(row.CancelledRaw)
            };
        }

        private void ValidateRoundRobinClub(string rrClub, int eventNumber)
        {
            var normalised = rrClub.Trim().ToUpper();
            
            if (string.IsNullOrWhiteSpace(normalised) || normalised == "WVCC")
                return; // Empty, or `WVCC` is allowed

            var exists = _db.RoundRobinClubs
                .Any(c => c.ShortName.ToUpper() == normalised);

            if (!exists)
            {
                throw new InvalidOperationException(
                    $"Event {eventNumber}: Round Robin Club '{rrClub}' is not a known round robin club.");
            }
        }

        private static bool IsYes(string? raw) =>
            !string.IsNullOrWhiteSpace(raw) &&
            new[] { "Y", "YES", "TRUE" }.Contains(raw.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
