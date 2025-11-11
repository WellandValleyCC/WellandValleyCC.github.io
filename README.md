# Welland Valley Cycling Club — Time Trial Results Engine

This repository powers the static website for Welland Valley CC's time trial results. Source data, processors and templates live on the **master** branch; generated HTML is published from the **gh-pages** branch and served by GitHub Pages (so generated output does not clutter the source branch.)

---

## Overview

- **Input**: `data/competitors_2026.csv` or `data/ClubEvents 2026.xlsx` membership lists and event result files 
- **Reference**: SQLite databases (`club_competitors_YYYY.db`, `club_events_YYYY.db`)
- **Processing**: C# apps and helper scripts compute standings and generate HTML
- **Publish**: CI workflow pushes generated site to `gh-pages` as atomic commits where it is served by GitHub Pages

---

## Repository Structure

```plaintext
WellandValleyCC.github.io/
├── .github/
│   └── workflows/
│       └── process-data.yml            # CI pipeline: build, test, run processor
├── processor/
│   ├── ClubProcessor.sln              # Solution file for modular apps
│   ├── ClubProcessor/                 # C# app: process competitors or events
│   ├── XlsxToCsvExtractor/            # C# app: extract CSVs from XLSX
│   ├── Shared/                        # Shared models, helpers, extensions
│   ├── ClubProcessor.Tests/          # xUnit test project with in-memory DB
│   └── internal/
│       └── ci-simulation.md          # Contributor guide for local CI testing
├── data/
│   ├── competitors_2025.csv          # Sample input for processor
│   ├── club.db                       # SQLite: members + calendar
│   ├── standings-2026.json           # Optional: machine-readable output
│   └── logs/                         # Optional: debug logs or audit trail
├── results/
│   ├── Club Events 2026.xlsx         # Results spreadsheet modified on PC and uploaded after each TT
├── docs/                             # GitHub Pages root
│   ├── index.htm                     # Main landing page
│   ├── events/
│   │   ├── 2026-04-12.html           # TT event result
│   │   └── 2026-04-19.html
│   ├── standings/
│   │   ├── 2026-veterans.html
│   │   ├── 2026-women.html
│   │   ├── 2026-roadbike.html
│   │   └── 2025-veterans.html        # Archived standings
│   ├── riders/
│   │   └── mike-smith.html           # Optional: rider profile
│   └── assets/
│       ├── styles.css
│       └── script.js
├── README.md                         # Project overview
└── .gitignore                        # Ignore /data or build artifacts
```

- master branch: editable source (data, scripts, templates). Do not keep generated HTML here.- 

- gh-pages branch: single-purpose branch containing only the generated static site that GitHub Pages serves.

---


## How publishing works

1. Competitor data is maintained in the private repo `WellandValleyCC.club-membership-private` (see its README).  
2. A workflow there commits `competitors_YYYY.csv` into this repo (`master/data/`).  
3. A workflow on `master` processes that file (`--mode competitions`) and updates `club_competitors_YYYY.db`.  
4. After each TT event, editors commit `ClubEvents_YYYY.xlsx` into `master/data/`.  
5. A workflow on `master` processes the event file (`--mode events`), generates HTML into `site-out/`, and publishes it to `gh-pages` as an atomic commit.  
6. GitHub Pages serves the site directly from the `gh-pages` branch.  

Each publish is atomic and traceable to its source commit.


---

## Local Development

- Run `XlsxToCsvExtractor` on `.xlsx` files in `results/`
- Run `ClubProcessor` with CSVs and `club.db`
- Output HTML into `site-out/` and open `site-out/index.html`

See [Developer Docs](developer-docs/) for detailed guides:
- [CI Workflows](developer-docs/ci-workflows.md)
- [Event Processor](developer-docs/event-processor.md)
- [League Competitions](developer-docs/leagues.md)
- [Local CI Simulation Guide](processor/internal/ci-simulation.md)

---

## Tools
### VTTA Scraper
[VTTA Scraper](tools/vtta-scraper/README.md)

Additional utilities live under `tools/`

--- 

## League Competitions
League allocations (Prem, 1–4) are defined mid‑season in `Leagues` worksheet with ClubEvents_2026.xlsx
  
See [League Competitions Guide](developer-docs/leagues.md) for details on membership persistence and scoring outputs.

---

## Contributions

This project is maintained by club volunteers. Feel free to submit issues or pull requests to improve automation, layout, or accessibility.

### Notes for contributors

- Do not commit generated HTML into `master`. Keep templates, scripts and source data on `master`.

- Fix source data on `master` rather than editing `gh-pages` directly

- Keep contributor guides updated when processor arguments or publishing steps change
