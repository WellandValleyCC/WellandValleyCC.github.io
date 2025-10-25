using ClubProcessor.Context;
using ClubProcessor.Models;
using ClubProcessor.Models.Csv;
using ClubProcessor.Models.Enums;
using CsvHelper;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ClubProcessor.Services
{
    public class EventsImporter
    {
        private readonly EventDbContext eventContext;
        private readonly CompetitorDbContext competitorContext;

        public EventsImporter(EventDbContext eventContext, CompetitorDbContext competitorContext)
        {
            this.eventContext = eventContext;
            this.competitorContext = competitorContext;
        }

        public void ImportFromFolder(string folderPath)
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

            var incomingRows = csv.GetRecords<RideCsvRow>().ToList();
            var incomingRides = incomingRows
                .Select(row => ParseRide(row, eventNumber))
                .Where(ride => ride != null)
                .ToList();

            var existingRides = eventContext.Rides
                .Where(r => r.EventNumber == eventNumber)
                .ToList();

            var incomingByName = incomingRides
                .GroupBy(r => r.Name)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var group in incomingRides.GroupBy(r => r.Name))
            {
                if (group.Count() > 1)
                {
                    Console.WriteLine($"[WARN] Duplicate ride name '{group.Key}' in Event {eventNumber}. Using first entry.");
                }
            }

            var existingByName = existingRides.ToDictionary(r => r.Name);

            var toAdd = new List<Ride>();
            var toUpdate = new List<Ride>();
            var toDelete = new List<Ride>();

            foreach (var incoming in incomingRides)
            {
                if (existingByName.TryGetValue(incoming.Name, out var existing))
                {
                    if (!RidesAreEqual(existing, incoming))
                    {
                        Console.WriteLine($"[UPDATE] Ride '{incoming.Name}' in Event {eventNumber}");
                        LogRideDiff(existing, incoming);
                        UpdateRide(existing, incoming);
                        toUpdate.Add(existing);
                    }
                }
                else
                {
                    Console.WriteLine($"[ADD] Ride '{incoming.Name}' in Event {eventNumber}");
                    toAdd.Add(incoming);
                }
            }

            foreach (var existing in existingRides)
            {
                if (!incomingByName.ContainsKey(existing.Name))
                {
                    Console.WriteLine($"[DELETE] Ride '{existing.Name}' in Event {eventNumber}");
                    toDelete.Add(existing);
                }
            }

            eventContext.Rides.RemoveRange(toDelete);
            eventContext.Rides.UpdateRange(toUpdate);
            eventContext.Rides.AddRange(toAdd);
        }

        private void UpdateRide(Ride target, Ride source)
        {
            target.ClubNumber = source.ClubNumber;
            target.ActualTime = source.ActualTime;
            target.TotalSeconds = source.TotalSeconds;
            target.IsRoadBike = source.IsRoadBike;
            target.Eligibility = source.Eligibility;
        }

        private bool RidesAreEqual(Ride a, Ride b)
        {
            return a.ClubNumber == b.ClubNumber &&
                   a.ActualTime == b.ActualTime &&
                   a.TotalSeconds == b.TotalSeconds &&
                   a.IsRoadBike == b.IsRoadBike &&
                   a.Eligibility == b.Eligibility;
        }

        private void LogRideDiff(Ride old, Ride updated)
        {
            if (old.ClubNumber != updated.ClubNumber)
                Console.WriteLine($"  - ClubNumber: {old.ClubNumber} → {updated.ClubNumber}");
            if (old.ActualTime != updated.ActualTime)
                Console.WriteLine($"  - ActualTime: {old.ActualTime} → {updated.ActualTime}");
            if (old.TotalSeconds != updated.TotalSeconds)
                Console.WriteLine($"  - TotalSeconds: {old.TotalSeconds} → {updated.TotalSeconds}");
            if (old.IsRoadBike != updated.IsRoadBike)
                Console.WriteLine($"  - IsRoadBike: {old.IsRoadBike} → {updated.IsRoadBike}");
            if (old.Eligibility != updated.Eligibility)
                Console.WriteLine($"  - Eligibility: {old.Eligibility} → {updated.Eligibility}");
        }


        /// <remarks>
        ///   Number/Name,H,M,S,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed
        ///   9999,0.0,24.0,18.0,r,DNF,John Doe,00:24:18,
        ///   Johnny Doe,0.0,27.0,30.0,,,Johnny Doe,00:27:30,X             * 
        /// </remarks>
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
                Name = row.Name,
                ClubNumber = isClubMember ? clubNumber : null,
                ActualTime = row.ActualTime,
                TotalSeconds = row.TotalSeconds,
                IsRoadBike = row.IsRoadBike,
                Eligibility = row.Eligibility,
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

