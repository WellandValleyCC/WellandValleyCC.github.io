using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Csv;
using ClubProcessor.Models.Enums;
using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubProcessor.Services
{
    public class EventProcessor
    {
        private readonly EventDbContext eventContext;
        private readonly CompetitorDbContext competitorContext;

        public EventProcessor(EventDbContext eventContext, CompetitorDbContext competitorContext)
        {
            this.eventContext = eventContext;
            this.competitorContext = competitorContext;
        }

        public void ProcessFolder(string folderPath)
        {
            var folderName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar));
            var yearMatch = Regex.Match(folderName, @"\b(20\d{2})\b"); // Matches 2000–2099

            if (!yearMatch.Success)
            {
                Console.WriteLine($"[ERROR] Could not extract year from folder name: {folderName}");
                return;
            }

            var year = yearMatch.Groups[1].Value;
            var calendarPath = Path.Combine(folderPath, $"calendar_{year}.csv");
            var eventsDir = Path.Combine(folderPath, "events");

            if (!File.Exists(calendarPath))
            {
                Console.WriteLine($"[ERROR] Missing calendar file: {calendarPath}");
                return;
            }

            if (!Directory.Exists(eventsDir))
            {
                Console.WriteLine($"[ERROR] Events folder not found: {eventsDir}");
                return;
            }

            Console.WriteLine($"[INFO] Processing year: {year}");
            Console.WriteLine($"[INFO] Loading calendar: {calendarPath}");
            Console.WriteLine($"[INFO] Processing events in: {eventsDir}");

            var csvFiles = Directory.GetFiles(eventsDir, "Event_*.csv");
            foreach (var csvPath in csvFiles.OrderBy(f => f))
            {
                var eventNumber = ExtractEventNumber(csvPath);
                Console.WriteLine($"[INFO] Processing Event {eventNumber}: {csvPath}");
                ProcessEventCsv(csvPath, eventNumber);
            }

            eventContext.SaveChanges();
        }

        private int ExtractEventNumber(string path)
        {
            var match = Regex.Match(Path.GetFileName(path), @"Event_(\d+)\.csv");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private void ProcessEventCsv(string csvPath, int eventNumber)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            var rows = csv.GetRecords<RideCsvRow>().ToList();
            foreach (var row in rows)
            {
                var ride = ParseRide(row, eventNumber);
                if (ride != null)
                {
                    eventContext.Rides.Add(ride);
                }
            }
        }

        private Ride? ParseRide(RideCsvRow row, int eventNumber)
        {
            // TODO: Parse columns from dynamic row
            // TODO: Compute calculated fields (points, standard time, etc.)
            // TODO: Handle DNS/DNF/DQ and set RideEligibility

            var rawNumberOrName = row.NumberOrName?.Trim();

            bool isClubMember = int.TryParse(rawNumberOrName, out int clubNumber);
            string? name = isClubMember ? null : rawNumberOrName;

            return new Ride
            {
                EventNumber = eventNumber,
                Name = name,
                ClubNumber = isClubMember ? clubNumber : null,
                ActualTime = row.ActualTime,
                TotalSeconds = row.TotalSeconds,
                IsRoadBike = row.IsRoadBike,
                Eligibility = row.Eligibility,

                // Add more fields as needed
            };
        }

        private double ParseSeconds(string time)
        {
            // TODO: Support mm:ss.ss and hh:mm:ss formats
            return TimeSpan.TryParse(time, out var ts) ? ts.TotalSeconds : 0;
        }

        private RideEligibility ParseEligibility(string status)
        {
            return status switch
            {
                "DNS" => RideEligibility.DNS,
                "DQ" => RideEligibility.DQ,
                "DNF" => RideEligibility.DNF,
                _ => RideEligibility.Valid
            };
        }
    }
}

