using System.Text;

namespace ClubSiteGenerator.Utilities
{
    public static class StylesWriter
    {
        public static void EnsureStylesheet(string outputDir)
        {
            var assetsDir = Path.Combine(outputDir, "assets");
            Directory.CreateDirectory(assetsDir);

            var cssPath = Path.Combine(assetsDir, "styles.css");
            File.WriteAllText(cssPath, CssContent());
        }

        private static string CssContent()
        {
            var builder = new StringBuilder();

            /* ===========================
               Baseline + Body
               =========================== */
            builder.AppendLine(@"
html, body {
  font-family: system-ui, sans-serif;
  font-size: 1rem;   /* baseline desktop size */
  line-height: 1.4;
  margin: 0;
  padding: 0;
}
");

            /* ===========================
               Results Table Styling
               =========================== */
            builder.AppendLine(@"
table.results {
  width: 100%;
  border-collapse: collapse;
  margin: 1rem 0;
  font-size: 1.4rem;
}
table.results th,
table.results td {
  border: 1px solid #ddd;
  padding: 0.4em 0.6em;
  text-align: center;
  line-height: 1.3;
}
table.results th {
  background-color: #f4f4f4;
  font-weight: bold;
}
table.results thead th.invisible-cell {
  background-color: transparent;
  color: transparent;
  border: none;
  padding: 0;
  margin: 0;
  font-weight: normal;
  line-height: 0;
  height: 0;
}
table.results td:first-child {
  text-align: left;
  font-weight: 500;
}
table.results tbody tr:hover { background-color: #f1f7ff; }
");

            /* ===========================
               Results & Legend Colour Coding
               =========================== */
            builder.AppendLine(@"
/* Eligibility (row-level) */
table.results tr.competition-eligible,
.legend .competition-eligible   { background-color: #e0ffe0; }

table.results tr.guest-second-claim,
.legend .guest-second-claim     { background-color: #e0f0ff; }

table.results tr.guest-non-club-member,
.legend .guest-non-club-member  { background-color: #fff0e0; }

/* Ten vs Non-Ten events (cell-level) */
table.results td.ten-mile-event,
.legend .ten-mile-event         { background-color: #FDE9D9; }

table.results td.non-ten-mile-event,
.legend .non-ten-mile-event     { background-color: #DCE6F1; }

/* Scoring columns (cell-level) */
table.results td.scoring-11,
.legend .scoring-11             { background-color: #CCFFCC; }

table.results td.best-8,
.legend .best-8                 { background-color: #EBF1DE; }
");

            /* ===========================
            Column Widths
            =========================== */
            builder.AppendLine(@"
/* Explicit column widths */
table.results th.best-8,
table.results td.best-8 {
  min-width: 70px;   /* wider than Scoring 11 */
}

table.results th.scoring-11,
table.results td.scoring-11 {
  min-width: 65px;   /* baseline width for Scoring 11 */
}
");

            /* ===========================
               Legend Styling
               =========================== */
            builder.AppendLine(@"
.legend {
  font-size: 1rem;
}
.legend span {
  display: inline-block;
  padding: 0.25em 0.5em;
  margin-right: 0.5em;
  border-radius: 4px;
  font-size: 1rem;
}
");

            /* ===========================
               Podium Highlighting
               =========================== */
            builder.AppendLine(@"

/* Gold */
table.results td.scoring-11.gold,
table.results td.best-8.gold,
table.results td.gold,
.legend .gold {
  background-color: #cfb53b;
}

/* Silver */
table.results td.scoring-11.silver,
table.results td.best-8.silver,
table.results td.silver,
.legend .silver {
  background-color: #c4c4c4;
}

/* Bronze */
table.results td.scoring-11.bronze,
table.results td.best-8.bronze,
table.results td.bronze,
.legend .bronze {
  background-color: #a48347;
  color: #DDD9C4;
}

");

            /* ===========================
               Header + Navigation
               =========================== */
            builder.AppendLine(@"
header {
  font-size: 1.25rem;
}
.event-number { 
  color: #666; 
  font-weight: normal; 
  font-size: 1.5rem; 
  margin-right: 0.25em; 
}
header .event-date { 
  font-style: italic; 
  font-size: 1.25rem; 
  color: #555; 
  margin: 0.25em 0; 
}
header .event-distance { 
  font-size: 1.25rem; 
  font-weight: 500; 
  color: #333; 
  margin: 0.25em 0; 
}

/* Explicit header row controls */
table.results thead .event-number {
  font-size: 0.9rem;
  font-weight: 600;
  color: #444;
  padding: 1px 6px;
  line-height: 1.1;
}
table.results thead .event-date {
  font-size: 0.9rem;
  font-weight: 400;
  font-style: italic;
  color: #555;
  white-space: nowrap; 
  padding: 2px 6px;
  line-height: 1.2;
}
table.results thead .event-title {
  font-size: 1rem;
  font-weight: 500;
  color: #222;
  text-align: center;
  padding: 2px 6px;
  line-height: 1.2;
}

/* Header row spacing and font control */
table.results thead tr:nth-child(1) th,
table.results thead tr:nth-child(2) th {
  padding-top: 4px;
  padding-bottom: 4px;
  font-size: 0.9rem;
  vertical-align: middle;
}

table.results thead tr:nth-child(3) th {
  font-size: 0.8rem;
  padding-top: 2px;
  padding-bottom: 2px;
  font-weight: normal;
  color: #555;
  background-color: #f8f8f8;
  border-top: 1px solid #ccc;
}

/* Shared navigation styling for events and competitions */
.event-nav,
.competition-nav {
  display: flex;
  justify-content: space-between;
  margin: 0.5em 0 1em 0;
  font-size: 1rem;
}

.event-nav a,
.competition-nav a {
  flex: 0 0 auto;
  text-align: center;
  padding: 0.4em 0.8em;
  border-radius: 0.25rem;
  background-color: #f8f8f8;
  color: #004080;
  font-weight: 500;
  text-decoration: none;
  transition: background-color 0.2s ease;
  font-size: 1rem;
}

.event-nav a.prev,
.competition-nav a.prev { text-align: left; }

.event-nav a.index,
.competition-nav a.index { text-align: center; }

.event-nav a.next,
.competition-nav a.next { text-align: right; }

.event-nav a.prev::before,
.competition-nav a.prev::before { content: ""←""; }

.event-nav a.next::after,
.competition-nav a.next::after { content: ""→""; }

.event-nav a:hover,
.competition-nav a:hover { background-color: #e0e0e0; }
");

            /* ===========================
            Competition Rules Block
            =========================== */
            builder.AppendLine(@"
.competition-rules {
  background: #f9f9f9;
  border-left: 4px solid #ccc;
  padding: 0.15em 2em;
  margin: 0.5em 3em;
  font-style: italic;
  font-size: 0.95rem;
  color: #333;
");

            /* ===========================
               Footer
               =========================== */
            builder.AppendLine(@"
footer {
  margin-top: 2rem;
  font-size: 0.75rem;
  color: #666;
  text-align: center;
  border-top: 1px solid #ddd;
  padding-top: 0.5em;
}
");

            /* ===========================
               Mobile Overrides
               =========================== */
            builder.AppendLine(@"
@media (max-width: 600px) {
  html, body { font-size: 1rem; }

  table.results { font-size: 0.9rem; }
  table.results th, table.results td { padding: 0.25em 0.5em; line-height: 1.2; }

  .legend { font-size: 0.9rem; justify-content: space-around; gap: 0.4em; }
  .legend span { font-size: 0.8rem; padding: 0.25em 0.4em; margin-right: 0; }

  header { font-size: 0.9rem; }
  .event-number { font-size: 1.2rem; }
  header .event-date { font-size: 0.9rem; }
  header .event-distance { font-size: 0.9rem; }

  /* Mobile header row overrides */
  table.results thead .event-number { font-size: 1rem; font-weight: 500; }
  table.results thead .event-date   { font-size: 0.85rem; font-weight: 400; }
  table.results thead .event-title  { font-size: 0.85rem; font-weight: 500; }

  .event-nav {
    font-size: 0.9rem;
    display: flex;
    justify-content: space-between;
    gap: 0.5em;
  }

  .event-nav a {
    flex: 0 0 auto;
    font-size: 0.9rem;
    padding: 0.6em 1em;
  }

  footer { font-size: 0.6rem; }
}
");

            return builder.ToString();
        }
    }
}
