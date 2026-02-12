using System.Text;
using ClubCore.Models;

namespace ClubSiteGenerator.Services
{
    public class RoundRobinIndexRenderer
    {
        private readonly IEnumerable<CalendarEvent> calendar;
        private readonly IEnumerable<RoundRobinClub> clubs;
        private readonly string outputDir;
        private readonly string cssFileName;
        private readonly int competitionYear;


        public RoundRobinIndexRenderer(
            IEnumerable<CalendarEvent> calendar,
            IEnumerable<RoundRobinClub> clubs,
            string outputDir,
            string cssFileName)
        {
            this.calendar = calendar;
            this.clubs = clubs;
            this.outputDir = outputDir;
            this.cssFileName = cssFileName;

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
            sb.AppendLine($"  <link rel=\"stylesheet\" href=\"assets/{cssFileName}\">");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("  <div class=\"container\">");

            sb.AppendLine(RenderHeaderGraphic());

            sb.AppendLine($"    <h1>Round Robin TT – {competitionYear}</h1>");
            sb.AppendLine("    <p>A multi‑club time trial series hosted across the region.</p>");

            sb.AppendLine(RenderCalendar());
            sb.AppendLine(RenderCompetitionsSection());
            sb.AppendLine(RenderParticipatingClubsSection(clubs));

            sb.AppendLine(RenderGeneratedFooter());

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
            sb.AppendLine("<div class=\"calendar-grid\">");

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

            sb.AppendLine("  <div class=\"weekday-row\">");
            sb.AppendLine("    <div>Mon</div>");
            sb.AppendLine("    <div>Tue</div>");
            sb.AppendLine("    <div>Wed</div>");
            sb.AppendLine("    <div>Thu</div>");
            sb.AppendLine("    <div>Fri</div>");
            sb.AppendLine("    <div>Sat</div>");
            sb.AppendLine("    <div>Sun</div>");
            sb.AppendLine("  </div>");

            sb.AppendLine("  <div class=\"month-grid\">");

            int daysInMonth = DateTime.DaysInMonth(competitionYear, month);
            int firstDayOfWeek = ((int)new DateTime(competitionYear, month, 1).DayOfWeek + 6) % 7;

            // Leading blanks
            for (int i = 0; i < firstDayOfWeek; i++)
                sb.AppendLine("    <div class=\"cell empty\"><div class=\"cell-content\"></div></div>");

            // Actual days
            for (int day = 1; day <= daysInMonth; day++)
            {
                var ev = events.FirstOrDefault(e => e.EventDate.Day == day);

                if (ev == null)
                {
                    sb.AppendLine(
        $@"    <div class=""cell no-event"">
            <div class=""cell-content"">{day}</div>
        </div>");
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
        // EVENT CELL (DATE + LINK + CLUB SHORTNAME BELOW)
        // ------------------------------------------------------------
        private string RenderEventCell(CalendarEvent ev, int day)
        {
            var link = $"events/{competitionYear}-rr-event-{ev.RoundRobinEventNumber:D2}.html";
            var club = FindClubForEvent(ev);

            var classes = new List<string> { "day-cell", "rr-event" };

            // No background logos anymore
            var classAttr = string.Join(" ", classes);

            // Club shortname (mixed case preserved)
            var clubShort = club?.ShortName ?? "";

            return
        $@"    <div class=""{classAttr}"">
            <div class=""cell-content"">
                <a href=""{link}"" title=""{ev.EventName}"">{day}</a>
                <div class=""club-shortname"">{clubShort}</div>
            </div>
        </div>";
        }

        // ------------------------------------------------------------
        // COMPETITIONS SECTION
        // ------------------------------------------------------------
        private string RenderCompetitionsSection()
        {
            return
        $@"<h2>Competitions</h2>
<ul class=""competitions"">
    <li><a href=""competitions/{competitionYear}-rr-open.html"">Open Competition</a></li>
    <li><a href=""competitions/{competitionYear}-rr-women.html"">Women’s Competition</a></li>
    <li><a href=""competitions/{competitionYear}-rr-club.html"">Club Competition</a></li>
</ul>";
        }

        // ------------------------------------------------------------
        // PARTICIPATING CLUBS
        // ------------------------------------------------------------
        private string RenderParticipatingClubsSection(IEnumerable<RoundRobinClub> clubs)
        {
            var activeClubs = clubs
                .Where(c => c.FromYear <= competitionYear)
                .OrderBy(c => c.FullName)
                .ToList();

            var sb = new StringBuilder();

            sb.AppendLine("<h2>Participating Clubs</h2>");
            sb.AppendLine("<div class=\"clubs-grid\">");

            foreach (var club in activeClubs)
            {
                var logoPath = $"logos/{club.ShortName.ToLower()}.png";

                sb.AppendLine("  <div class=\"club\">");

                // Optional: wrap logo in a link if WebsiteUrl is present
                if (!string.IsNullOrWhiteSpace(club.WebsiteUrl))
                {
                    sb.AppendLine($"    <a href=\"{club.WebsiteUrl}\" target=\"_blank\" rel=\"noopener\">");
                    sb.AppendLine($"      <div class=\"club-logo\"><img src=\"{logoPath}\" alt=\"{club.ShortName}\"></div>");
                    sb.AppendLine("    </a>");
                }
                else
                {
                    sb.AppendLine($"    <div class=\"club-logo\"><img src=\"{logoPath}\" alt=\"{club.ShortName}\"></div>");
                }

                sb.AppendLine($"    <div class=\"club-name\">{club.FullName}</div>");
                sb.AppendLine("  </div>");
            }

            sb.AppendLine("</div>");

            return sb.ToString();
        }

        private string RenderHeaderGraphic()
        {
            return
        $@"<div class=""rr-hero"">
    <img src=""/logos/round-robin/rr-header-2026.png""
         alt=""Round Robin TT Series branding""
         class=""rr-header"" />
</div>";
        }

        private string RenderGeneratedFooter()
        {
            var timestamp = DateTime.UtcNow.ToString("dddd, dd MMMM yyyy HH:mm 'UTC'");
            return $"<footer><p class=\"generated\">Generated {timestamp}</p></footer>";
        }

        private RoundRobinClub? FindClubForEvent(CalendarEvent ev)
        {
            if (string.IsNullOrWhiteSpace(ev.RoundRobinClub))
                return null;

            return clubs.FirstOrDefault(c =>
                string.Equals(c.ShortName, ev.RoundRobinClub, StringComparison.OrdinalIgnoreCase));
        }
    }
}