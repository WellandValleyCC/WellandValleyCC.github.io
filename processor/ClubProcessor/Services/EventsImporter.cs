using ClubCore.Context;
using ClubCore.Models;
using ClubCore.Models.Csv;
using ClubCore.Models.Enums;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
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
            var yearMatch = Regex.Match(folderName, @"\b(20\d{2})\b"); // Matches 2000–2099

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

            Console.WriteLine($"[INFO] Processing year: {year}");
            Console.WriteLine($"[INFO] Processing events in: {eventsDir}");

            ClearRidesForObsoleteEvents();

            var csvFiles = Directory.GetFiles(eventsDir, "Event_*.csv");
            foreach (var csvPath in csvFiles.OrderBy(f => f))
            {
                var eventNumber = ExtractEventNumber(csvPath);
                Console.WriteLine($"[INFO] Processing Event {eventNumber}: {csvPath}");
                ProcessEventCsv(csvPath, eventNumber, year);
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

        private int ExtractEventNumber(string path)
        {
            var match = Regex.Match(Path.GetFileName(path), @"Event_(\d+)\.csv");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private void ProcessEventCsv(string csvPath, int eventNumber, int competitionYear)
        {
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            if (!calendar.Any(ev => ev.EventNumber == eventNumber))
            {
                Console.WriteLine($"[WARN] skipping Event {eventNumber} - no valid entry in Calendar sheet.");
                return;
            }

            var eventMiles = calendar
                .Single(ev => ev.EventNumber == eventNumber).Miles;

            var incomingRows = csv.GetRecords<RideCsvRow>().ToList();
            var incomingRides = incomingRows
                .Select(row => ParseRide(row, eventNumber, competitionYear))
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

            foreach (var row in incomingRows.Where(r => string.IsNullOrWhiteSpace(r.Name)))
            {
                Console.WriteLine($"[WARN] Row with missing Name: NumberOrName={row.NumberOrName}, Time={row.ActualTime}");
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

        /// <remarks>
        ///   Number/Name,H,M,S,Roadbike?,DNS/DNF/DQ,Name,Actual Time,Guest or Not Renewed,Number/Raw Name
        ///   9999,0.0,24.0,18.0,r,DNF,John Doe,00:24:18,,9999
        ///   Johnny Doe,0.0,27.0,30.0,,,Johnny Doe,00:27:30,X,Johnny Doe
        ///   Jane Doe (HCRC),0,21.0,57.0,,,Joe Murray (HCRC),00:21:57,X,Jane Doe
        /// </remarks>
        private Ride? ParseRide(RideCsvRow row, int eventNumber, int competitionYear)
        {
            var rawNumberOrName = row.NumberOrName?.Trim();

            bool isClubMember = int.TryParse(rawNumberOrName, out int clubNumber);
            
            var clubShortName = ResolveClubShortName(row, isClubMember, competitionYear);

            return new Ride
            {
                EventNumber = eventNumber,
                Name = row.Name,
                ClubNumber = isClubMember ? clubNumber : null,
                RoundRobinClub = clubShortName,
                TotalSeconds = row.TotalSeconds,
                AvgSpeed = row.TotalSeconds > 0 ? 25.0 * 1000 / row.TotalSeconds * 3.6 : null, // Example: 25 km course
                IsRoadBike = row.IsRoadBike,
                Status = row.Eligibility,
            };
        }

        private string? ResolveClubShortName(RideCsvRow row, bool isClubMember, int competitionYear)
        {
            if (isClubMember)
                return "WVCC"; // default for members

            // Otherwise try to extract a club name from the rider's name
            var clubName = ExtractClubNameFromName(row.Name);
            if (clubName is null)
                return null;

            return LookupClubShortName(clubName, competitionYear);
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
    }
}

