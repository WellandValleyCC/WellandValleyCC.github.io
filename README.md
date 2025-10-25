# Welland Valley Cycling Club — Time Trial Results Engine

This repository powers the static website for Welland Valley CC's time trial results. Source data, processors and templates live on the **master** branch; generated HTML is published from the **gh-pages** branch so generated output does not clutter the source branch.

---

## Overview

- **Input**: `data/competitors_2026.csv` or `data/ClubEvents 2026.xlsx` membership lists and event result files 
- **Reference**: `club_2026.db` SQLite file with member records and calendar data  
- **Processing**: C# apps and helper scripts extract data, compute standings, and emit HTML pages  
- **Publish**: Generated site is pushed to the `gh-pages` branch and served by GitHub Pages

---

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
├── docs/                             # ✅ GitHub Pages root
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

## How publishing works

An editor pushes CSV/XLSX to master under data/ or triggers processing manually.

A workflow on master runs the processor, writes generated HTML to site-out/, then commits and pushes the generated site to gh-pages as an atomic commit.

GitHub Pages serves the gh-pages branch. Each publish is atomic and corresponds exactly to the commit pushed to gh-pages.

## Developer guide

### Local build

- Run `XlsxToCsvExtractor` on raw `.xlsx` files in `results/` to produce CSVs.

- Run the processor with the CSVs and `club.db`.

- Direct generated HTML into `site-out/` and open `site-out/index.html` to verify locally.

### CI workflow overview

- Checkout `master` with full history (`fetch-depth: 0`).

- Install runtime tools and dependencies (dotnet, python packages, etc.).

- Run processor to produce `site-out/`.

- Publish `site-out/` to `gh-pages` branch in one atomic commit.

- Use workflow `concurrency` to avoid overlapping generator runs.

### Prevent loops
- Trigger processor workflows only on input/source paths, for example:

``` yaml
on:
  push:
    branches: [ master ]
    paths:
      - 'data/**'
      - 'results/**'
      - 'scripts/**'
```

- Ensure processor workflows do not trigger on `gh-pages/**`.

- Mark generated commits with a tag in the message, for example:

``` text
chore: publish site from master <SHA> [generated]
```

### Example CI commit and push snippet

``` Bash
git config user.name "github-actions[bot]"
git config user.email "41898282+github-actions[bot]@users.noreply.github.com"

# prepare or update gh-pages worktree
git worktree add /tmp/gh-pages gh-pages || (git checkout --orphan gh-pages && git rm -rf .)

# sync generated site into worktree and commit
rsync -a --delete site-out/ /tmp/gh-pages/
cd /tmp/gh-pages

git add -A
if git diff --quiet --exit-code; then
  echo "No changes to publish"
  exit 0
fi

git commit -m "chore: publish site from master ${GITHUB_SHA} [generated]"
git push origin HEAD:gh-pages
```

Note: the snippet above uses only `GITHUB_TOKEN` (no PAT required) when run inside GitHub Actions with `permissions: contents: write`.

### Example `generate-site.yml` workflow

``` yaml
name: Generate site to gh-pages

on:
  push:
    branches: [ master ]
    paths:
      - 'data/**'
      - 'results/**'
      - 'scripts/**'
      - '.github/workflows/generate-site.yml'

permissions:
  contents: write

jobs:
  build-and-publish:
    runs-on: ubuntu-latest
    concurrency:
      group: generate-site-${{ github.ref }}
      cancel-in-progress: true
    steps:
      - name: Checkout
        uses: actions/checkout@v4
        with:
          fetch-depth: 0

      - name: Install runtime and deps
        run: |
          # example for dotnet and python helpers
          sudo apt-get update && sudo apt-get install -y python3 python3-pip
          dotnet --info
          python3 -m pip install --upgrade pip
          pip install pandas openpyxl

      - name: Build and run processor
        run: |
          mkdir -p site-out
          # adjust command to your processor invocation
          dotnet build processor/ClubProcessor.sln -c Release
          dotnet run --project processor/ClubProcessor -- build --input data --db club.db --output site-out

      - name: Publish to gh-pages
        env:
          GH_PAGES_BRANCH: gh-pages
        run: |
          git config user.name "github-actions[bot]"
          git config user.email "41898282+github-actions[bot]@users.noreply.github.com"
          git worktree add /tmp/gh-pages gh-pages || (git checkout --orphan gh-pages && git rm -rf .)
          rsync -a --delete site-out/ /tmp/gh-pages/
          cd /tmp/gh-pages
          git add -A
          if git diff --quiet --exit-code; then
            echo "No changes to publish"
            exit 0
          fi
          git commit -m "chore: publish site from master ${GITHUB_SHA} [generated]"
          git push origin HEAD:gh-pages
```

### 🧪 Local Development

To test locally:
- Run `XlsxToCsvExtractor` on a `.xlsx` file in `/results/`
- Run `TTProcessor` with the extracted CSVs and `club.db`
- Output HTML files to `/docs/` and open in browser

[Local CI Simulation Guide](processor/internal/ci-simulation.md)



## 📬 Contributions

This project is maintained by club volunteers. Feel free to submit issues or pull requests to improve automation, layout, or accessibility.

### Notes for contributors

- Do not commit generated HTML into `master`. Keep templates, scripts and source data on `master`.

- If you want a snapshot before removing `docs/` from `master`, create a tag:

``` Bash
git tag -a pre-ghpages-site -m "Snapshot before moving site to gh-pages"
git push origin pre-ghpages-site
```

- For emergency edits to the published site, pushing directly to `gh-pages` is possible but prefer fixing source on `master` so the generator remains authoritative.

 
- Keep `README.md` and contribution notes updated when you change processor arguments, template locations, or publishing steps.

# Developer notes
   
   
## 🧪 Running the Event Processor

### 🐍 Step 1: Extract Event CSVs from XLSX

To split `ClubEvents_2026.xlsx` into per-sheet CSVs:

```bash
python scripts/extract_club_events.py data/ClubEvents_2026.xlsx data/extracted/events/
```

This will produce:
- `calendar_2026.csv` — event metadata
- `competitors_2026.csv` — reference sheet
- `Event_01.csv` to `Event_26.csv` — per-event results

---

### 🧠 Step 2: Run the Processor via CLI

To ingest the extracted CSVs and populate the event database:

```bash
dotnet run --project processor/ClubProcessor/ClubProcessor.csproj -- --mode events --folder data/extracted/events/
```

This will:
- Parse each `Event_*.csv`
- Normalize and validate data
- Write to `club_events_2026.db` (or configured target)
- Emit diagnostics and metrics to console

---

### 🧪 Step 3: Debug in Visual Studio

To run the processor in debug mode:

1. **Set the Startup Project**
   - In Solution Explorer, right-click `ClubProcessor.csproj`
   - Select **Set as Startup Project**

2. **Configure Debug Arguments**
   - Right-click `ClubProcessor` → **Properties**
   - Go to the **Debug** tab
   - Set **Application arguments** to:
     ```
     --mode events --folder ../../data/extracted/events/
     ```
   - Adjust the path if your `.csproj` is located elsewhere

3. **Set Breakpoints**
   - Open the method that handles `--mode events`
   - Add breakpoints in:
     - CSV parsing logic
     - Event object creation
     - SQLite write operations

4. **Run the Processor**
   - Press **F5** to launch in debug mode
   - Inspect how each CSV is processed and written to the event database

## Notes on ClubEvents_2026.XLSX

### Conditional formatting on Event_nn sheets

Configured using this macro.  Not saved as part of the sheet as it's not .xlsm

```
Sub ApplyStandardEventFormattingWithAbsoluteRefs()
    Dim ws As Worksheet
    Dim i As Integer
    Dim fmt As FormatCondition

    For i = 1 To 26
        On Error Resume Next
        Set ws = ThisWorkbook.Sheets("Event_" & Format(i, "00"))
        On Error GoTo 0

        If Not ws Is Nothing Then
            ' Clear all existing conditional formatting
            ws.Cells.FormatConditions.Delete

            ' Rule 1: Format $D$2:$D$101 with number format (2 decimal places)
            With ws.Range("$D$2:$D$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=TEXT($B$104,""@"")=""Y""")
                fmt.NumberFormat = "0.00"
                fmt.StopIfTrue = False
            End With
            
            ' Rule 2: Format $H$2:$H$101 with number format (2 decimal places)
            With ws.Range("$H$2:$H$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=TEXT($B$104,""@"")=""Y""")
                fmt.NumberFormat = "hh:mm:ss.00;@"
                fmt.StopIfTrue = False
            End With

            ' Rule 3: Fill $G$2:$H$101 with cyan when column I = "X"
            With ws.Range("$G$2:$H$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=$I2=""X""")
                fmt.Interior.Color = RGB(179, 235, 255)
                fmt.StopIfTrue = False
            End With

        End If
    Next i

    MsgBox "Formatting applied with absolute references to Event_01 through Event_26.", vbInformation
End Sub
```