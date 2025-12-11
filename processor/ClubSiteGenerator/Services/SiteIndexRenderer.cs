using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Models.Enums;
using ClubSiteGenerator.ResultsGenerator;
using System.Text;

namespace ClubSiteGenerator.Services
{
    public class SiteIndexRenderer
    {
        private readonly IEnumerable<EventResultsSet> events;
        private readonly IEnumerable<CompetitionResultsSet> competitions;
        private readonly string outputDir;

        public SiteIndexRenderer(IEnumerable<EventResultsSet> events,
                                 IEnumerable<CompetitionResultsSet> competitions,
                                 string outputDir)
        {
            this.events = events;
            this.competitions = competitions;
            this.outputDir = outputDir;
        }

        public void RenderIndex()
        {
            Directory.CreateDirectory(outputDir);

            // Order events by EventNumber
            var orderedEvents = events.OrderBy(ev => ev.EventNumber).ToList();

            // Order competitions by fixed sequence
            var orderedCompetitions = competitions
                .OrderBy(c => Array.IndexOf(CompetitionOrder, c.CompetitionType))
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <title>Season Index</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"assets/styles.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Season Overview</h1>");

            // Legacy link
            sb.AppendLine("<h2>Past Seasons</h2>");
            sb.AppendLine("<ul>");
            sb.AppendLine("  <li><a href=\"https://wellandvalleycc.github.io/legacy/index.htm\">Legacy Results Archive</a></li>");
            sb.AppendLine("</ul>");

            // Events section
            sb.AppendLine("<h2>2026 Calendar</h2>");
            sb.AppendLine("<div class=\"calendar-grid\">");

            foreach (int month in new[] { 3, 4, 5, 6, 7, 8 }) // Mar–Aug
            {
                var monthEvents = orderedEvents.Where(e => e.EventDate.Year == 2026 && e.EventDate.Month == month);
                sb.AppendLine(RenderMonthCalendar(2026, month, monthEvents));
            }

            sb.AppendLine("</div>");

            // Competitions grouped
            sb.AppendLine("<h2>Championship Competitions</h2>");
            sb.AppendLine("<ul>");
            foreach (var comp in orderedCompetitions.Where(c =>
                c.CompetitionType == CompetitionType.Seniors ||
                c.CompetitionType == CompetitionType.Veterans ||
                c.CompetitionType == CompetitionType.Women ||
                c.CompetitionType == CompetitionType.Juniors ||
                c.CompetitionType == CompetitionType.Juveniles ||
                c.CompetitionType == CompetitionType.RoadBikeMen ||
                c.CompetitionType == CompetitionType.RoadBikeWomen))
            {
                sb.AppendLine($"  <li><a href=\"{comp.SubFolderName}/{comp.FileName}.html\">{comp.DisplayName}</a></li>");
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("<h2>Leagues</h2>");
            sb.AppendLine("<ul>");
            foreach (var comp in orderedCompetitions.Where(c =>
                c.CompetitionType == CompetitionType.PremierLeague ||
                c.CompetitionType == CompetitionType.League1 ||
                c.CompetitionType == CompetitionType.League2 ||
                c.CompetitionType == CompetitionType.League3 ||
                c.CompetitionType == CompetitionType.League4))
            {
                sb.AppendLine($"  <li><a href=\"{comp.SubFolderName}/{comp.FileName}.html\">{comp.DisplayName}</a></li>");
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("<h2>Nev Brooks</h2>");
            sb.AppendLine("<ul>");
            foreach (var comp in orderedCompetitions.Where(c => c.CompetitionType == CompetitionType.NevBrooks))
            {
                sb.AppendLine($"  <li><a href=\"{comp.SubFolderName}/{comp.FileName}.html\">{comp.DisplayName}</a></li>");
            }
            sb.AppendLine("</ul>");

            sb.AppendLine("</body></html>");

            var path = Path.Combine(outputDir, "preview.html");
            File.WriteAllText(path, sb.ToString());
        }

        private string RenderMonthCalendar(int year, int month, IEnumerable<EventResultsSet> events)
        {
            var sb = new StringBuilder();
            var monthName = new DateTime(year, month, 1).ToString("MMMM");
            sb.AppendLine("<div class=\"month\">");
            sb.AppendLine($"  <h3>{monthName}</h3>");
            sb.AppendLine("  <table class=\"calendar\">");

            // Header row
            sb.AppendLine("    <tr>");
            foreach (var dayName in new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" })
                sb.AppendLine($"      <th>{dayName}</th>");
            sb.AppendLine("    </tr>");

            int daysInMonth = DateTime.DaysInMonth(year, month);
            int firstDayOfWeek = ((int)new DateTime(year, month, 1).DayOfWeek + 6) % 7;

            sb.AppendLine("    <tr>");

            // Empty cells before the first day
            for (int i = 0; i < firstDayOfWeek; i++)
                sb.AppendLine("      <td></td>");

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(year, month, day);
                var ev = events.FirstOrDefault(e => e.EventDate == date);

                if (ev != null)
                {
                    sb.AppendLine($"      <td><a href=\"{ev.SubFolderName}/{ev.FileName}.html\">{day}</a></td>");
                }
                else
                {
                    sb.AppendLine($"      <td class=\"no-event\">{day}</td>");
                }

                // End of week → close row and start new one
                if ((day + firstDayOfWeek) % 7 == 0 && day != daysInMonth)
                {
                    sb.AppendLine("    </tr>");
                    sb.AppendLine("    <tr>");
                }
            }

            // Fill trailing empty cells
            int lastDayOfWeek = (firstDayOfWeek + daysInMonth) % 7;
            if (lastDayOfWeek != 0)
            {
                for (int i = lastDayOfWeek; i < 7; i++)
                    sb.AppendLine("      <td></td>");
                sb.AppendLine("    </tr>");
            }
            else
            {
                sb.AppendLine("    </tr>");
            }

            sb.AppendLine("  </table>");
            sb.AppendLine("</div>");
            return sb.ToString();
        }

        public static readonly CompetitionType[] CompetitionOrder =
        {
            CompetitionType.Seniors, CompetitionType.Veterans, CompetitionType.Women, CompetitionType.Juniors, CompetitionType.Juveniles, 
            CompetitionType.RoadBikeMen, CompetitionType.RoadBikeWomen,
            CompetitionType.PremierLeague, CompetitionType.League1, CompetitionType.League2, CompetitionType.League3, CompetitionType.League4,
            CompetitionType.NevBrooks
        };
    }

}
