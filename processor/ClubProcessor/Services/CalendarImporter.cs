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
            var records = ParseCalendarEvents(reader);

            _db.CalendarEvents.ExecuteDelete();
            _db.CalendarEvents.AddRange(records);
            _db.SaveChanges();
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
