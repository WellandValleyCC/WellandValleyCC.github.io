# Welland Valley Cycling Club — Time Trial Results Engine

This repository powers the static website for Welland Valley CC's time trial results. It uses GitHub Actions to automate the processing of event results and generate HTML pages for public viewing.

## 🧠 Overview

- **Input**: `.xlsx` or `.csv` files containing individual TT event results
- **Reference**: `club.db` SQLite file with member details and event calendar
- **Processing**: C# apps extract results, calculate league standings, and emit HTML
- **Output**: Static HTML files published via GitHub Pages from the `/docs` folder

## 📁 Folder Structure

```plaintext
Wellandvalley.github.io/
├── .github/
│   └── workflows/
│       └── process-results.yml         # GitHub Action to automate XLSX → HTML
├── processor/
│   ├── TTProcessor.csproj              # C# app to generate HTML standings
│   ├── XlsxToCsvExtractor.csproj       # C# app to extract CSVs from XLSX
│   └── Shared/                         # Optional: shared models, helpers
├── results/
│   ├── 2026-04-12.xlsx                 # Raw input file for event
│   └── 2026-04-19.xlsx
├── data/
│   ├── club.db                         # SQLite: members + calendar
│   ├── standings-2026.json             # Optional: machine-readable output
│   └── logs/                           # Optional: debug logs or audit trail
├── docs/                               # ✅ GitHub Pages root
│   ├── index.htm                       # Main landing page
│   ├── events/
│   │   ├── 2026-04-12.html             # TT event result
│   │   └── 2026-04-19.html
│   ├── standings/
│   │   ├── 2026-veterans.html
│   │   ├── 2026-women.html
│   │   ├── 2026-roadbike.html
│   │   └── 2025-veterans.html          # Archived standings
│   ├── riders/
│   │   └── mike-smith.html             # Optional: rider profile
│   └── assets/
│       ├── styles.css
│       └── script.js
├── README.md                           # Project overview
└── .gitignore                          # Ignore /data or build artifacts

```

## 🧪 Local Development

To test locally:
- Run `XlsxToCsvExtractor` on a `.xlsx` file in `/results/`
- Run `TTProcessor` with the extracted CSVs and `club.db`
- Output HTML files to `/docs/` and open in browser

## 📬 Contributions

This project is maintained by club volunteers. Feel free to submit issues or pull requests to improve automation, layout, or accessibility.


