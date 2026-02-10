using System.Text;
using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public class RoundRobinIndexRenderer
    {
        private readonly IEnumerable<CalendarEvent> calendar;
        private readonly string outputDir;
        private readonly int competitionYear;

        public RoundRobinIndexRenderer(
            IEnumerable<CalendarEvent> calendar,
            string outputDir)
        {
            this.calendar = calendar;
            this.outputDir = outputDir;

            competitionYear = calendar.FirstOrDefault()?.EventDate.Year
                              ?? DateTime.Now.Year;
        }

        // ------------------------------------------------------------
        // PUBLIC ENTRY POINT
        // ------------------------------------------------------------
        public void RenderIndex(string indexFileName)
        {
            Directory.CreateDirectory(outputDir);

            var html = BuildIndexHtml();
            var path = Path.Combine(outputDir, indexFileName);

            File.WriteAllText(path, html);
        }

        // ------------------------------------------------------------
        // MAIN PAGE BUILDER
        // ------------------------------------------------------------
        private string BuildIndexHtml()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("  <meta charset=\"utf-8\">");
            sb.AppendLine("  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
            sb.AppendLine($"  <title>Round Robin TT – {competitionYear}</title>");
            sb.AppendLine("  <link rel=\"stylesheet\" href=\"assets/roundrobin.css\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("  <div class=\"container\">");
            sb.AppendLine($"    <h1>Round Robin TT – {competitionYear}</h1>");
            sb.AppendLine("    <p>A multi‑club time trial series hosted across the region.</p>");

            sb.AppendLine(RenderCalendar());
            sb.AppendLine(RenderCompetitionsSection());
            sb.AppendLine(RenderParticipatingClubsSection());

            sb.AppendLine("  </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        // ------------------------------------------------------------
        // CALENDAR (CSS GRID)
        // ------------------------------------------------------------
        private string RenderCalendar()
        {
            var sb = new StringBuilder();

            sb.AppendLine("<h2>Calendar</h2>");
            sb.AppendLine("<div class=\"calendar-wrapper\">");

            var firstMonth = calendar.Min(e => e.EventDate.Month);
            var lastMonth = calendar.Max(e => e.EventDate.Month);

            for (int month = firstMonth; month <= lastMonth; month++)
            {
                var monthEvents = calendar
                    .Where(e => e.EventDate.Year == competitionYear &&
                                e.EventDate.Month == month)
                    .OrderBy(e => e.EventDate)
                    .ToList();

                if (monthEvents.Any())
                    sb.AppendLine(RenderMonth(month, monthEvents));
            }

            sb.AppendLine("</div>");
            return sb.ToString();
        }

        // ------------------------------------------------------------
        // MONTH BLOCK
        // ------------------------------------------------------------
        private string RenderMonth(int month, List<CalendarEvent> events)
        {
            var sb = new StringBuilder();
            var monthName = new DateTime(competitionYear, month, 1).ToString("MMMM");

            sb.AppendLine("<div class=\"month\">");
            sb.AppendLine($"  <h3>{monthName}</h3>");
            sb.AppendLine("  <div class=\"calendar-grid\">");

            int daysInMonth = DateTime.DaysInMonth(competitionYear, month);
            int firstDayOfWeek = ((int)new DateTime(competitionYear, month, 1).DayOfWeek + 6) % 7;

            // Leading blanks
            for (int i = 0; i < firstDayOfWeek; i++)
                sb.AppendLine("    <div class=\"day empty\"></div>");

            // Actual days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var ev = events.FirstOrDefault(e => e.EventDate.Day == day);

                if (ev == null)
                {
                    sb.AppendLine($"    <div class=\"day no-event\"><span class=\"date\">{day}</span></div>");
                }
                else
                {
                    sb.AppendLine(RenderEventCell(ev, day));
                }
            }

            sb.AppendLine("  </div>");
            sb.AppendLine("</div>");

            return sb.ToString();
        }

        // ------------------------------------------------------------
        // EVENT CELL (LOGO + DATE + LINK)
        // ------------------------------------------------------------
        private string RenderEventCell(CalendarEvent ev, int day)
        {
            var club = ev.RoundRobinClub;
            var logoPath = $"logos/{club.ToLower()}.png"; // adjust if using SVG

            // Link target: event results page (future)
            var link = $"event{ev.RoundRobinEventNumber:D2}.html";

            return
$@"    <div class=""day event"">
          <a href=""{link}"" class=""cell-link"" title=""{ev.EventName}"">
              <span class=""date"">{day}</span>
              <img src=""{logoPath}"" alt=""{club}"" class=""host-logo"" />
          </a>
      </div>";
        }

        // ------------------------------------------------------------
        // COMPETITIONS SECTION
        // ------------------------------------------------------------
        private string RenderCompetitionsSection()
        {
            return
@"<h2>Competitions</h2>
<ul class=""competitions"">
    <li><a href=""competition-open.html"">Open Competition</a></li>
    <li><a href=""competition-women.html"">Women’s Competition</a></li>
    <li><a href=""competition-club.html"">Club Competition</a></li>
</ul>";
        }

        // ------------------------------------------------------------
        // PARTICIPATING CLUBS
        // ------------------------------------------------------------
        private string RenderParticipatingClubsSection()
        {
            // For now, static. Later: data‑driven.
            return
@"<h2>Participating Clubs</h2>
<div class=""club-grid"">
    <div class=""club""><img src=""logos/wvcc.png""><div>WVCC</div></div>
    <div class=""club""><img src=""logos/hcrc.png""><div>HCRC</div></div>
    <div class=""club""><img src=""logos/rfw.png""><div>RFW</div></div>
    <div class=""club""><img src=""logos/ratae.png""><div>Ratae</div></div>
    <div class=""club""><img src=""logos/lfcc.png""><div>LFCC</div></div>
</div>";
        }
    }
}