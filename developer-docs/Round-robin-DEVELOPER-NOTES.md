# Round Robin Developer Notes

This project uses **two GitHub repositories** that work together to generate and
publish the Round Robin Time Trial Series website.

## 1. WellandValleyCC.github.io (master branch)

This is the **source-of-truth** repository for:

- The ClubSiteGenerator (C#)
- The ClubProcessor (C#)
- The Python extraction scripts
- **All authoritative static assets** for both the Round Robin site and the WVCC main site
- The generated output folders (local only, ignored by git)

It contains two canonical asset folders:

    RoundRobinSiteAssets/
        assets/   (CSS, JS, static files)
        logos/    (club logos and Round Robin branding)

    ClubSiteAssets/
        assets/   (WVCC main site assets, future)

### Generated output folders:

    RoundRobinSiteOutput/   (generated Round Robin site, local only)
    SiteOutput/             (WVCC main site output, local only)



## 2. RoundRobinTT.github.io (main branch, /docs folder)

This repo contains the **published** Round Robin site.

It is the **deployment target only**.  
It does **not** contain the authoritative assets - those live in the WVCC repo.

## 🏗️ Local Development Workflow

To preview the Round Robin site locally, you generate the full static site into the `RoundRobinSiteOutput` folder. The workflow has four steps:

---

### **1. Extract Club Events from the Spreadsheet**

Run the Python extraction script from the root of the WVCC repo:

```
cd C:\repos\wvcc\WellandValleyCC.github.io\
python3 scripts/extract_club_events.py data/ClubEvents_2026.xlsx data/extracted/
```

This script:

- Reads the master spreadsheet  
- Normalises and validates the event rows  
- Writes CSV files into `data/extracted/`

---

### **2. Process Extracted Data**

Run the ClubProcessor to convert the extracted CSV into Round Robin–specific event data:

```
& "C:\repos\wvcc\WellandValleyCC.github.io\processor\ClubProcessor\bin\Debug\net9.0\ClubProcessor.exe" --mode events --file C:\repos\wvcc\WellandValleyCC.github.io\data\extracted\2026
```

This step:

- Reads the extracted CSVs  
- Applies Round Robin logic  
- Produces processed event data for the site generator  

---

### **3. Generate the Static Site**

Run the ClubSiteGenerator:

```
& "C:\repos\wvcc\WellandValleyCC.github.io\processor\ClubSiteGenerator\bin\Debug\net9.0\ClubSiteGenerator.exe" --year 2026
```

This produces the full static site into:

```
RoundRobinSiteOutput/
```

This folder is:

- local only  
- ignored by git  
- safe to delete and regenerate at any time  

---

### **4. Serve the Site Locally**

Preview the generated site using:

```
dotnet serve -d RoundRobinSiteOutput -o
```

Because `RoundRobinSiteOutput` is not version-controlled, it must contain copies of the Round Robin assets and logos from:

```
RoundRobinSiteAssets/
```

These are copied automatically via a local-only pre-build step.

## 🔄 Round Robin Site Generation Flow

The following diagram shows how data moves through the full Round Robin generation pipeline, from the master spreadsheet to the final static site served locally.

```
┌──────────────────────────────────────────────────────────────┐
│        WellandValleyCC.github.io (master branch)             │
│                                                              │
│   RoundRobinSiteAssets/      ClubSiteAssets/                 │
│      assets/                    assets/                      │
│      logos/                                                  │
│                                                              │
│   data/ClubEvents_YYYY.xlsx                                  │
│            │                                                 │
│            │ python3 scripts/extract_club_events.py          │
│            ▼                                                 │
│   data/extracted/YYYY/   (CSV files)                         │
│            │                                                 │
│            │ ClubProcessor.exe --mode events                 │
│            ▼                                                 │
│   SQLite database (Round Robin event tables)                 │
│            │                                                 │
│            │ ClubSiteGenerator.exe --year YYYY               │
│            ▼                                                 │
│   RoundRobinSiteOutput/   (generated static site)            │
└───────────┬──────────────────────────────────────────────────┘
            │
            │ dotnet serve -d RoundRobinSiteOutput -o
            ▼
┌──────────────────────────────────────────────────────────────┐
│                 Local Preview in Browser                     │
└──────────────────────────────────────────────────────────────┘
```

## ⚙️ Optional: Automatic Copying of Assets & Logos (Local Only)

For convenience, you may automate copying the following folders:

- `RoundRobinSiteAssets/assets`
- `RoundRobinSiteAssets/logos`
- `ClubSiteAssets/assets`

into:

- `RoundRobinSiteOutput/`
- `SiteOutput/`

This ensures your local preview always has the correct styling and branding.  
This step is local-only and does not affect other contributors.

---

### Visual Studio Local-Only Pre-Build Step

Create or edit the following file:

```
ClubSiteGenerator.csproj.user
```

Add this content:

```
<Project>
  <PropertyGroup>
    <PreBuildEvent>
      REM Round Robin assets
      xcopy /E /I /Y "C:\repos\wvcc\WellandValleyCC.github.io\RoundRobinSiteAssets\assets" "C:\repos\wvcc\WellandValleyCC.github.io\RoundRobinSiteOutput\assets"
      xcopy /E /I /Y "C:\repos\wvcc\WellandValleyCC.github.io\RoundRobinSiteAssets\logos"  "C:\repos\wvcc\WellandValleyCC.github.io\RoundRobinSiteOutput\logos"

      REM WVCC main site assets (folder may not exist yet — safe)
      xcopy /E /I /Y "C:\repos\wvcc\WellandValleyCC.github.io\ClubSiteAssets\assets" "C:\repos\wvcc\WellandValleyCC.github.io\SiteOutput\assets"
    </PreBuildEvent>
  </PropertyGroup>
</Project>
```

---

### Notes

- The authoritative copies of all assets live in the WVCC repo (`RoundRobinSiteAssets` and `ClubSiteAssets`).
- The copies inside `RoundRobinSiteOutput` and `SiteOutput` are generated and not tracked by git.
- If a folder does not yet exist, XCOPY simply does nothing and the build continues normally.

## 🧭 Repository Structure Summary

This section summarises the key folders involved in generating and publishing the Round Robin site.  
Only the WVCC repo contains authoritative assets and source code; the RoundRobinTT repo is publish‑only.

---

### **WellandValleyCC.github.io (master branch)**

Authoritative source for:

- ClubSiteGenerator (C#)
- ClubProcessor (C#)
- Python extraction scripts
- All Round Robin and WVCC static assets
- Local‑only generated output folders

Key folders:

```
RoundRobinSiteAssets/
    assets/
    logos/

ClubSiteAssets/
    assets/

data/
    ClubEvents_YYYY.xlsx
    extracted/YYYY/

RoundRobinSiteOutput/   (generated site, local only)
SiteOutput/             (future WVCC site, local only)
```

---

### **RoundRobinTT.github.io (main branch)**

This repo contains:

- The published Round Robin static site
- Served from the `/docs` folder via GitHub Pages

Key folder:

```
docs/   (published Round Robin site)
```

This repo does **not** contain authoritative assets.  
It is updated only by copying the generated output from `RoundRobinSiteOutput/`.

---

### **Publishing Workflow Summary**

1. Generate the site locally into `RoundRobinSiteOutput/`
2. Copy the contents of `RoundRobinSiteOutput/` into:
   ```
   RoundRobinTT.github.io/docs/
   ```
3. Commit and push to publish the updated site

Only the WVCC repo contains the real source of truth.  
The RoundRobinTT repo is a deployment target only.

## 📝 Updating the Published Round Robin Site

The Round Robin site is published from the `RoundRobinTT.github.io` repository.  
Only the `/docs` folder in that repo is served by GitHub Pages.

This repo contains **no source code and no authoritative assets**.  
It is a deployment target only.

---

### **Publishing Steps**

After generating the site locally into:

```
RoundRobinSiteOutput/
```

copy the entire contents of that folder into:

```
C:\repos\wvcc\RoundRobinTT.github.io\docs\
```

You can do this manually or with a simple XCOPY command:

```
xcopy /E /I /Y "C:\repos\wvcc\WellandValleyCC.github.io\RoundRobinSiteOutput" "C:\repos\wvcc\RoundRobinTT.github.io\docs"
```

Then commit and push:

```
git add .
git commit -m "Publish updated Round Robin site for YYYY"
git push
```

GitHub Pages will automatically rebuild and publish the updated site.

---

### **Important Notes**

- Only the WVCC repo contains the real source of truth.
- The RoundRobinTT repo should never contain:
  - Python scripts  
  - C# source code  
  - Asset folders  
  - Extracted data  
  - SQLite databases  
- It should contain **only** the generated static site inside `/docs`.

This keeps the deployment repo clean, predictable, and easy for future maintainers.

## 🧪 Testing & Validation

Before publishing the Round Robin site, it’s important to validate that all data, assets, and generated pages are correct.  
This section outlines the recommended checks to perform after generating the site locally.

---

### **1. Validate Extracted Data**

After running the Python extraction script, confirm:

- All expected events appear in `data/extracted/YYYY/`
- No rows were skipped due to validation errors
- Event dates, times, and course codes match the spreadsheet
- Club names and categories are normalised correctly

Optional: open the CSV files directly to spot‑check values.

---

### **2. Validate Processed Data (SQLite)**

After running the ClubProcessor:

- Open the generated SQLite database (using DB Browser for SQLite or similar)
- Check that:
  - All events for the year are present
  - Round Robin scoring fields are populated
  - Event metadata (distance, course, organiser) is correct
  - No duplicate events exist
  - No missing fields appear in the tables

This ensures the generator receives clean, consistent data.

---

### **3. Validate the Generated Static Site**

After running the ClubSiteGenerator:

- Open `RoundRobinSiteOutput/` in a browser (via `dotnet serve`)
- Check:
  - Home page loads correctly
  - Event list shows all expected events
  - Individual event pages render correctly
  - Results tables load and sort properly
  - Navigation links work
  - CSS and logos appear correctly
  - No missing images or broken links

---

### **4. Validate Asset Copying**

If using the optional pre‑build XCOPY step:

- Confirm that:
  - `assets/` exists inside `RoundRobinSiteOutput/`
  - `logos/` exists inside `RoundRobinSiteOutput/`
  - Stylesheets and JS files load correctly
  - Logos appear on all pages

If anything is missing, re‑run the build or check the XCOPY paths.

---

### **5. Validate the Published Site (After Deployment)**

Once the updated site is pushed to `RoundRobinTT.github.io`:

- Visit the live site
- Hard‑refresh the browser (Ctrl+F5) to avoid cached assets
- Confirm:
  - All pages match the local version
  - No 404s or missing assets
  - GitHub Pages has rebuilt successfully

If GitHub Pages shows an error, check the repo’s Pages settings and ensure the `/docs` folder is selected.

---

### Summary

Performing these checks ensures:

- The spreadsheet was interpreted correctly  
- The processor produced valid Round Robin data  
- The generator created a complete static site  
- Assets and logos are present  
- The published site matches the local preview  

This makes the workflow reliable and contributor‑proof.

## 🧹 Cleaning Up & Regenerating the Site

Because the Round Robin site is fully static and generated from source data, it is safe to delete and regenerate all output at any time.  
This section explains what can be safely removed, what should never be deleted, and how to reset your environment if something becomes inconsistent.

---

### **Safe to Delete at Any Time**

The following folders are *generated output* and can be removed without risk:

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

Deleting these does **not** affect any authoritative data or assets.  
They will be recreated automatically the next time you run:

- the Python extraction script  
- the ClubProcessor  
- the ClubSiteGenerator  

---

### **Never Delete These Folders**

These contain the authoritative source of truth:

```
RoundRobinSiteAssets/
ClubSiteAssets/
data/ClubEvents_YYYY.xlsx
processor/   (C# source code)
scripts/     (Python extraction scripts)
```

If you remove these, you will lose real data or source code.

---

### **When to Clean & Regenerate**

You should clean and regenerate when:

- The spreadsheet changes  
- You add or remove events  
- You update Round Robin scoring logic  
- You change assets (CSS, JS, logos)  
- You suspect stale data in the output  
- The generated site looks incorrect or incomplete  

A clean rebuild ensures the output reflects the latest data and logic.

---

### **Recommended Reset Procedure**

1. Delete the following folders:

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

2. Re-run the extraction:

```
python3 scripts/extract_club_events.py data/ClubEvents_YYYY.xlsx data/extracted/
```

3. Re-run the processor:

```
ClubProcessor.exe --mode events --file data/extracted/YYYY
```

4. Re-run the generator:

```
ClubSiteGenerator.exe --year YYYY
```

5. Preview locally:

```
dotnet serve -d RoundRobinSiteOutput -o
```

This guarantees a clean, reproducible build.

---

### Summary

Cleaning and regenerating is:

- safe  
- fast  
- reproducible  
- the recommended workflow  

The authoritative data always lives in the WVCC repo; everything else is disposable.

## 🧭 Troubleshooting Common Issues

This section lists the most common problems encountered during extraction, processing, generation, or publishing — along with the likely causes and how to fix them.  
All issues are safe to resolve locally because the authoritative data is never touched.

---

### **1. Missing CSS or Logos in Local Preview**

**Symptoms:**
- Pages load but look unstyled  
- Logos do not appear  
- Browser console shows 404s for `/assets/...` or `/logos/...`

**Likely Cause:**
Assets were not copied into `RoundRobinSiteOutput/`.

**Fix:**
- Re-run the build if using the pre-build XCOPY step  
- Or manually copy:

```
RoundRobinSiteAssets/assets → RoundRobinSiteOutput/assets
RoundRobinSiteAssets/logos  → RoundRobinSiteOutput/logos
```

---

### **2. Event Pages Missing or Empty**

**Symptoms:**
- Some events do not appear in the event list  
- Individual event pages exist but contain no data  
- SQLite tables appear incomplete

**Likely Cause:**
The extracted CSV files are stale or incomplete.

**Fix:**
1. Delete:
   ```
   data/extracted/YYYY/
   ```
2. Re-run the extraction script  
3. Re-run the processor  
4. Re-run the generator  

---

### **3. Wrong Year Appearing in the Generated Site**

**Symptoms:**
- The site shows last year’s events  
- URLs contain the wrong year  
- The homepage lists outdated dates

**Likely Cause:**
The generator was run with the wrong `--year` argument.

**Fix:**
Run:

```
ClubSiteGenerator.exe --year YYYY
```

---

### **4. GitHub Pages Shows an Old Version of the Site**

**Symptoms:**
- Local preview is correct  
- Live site still shows old content  
- Hard refresh does not fix it

**Likely Cause:**
The `/docs` folder in the RoundRobinTT repo was not fully updated.

**Fix:**
1. Delete everything inside:
   ```
   RoundRobinTT.github.io/docs/
   ```
2. Copy the entire contents of:
   ```
   RoundRobinSiteOutput/
   ```
3. Commit and push again

---

### **5. GitHub Pages Build Error**

**Symptoms:**
- GitHub Pages shows a red error banner  
- Site fails to publish  
- Pages settings show “Build failed”

**Likely Cause:**
The Pages source folder is not set to `/docs`.

**Fix:**
In the RoundRobinTT repo:

1. Go to **Settings → Pages**  
2. Set **Source = Deploy from branch**  
3. Set **Branch = main /docs**  

---

### **6. Python Extraction Script Fails**

**Symptoms:**
- Script exits with an error  
- CSV files are not created  
- Terminal shows a row parsing issue

**Likely Causes:**
- Spreadsheet contains unexpected formatting  
- A row is missing a required field  
- A column name changed

**Fix:**
- Open the spreadsheet and inspect the row mentioned in the error  
- Ensure all required columns exist  
- Re-run the script

---

### **7. ClubProcessor Crashes or Produces No Output**

**Symptoms:**
- No SQLite database is created  
- Console shows a missing file or invalid path  
- Event tables are empty

**Likely Cause:**
The `--file` argument points to the wrong extracted folder.

**Fix:**
Use the full path:

```
ClubProcessor.exe --mode events --file data/extracted/YYYY
```

---

### **8. Generator Runs but Output Folder Is Empty**

**Symptoms:**
- No HTML files appear in `RoundRobinSiteOutput/`  
- Console shows no errors  
- Output folder remains unchanged

**Likely Cause:**
The generator was run from the wrong working directory.

**Fix:**
Run it from the WVCC repo root:

```
cd C:\repos\wvcc\WellandValleyCC.github.io\
ClubSiteGenerator.exe --year YYYY
```

---

### Summary

Most issues come from:

- stale extracted data  
- incorrect paths  
- missing assets  
- wrong working directory  
- Pages not pointing to `/docs`  

All problems are safe to fix locally because the WVCC repo contains the authoritative source of truth.


## 🗂️ Appendix: Paths, Commands & Quick Reference

This appendix provides a concise reference for all key paths, commands, and workflows used throughout the Round Robin site generation process.  
It is designed for fast lookup and to help future contributors onboard quickly.

---

## **Key Repository Paths**

### **WVCC Repo (Authoritative Source)**

```
WellandValleyCC.github.io/
    RoundRobinSiteAssets/
        assets/
        logos/

    ClubSiteAssets/
        assets/

    data/
        ClubEvents_YYYY.xlsx
        extracted/YYYY/

    processor/
        ClubProcessor/
        ClubSiteGenerator/

    scripts/
        extract_club_events.py

    RoundRobinSiteOutput/   (generated, local only)
    SiteOutput/             (generated, local only)
```

### **RoundRobinTT Repo (Published Site)**

```
RoundRobinTT.github.io/
    docs/   (published static site)
```

---

## **Core Commands**

### **1. Extract Events from Spreadsheet**

```
python3 scripts/extract_club_events.py data/ClubEvents_YYYY.xlsx data/extracted/
```

### **2. Process Extracted Data**

```
ClubProcessor.exe --mode events --file data/extracted/YYYY
```

### **3. Generate the Static Site**

```
ClubSiteGenerator.exe --year YYYY
```

### **4. Serve Locally**

```
dotnet serve -d RoundRobinSiteOutput -o
```

---

## **Optional Local-Only Asset Copying**

### XCOPY Commands

```
xcopy /E /I /Y "RoundRobinSiteAssets\assets" "RoundRobinSiteOutput\assets"
xcopy /E /I /Y "RoundRobinSiteAssets\logos"  "RoundRobinSiteOutput\logos"
xcopy /E /I /Y "ClubSiteAssets\assets"       "SiteOutput\assets"
```

---

## **Publishing to GitHub Pages**

### Copy Generated Site to Deployment Repo

```
xcopy /E /I /Y "RoundRobinSiteOutput" "C:\repos\wvcc\RoundRobinTT.github.io\docs"
```

### Commit & Push

```
git add .
git commit -m "Publish updated Round Robin site for YYYY"
git push
```

---

## **Clean Rebuild Checklist**

1. Delete:
   ```
   RoundRobinSiteOutput/
   SiteOutput/
   data/extracted/YYYY/
   ```
2. Re-run extraction  
3. Re-run processor  
4. Re-run generator  
5. Preview locally  

---

## **Quick Diagnostic Guide**

| Problem | Likely Cause | Fix |
|--------|--------------|-----|
| Missing CSS/logos | Assets not copied | Re-run XCOPY or copy manually |
| Missing events | Stale extracted data | Delete `extracted/YYYY` and re-run |
| Wrong year | Wrong generator argument | Run with `--year YYYY` |
| Live site outdated | `/docs` not fully updated | Delete and recopy output |
| Pages build error | Wrong Pages source | Set to `main /docs` |

---

## **Summary**

This appendix consolidates all essential paths, commands, and workflows into a single reference.  
It is safe to regenerate all output at any time because the WVCC repo contains the authoritative source of truth.

## 🧩 Appendix: Environment Setup & Developer Notes

This appendix documents the recommended development environment for working on the Round Robin and WVCC site generation workflow.  
It ensures future contributors can reproduce the setup reliably.

---

## **Required Software**

### **1. Python 3.x**

Used for extracting event data from the ClubEvents spreadsheet.

Verify installation:

```
python3 --version
```

Required packages:

```
pandas
openpyxl
```

Install with:

```
pip install pandas openpyxl
```

---

### **2. .NET SDK (version 9.0 or later)**

Required for:

- ClubProcessor (C#)
- ClubSiteGenerator (C#)
- Running `dotnet serve` for local preview

Verify installation:

```
dotnet --version
```

---

### **3. SQLite Browser (optional but recommended)**

Used to inspect the generated Round Robin database.

Any of the following tools work:

- DB Browser for SQLite  
- SQLiteStudio  
- Azure Data Studio with SQLite extension  

---

### **4. Git**

Required for cloning and updating both repositories.

Verify installation:

```
git --version
```

---

## **Recommended Folder Layout**

To avoid path issues, use this structure:

```
C:\repos\wvcc\
    WellandValleyCC.github.io\
    RoundRobinTT.github.io\
```

This ensures:

- XCOPY paths work without modification  
- Scripts and executables can reference consistent absolute paths  
- Contributors can follow documentation without adjusting examples  

---

## **Building the C# Projects**

From the WVCC repo root:

```
dotnet build processor/ClubProcessor
dotnet build processor/ClubSiteGenerator
```

This produces:

```
processor/ClubProcessor/bin/Debug/net9.0/
processor/ClubSiteGenerator/bin/Debug/net9.0/
```

Executables can be run directly from these folders.

---

## **Running the Python Script from Any Directory**

If desired, add the `scripts/` folder to your PATH.

Example (PowerShell):

```
setx PATH "$env:PATH;C:\repos\wvcc\WellandValleyCC.github.io\scripts"
```

This allows:

```
extract_club_events.py ...
```

instead of:

```
python3 scripts/extract_club_events.py ...
```

---

## **Editor Recommendations**

### **Visual Studio Code**

Recommended extensions:

- Python  
- C# Dev Kit  
- SQLite Viewer  
- Markdown All in One  

### **Visual Studio (Windows)**

Recommended for:

- Running the pre-build XCOPY step  
- Debugging C# processors  
- Managing solution-wide builds  

---

## **Common Path Variables (for future maintainers)**

These are the most frequently referenced paths:

```
%WVCC%      = C:\repos\wvcc\WellandValleyCC.github.io
%RR_TT%     = C:\repos\wvcc\RoundRobinTT.github.io
%EXTRACTED% = %WVCC%\data\extracted\YYYY
%OUTPUT%    = %WVCC%\RoundRobinSiteOutput
```

Using these variables avoids hard‑coding paths in scripts.

---

## **Summary**

This appendix ensures contributors can:

- Install the correct tools  
- Build the C# projects  
- Run the Python extraction script  
- Inspect SQLite output  
- Maintain consistent folder structures  
- Avoid path‑related issues  

A reproducible environment is essential for a contributor‑proof workflow.

## 🧱 Appendix: Design Principles & Future-Proofing

This appendix documents the architectural principles behind the Round Robin and WVCC site‑generation workflow.  
It explains *why* the system is structured the way it is, and how future contributors can extend it safely.

---

## **Core Design Principles**

### **1. Single Source of Truth**

All authoritative data, assets, and source code live in:

```
WellandValleyCC.github.io/
```

This ensures:

- No duplication of assets  
- No divergence between repos  
- No risk of editing the wrong copy  
- A predictable workflow for contributors  

The RoundRobinTT repo is *publish-only*.

---

### **2. Fully Regenerable Output**

The system is designed so that all generated content can be deleted and recreated at any time:

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

This makes the workflow:

- deterministic  
- reproducible  
- contributor-proof  

No generated file is ever committed to the WVCC repo.

---

### **3. Explicit, Intention-Revealing Paths**

All scripts and commands use clear, absolute paths in documentation to avoid ambiguity:

```
C:\repos\wvcc\WellandValleyCC.github.io\
```

This helps future maintainers understand:

- where data lives  
- where output goes  
- which repo is authoritative  

Contributors may later introduce environment variables or config files, but clarity comes first.

---

### **4. Separation of Concerns**

Each stage of the pipeline has a single responsibility:

| Component | Responsibility |
|----------|----------------|
| Python script | Extract raw event data from spreadsheet |
| ClubProcessor | Transform extracted data into Round Robin domain model |
| SQLite DB | Temporary structured storage for processed data |
| ClubSiteGenerator | Generate static HTML site |
| RoundRobinTT repo | Publish-only deployment target |

This separation makes debugging and extension straightforward.

---

### **5. Local-Only Enhancements Never Affect CI**

Features like XCOPY asset copying are:

- optional  
- local-only  
- never committed  
- never required for CI or other contributors  

This prevents environment-specific behaviour from leaking into the shared workflow.

---

### **6. Human-Readable, Markdown-First Documentation**

All documentation:

- uses plain Markdown  
- avoids hidden formatting  
- avoids editor-specific features  
- is designed for GitHub readability  

This ensures future contributors can read and update docs without special tools.

---

## **Future-Proofing Considerations**

### **1. Adding New Event Types**

If the club introduces new event categories:

- Update the spreadsheet schema  
- Update the Python extractor  
- Update the processor’s domain model  
- Update the generator templates  

Because each stage is isolated, changes remain manageable.

---

### **2. Supporting Multiple Years Simultaneously**

The system already supports year-based filtering.  
Future enhancements may include:

- multi-year archives  
- year switchers in the UI  
- automated year rollover scripts  

The current structure is compatible with all of these.

---

### **3. Migrating to a Database-Backed Live Site**

If the club ever wants a dynamic site:

- The processor and SQLite schema already define a clean domain model  
- The generator templates can be repurposed as server-side views  
- The static site can remain as a fallback or archive  

The architecture intentionally avoids tight coupling.

---

### **4. Adding Automated Publishing**

Future maintainers may add:

- GitHub Actions to copy output to RoundRobinTT  
- Scheduled rebuilds  
- Automatic validation of spreadsheets  

The current workflow is compatible with automation but does not depend on it.

---

## **Summary**

The system is intentionally:

- simple  
- explicit  
- reproducible  
- contributor-proof  
- future-ready  

This appendix explains the reasoning behind the architecture so future maintainers can extend it confidently.

## 🧩 Appendix: Glossary of Terms

This glossary defines the key terms used throughout the Round Robin and WVCC site‑generation workflow.  
It is intended to help future contributors quickly understand the domain language and technical vocabulary.

---

## **Round Robin Terms**

### **Round Robin**
A season‑long competition where riders earn points based on participation and performance across selected club events.

### **Event**
A single time trial or club competition, defined by:
- date  
- time  
- course  
- organiser  
- category  

### **Category**
The type of event, such as:
- Open TT  
- Club TT  
- Handicap  
- Hill Climb  
- Reliability Ride  

### **Points**
The scoring system used to rank riders in the Round Robin competition.

---

## **Technical Terms**

### **Extraction**
The process of converting the ClubEvents spreadsheet into structured CSV files using the Python script.

### **Processing**
The step where the ClubProcessor reads extracted CSV files and produces a structured SQLite database.

### **Generation**
The step where the ClubSiteGenerator reads the SQLite database and produces the static HTML site.

### **Static Site**
A website made entirely of pre‑generated HTML, CSS, JS, and images — no server‑side code.

### **SQLite**
A lightweight, file‑based database used as a temporary store for processed event data.

### **XCOPY**
A Windows command used to copy folders recursively.  
Used locally to copy assets into output folders.

---

## **Repository Terms**

### **WVCC Repo**
The authoritative repository containing:
- source code  
- assets  
- scripts  
- data  
- generators  

### **RoundRobinTT Repo**
The publish‑only repository containing:
- the generated static site in `/docs`  
- no source code  
- no assets  
- no scripts  

### **Output Folder**
A folder containing generated HTML files.  
Safe to delete at any time.

---

## **Markdown & Documentation Terms**

### **Code Fence**
A block of text wrapped in backticks to prevent Markdown from interpreting it.

### **ASCII Diagram**
A diagram made from plain text characters (│ ┌ ┘ etc.) used to illustrate workflows.

### **Absolute Path**
A full filesystem path beginning with a drive letter, e.g.:

```
C:\repos\wvcc\WellandValleyCC.github.io\
```

Used for clarity and reproducibility.

---

## **Summary**

This glossary provides a quick reference for both technical and domain‑specific terminology.  
It ensures future contributors can understand the workflow without needing prior context.

## 🧩 Appendix: Frequently Asked Questions (FAQ)

This appendix collects the most common questions future contributors are likely to ask when working with the Round Robin and WVCC site‑generation workflow.  
All answers are concise, practical, and aligned with the architecture described in this documentation.

---

## **General Workflow**

### **Q: Where does the real source of truth live?**
In the WVCC repo:

```
WellandValleyCC.github.io/
```

This repo contains all source code, assets, scripts, and data.

---

### **Q: What is the RoundRobinTT repo for?**
It is a **publish-only** repo.  
Only the generated static site goes into:

```
RoundRobinTT.github.io/docs/
```

No source code or assets should ever be added there.

---

### **Q: Can I delete the output folders?**
Yes.  
These folders are safe to delete at any time:

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

They will be regenerated automatically.

---

## **Extraction & Processing**

### **Q: The Python script failed — what should I check?**
- Spreadsheet formatting  
- Missing required columns  
- Unexpected blank rows  
- Incorrect file path  

Fix the spreadsheet row mentioned in the error and re-run the script.

---

### **Q: The processor created an empty SQLite database — why?**
Most likely the `--file` argument pointed to the wrong extracted folder.

Use:

```
ClubProcessor.exe --mode events --file data/extracted/YYYY
```

---

## **Site Generation**

### **Q: The generated site is missing events — what happened?**
The extracted CSV files are stale.  
Delete:

```
data/extracted/YYYY/
```

Then re-run extraction, processing, and generation.

---

### **Q: The site shows the wrong year — how do I fix it?**
Run the generator with the correct year:

```
ClubSiteGenerator.exe --year YYYY
```

---

### **Q: CSS or logos are missing — what should I do?**
Assets were not copied into the output folder.  
Re-run the XCOPY step or copy manually:

```
RoundRobinSiteAssets/assets → RoundRobinSiteOutput/assets
RoundRobinSiteAssets/logos  → RoundRobinSiteOutput/logos
```

---

## **Publishing**

### **Q: The live site didn’t update — what went wrong?**
The `/docs` folder in the RoundRobinTT repo was not fully replaced.

Fix:

1. Delete everything inside `docs/`
2. Copy everything from `RoundRobinSiteOutput/`
3. Commit and push

---

### **Q: GitHub Pages shows a build error — how do I fix it?**
Ensure Pages is configured to serve from:

```
Branch: main
Folder: /docs
```

---

## **Development Environment**

### **Q: Do I need Visual Studio?**
No — but it helps if you want to use the optional local-only XCOPY pre-build step.

VS Code works perfectly for:

- Python  
- C#  
- SQLite  
- Markdown  

---

### **Q: Do I need to install SQLite?**
No.  
The processor creates a SQLite file automatically.  
You only need a viewer if you want to inspect it.

---

## **Architecture & Design**

### **Q: Why is the SQLite database not committed?**
Because it is generated output.  
It must always reflect the latest extracted data, so committing it would cause confusion.

---

### **Q: Why are assets stored only in the WVCC repo?**
To avoid duplication and drift.  
The RoundRobinTT repo should contain only the published site.

---

### **Q: Why use absolute paths in documentation?**
For clarity and reproducibility.  
Future contributors can always adapt them later.

---

## **Summary**

This FAQ provides quick answers to the most common questions about the workflow.  
It helps future contributors understand the system without reading the entire documentation end‑to‑end.

## 🧩 Appendix: Contributor Onboarding Checklist

This appendix provides a concise, step‑by‑step onboarding checklist for new contributors.  
It ensures anyone joining the project can get fully set up, generate the site, and publish updates without needing to read the entire documentation first.

---

## **1. Clone Both Repositories**

```
C:\repos\wvcc\
    WellandValleyCC.github.io\
    RoundRobinTT.github.io\
```

Clone them:

```
git clone https://github.com/WellandValleyCC/WellandValleyCC.github.io
git clone https://github.com/WellandValleyCC/RoundRobinTT.github.io
```

---

## **2. Install Required Software**

- Python 3.x  
- .NET SDK 9.0+  
- Git  
- SQLite viewer (optional)  
- VS Code or Visual Studio  

Verify:

```
python3 --version
dotnet --version
git --version
```

---

## **3. Understand the Folder Structure**

Authoritative repo:

```
WellandValleyCC.github.io/
```

Publish-only repo:

```
RoundRobinTT.github.io/docs/
```

Generated output (safe to delete):

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

---

## **4. Run the Full Local Build Pipeline**

### Extract:

```
python3 scripts/extract_club_events.py data/ClubEvents_YYYY.xlsx data/extracted/
```

### Process:

```
ClubProcessor.exe --mode events --file data/extracted/YYYY
```

### Generate:

```
ClubSiteGenerator.exe --year YYYY
```

### Preview:

```
dotnet serve -d RoundRobinSiteOutput -o
```

---

## **5. (Optional) Enable Local Asset Copying**

Add a `.csproj.user` file with XCOPY commands to automatically copy:

- `RoundRobinSiteAssets/assets`
- `RoundRobinSiteAssets/logos`
- `ClubSiteAssets/assets`

into the output folders.

This step is local-only and never committed.

---

## **6. Validate the Output**

Check:

- All events appear  
- Event pages load  
- CSS and logos display  
- No broken links  
- SQLite tables contain expected data  

---

## **7. Publish the Updated Site**

Copy:

```
RoundRobinSiteOutput/ → RoundRobinTT.github.io/docs/
```

Then:

```
git add .
git commit -m "Publish updated Round Robin site for YYYY"
git push
```

Ensure GitHub Pages is set to:

```
Branch: main
Folder: /docs
```

---

## **8. Know What Is Safe to Delete**

Safe:

```
RoundRobinSiteOutput/
SiteOutput/
data/extracted/YYYY/
```

Never delete:

```
RoundRobinSiteAssets/
ClubSiteAssets/
data/ClubEvents_YYYY.xlsx
processor/
scripts/
```

---

## **9. Know Where to Ask Questions**

If unsure:

- Check the Troubleshooting section  
- Check the FAQ  
- Check the Glossary  
- Review the ASCII workflow diagram  

---

## **Summary**

This checklist gives new contributors everything they need to:

- set up their environment  
- run the full pipeline  
- validate output  
- publish updates  
- avoid common pitfalls  

It is designed to make onboarding fast, predictable, and contributor‑proof.

## 🧩 Appendix: Maintenance Tasks & Annual Rollover

This appendix documents the tasks required at the end of each season and the start of a new one.  
It ensures the Round Robin workflow remains smooth, predictable, and contributor‑proof year after year.

---

## **1. End‑of‑Season Tasks**

These tasks should be completed once the final event of the season has been processed and published.

### **1.1 Archive the Published Site**

The RoundRobinTT repo contains the live site for the current year.  
At year‑end, create a branch or tag to preserve the final state:

```
git tag -a "RR-YYYY-final" -m "Final Round Robin site for YYYY"
git push --tags
```

This provides a permanent snapshot of the season.

---

### **1.2 Export Final Results (Optional)**

If the club wants a static export of final standings:

- Open the generated site locally  
- Save the final standings page as a PDF  
- Store it in a club archive folder (not in the repo)  

This step is optional and outside the automated workflow.

---

## **2. Preparing for a New Season**

### **2.1 Create the New Spreadsheet**

Copy the previous year’s spreadsheet:

```
data/ClubEvents_YYYY.xlsx → data/ClubEvents_YYYY+1.xlsx
```

Update:

- event dates  
- courses  
- organisers  
- categories  
- any new event types  

Ensure column names remain unchanged unless a major version bump is planned.

---

### **2.2 Create the New Extracted Folder**

Create:

```
data/extracted/YYYY+1/
```

This folder will be populated automatically by the extraction script.

---

### **2.3 Update Documentation References**

Search for references to the old year:

- README  
- workflow docs  
- examples  
- commands  

Update them to the new year.

---

## **3. Running the First Build of the New Season**

Once the new spreadsheet is ready:

### Extract:

```
python3 scripts/extract_club_events.py data/ClubEvents_YYYY+1.xlsx data/extracted/
```

### Process:

```
ClubProcessor.exe --mode events --file data/extracted/YYYY+1
```

### Generate:

```
ClubSiteGenerator.exe --year YYYY+1
```

### Preview:

```
dotnet serve -d RoundRobinSiteOutput -o
```

Verify:

- all events appear  
- dates are correct  
- categories are correct  
- no missing assets  

---

## **4. Resetting the Published Site for the New Season**

At the start of a new season, the RoundRobinTT repo should be reset to a clean state.

### **4.1 Clear the `/docs` Folder**

Delete everything inside:

```
RoundRobinTT.github.io/docs/
```

### **4.2 Copy the New Season’s Output**

Copy:

```
RoundRobinSiteOutput/ → RoundRobinTT.github.io/docs/
```

### **4.3 Commit & Push**

```
git add .
git commit -m "Start Round Robin season YYYY+1"
git push
```

The live site will now show the new season’s initial state.

---

## **5. Optional: Add a Year Switcher (Future Enhancement)**

The current workflow supports one active year at a time.  
A future enhancement could add:

- a year dropdown  
- archived year pages  
- multi‑year navigation  

This would require:

- generator template updates  
- URL structure changes  
- additional data handling  

This is intentionally left out of the current workflow to keep it simple.

---

## **6. Summary**

Annual rollover requires:

- archiving the old season  
- preparing the new spreadsheet  
- updating documentation  
- running the first build  
- resetting the published site  

This appendix ensures the transition between seasons is smooth, predictable, and contributor‑proof.



