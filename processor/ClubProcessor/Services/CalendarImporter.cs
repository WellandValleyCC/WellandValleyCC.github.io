using ClubData.Models;
using ClubProcessor.Context;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        public List<CalendarEvent> ParseCalendarEvents(TextReader reader)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                TrimOptions = TrimOptions.Trim
            };

            using var csv = new CsvReader(reader, config);
            csv.Read();
            csv.ReadHeader();

            var records = new List<CalendarEvent>();
            while (csv.Read())
            {
                var record = TryParseCalendarEvent(csv);
                if (record != null)
                {
                    records.Add(record);
                }
            }

            return records;
        }

        public void ImportFromCsv(string csvPath)
        {
            using var reader = new StreamReader(csvPath);
            var incoming = ParseCalendarEvents(reader);

            var existing = _db.CalendarEvents.ToList();
            var incomingById = incoming.ToDictionary(e => e.EventID);
            var existingById = existing.ToDictionary(e => e.EventID);

            var toAdd = new List<CalendarEvent>();
            var toUpdate = new List<CalendarEvent>();
            var toDelete = new List<CalendarEvent>();

            foreach (var evt in incoming)
            {
                if (existingById.TryGetValue(evt.EventID, out var match))
                {
                    if (!EventsAreEqual(match, evt))
                    {
                        Console.WriteLine($"[UPDATE] Event {evt.EventID}: {match.EventName} → {evt.EventName}");
                        UpdateEvent(match, evt);
                        toUpdate.Add(match);
                    }
                }
                else
                {
                    Console.WriteLine($"[ADD] Event {evt.EventID}: {evt.EventName}");
                    toAdd.Add(evt);
                }
            }

            foreach (var evt in existing)
            {
                if (!incomingById.ContainsKey(evt.EventID))
                {
                    Console.WriteLine($"[DELETE] Event {evt.EventID}: {evt.EventName}");
                    toDelete.Add(evt);
                }
            }

            _db.CalendarEvents.RemoveRange(toDelete);
            _db.CalendarEvents.UpdateRange(toUpdate);
            _db.CalendarEvents.AddRange(toAdd);
            _db.SaveChanges();
        }

        private bool EventsAreEqual(CalendarEvent a, CalendarEvent b)
        {
            return a.EventDate == b.EventDate &&
                   a.StartTime == b.StartTime &&
                   a.EventName == b.EventName &&
                   a.Miles == b.Miles &&
                   a.Location == b.Location &&
                   a.IsHillClimb == b.IsHillClimb &&
                   a.IsClubChampionship == b.IsClubChampionship &&
                   a.IsNonStandard10 == b.IsNonStandard10 &&
                   a.IsEvening10 == b.IsEvening10 &&
                   a.IsHardRideSeries == b.IsHardRideSeries &&
                   a.SheetName == b.SheetName &&
                   a.IsCancelled == b.IsCancelled;
        }

        private void UpdateEvent(CalendarEvent target, CalendarEvent source)
        {
            target.EventDate = source.EventDate;
            target.StartTime = source.StartTime;
            target.EventName = source.EventName;
            target.Miles = source.Miles;
            target.Location = source.Location;
            target.IsHillClimb = source.IsHillClimb;
            target.IsClubChampionship = source.IsClubChampionship;
            target.IsNonStandard10 = source.IsNonStandard10;
            target.IsEvening10 = source.IsEvening10;
            target.IsHardRideSeries = source.IsHardRideSeries;
            target.SheetName = source.SheetName;
            target.IsCancelled = source.IsCancelled;
        }


        private CalendarEvent? TryParseCalendarEvent(CsvReader csv)
        {
            var eventId = csv.GetField<int>("EventID");

            var dateRaw = csv.GetField<string>("Date");
            if (!DateTime.TryParse(dateRaw, out var eventDate))
            {
                Console.WriteLine($"[WARN] Skipping EventID {eventId}: invalid date '{dateRaw}'");
                return null;
            }

            var timeRaw = csv.GetField<string>("Start time");
            if (!TimeSpan.TryParse(timeRaw, out var startTime))
            {
                Console.WriteLine($"[WARN] Skipping EventID {eventId}: invalid start time '{timeRaw}'");
                return null;
            }

            var nameRaw = csv.GetField<string>("Event Name");
            if (string.IsNullOrWhiteSpace(nameRaw))
            {
                Console.WriteLine($"[WARN] Skipping EventID {eventId}: missing event name");
                return null;
            }

            return new CalendarEvent
            {
                EventID = eventId,
                EventDate = eventDate,
                StartTime = startTime,
                EventName = nameRaw,
                Miles = csv.GetField<double>("Miles"),
                Location = csv.GetField("Location / Course") ?? string.Empty,
                IsHillClimb = csv.GetField("Hill Climb") == "Y",
                IsClubChampionship = csv.GetField("Club Championship") == "Y",
                IsNonStandard10 = csv.GetField("Non-Standard 10") == "Y",
                IsEvening10 = csv.GetField("Evening 10") == "Y",
                IsHardRideSeries = csv.GetField("Hard Ride Series") == "Y",
                SheetName = csv.GetField("Sheet Name") ?? string.Empty,
                IsCancelled = csv.GetField("isCancelled") == "Y"
            };
        }
    }
}
