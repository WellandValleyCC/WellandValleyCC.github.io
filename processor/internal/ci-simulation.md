# Local CI Simulation Guide

This guide explains how to simulate the GitHub Actions workflow locally using `simulate-ci.ps1`. It restores, builds, tests, and runs the processor based on detected input files — mimicking the automation pipeline used in production.

---

## 🧰 Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/en-us/download)
- PowerShell 5.1+ or PowerShell Core
- A valid `.sln` file at `processor/ClubProcessor.sln`
- Input files in `data/` (e.g. `competitors_2025.csv` or `Club Events.xlsx`)

---

## 🚀 Running the Simulation

From the repo root:

```powershell
cd processor
.\simulate-ci.ps1

```

This will:

Restore and build the solution

Run all unit tests

Detect a trigger file in ../data/

Run the processor in the appropriate mode (competitors or events)

Save output to a timestamped log file in logs/

## Log Output

Logs are saved to 

```
processor/logs/simulate-ci-YYYYMMDD-HHMMSS.log
```

These are ignored by Git via `.gitignore`:

```
processor/logs/
```

## Test Coverage

Unit tests are located in:

```
processor/ClubProcessor.Tests/
```

They use an in-memory database for isolation and fast execution. All tests must pass before pushing changes.

## Trigger File Detection

The script looks for the first matching file in `data/`:

- `competitors_*.csv` → runs in `competitors` mode

- `Club Events.xlsx` → runs in `events` mode

You can override this logic by editing the detection block in `simulate-ci.ps1`.

## Clean Up

To remove old logs:

```
Remove-Item .\logs\*.log
```

## Tips for Contributors

- Use `simulate-ci.ps1` before every commit to catch issues early

- Keep `data/` populated with realistic test files

- Avoid committing `.db` or `.log` files — they’re ignored by default

- For help, check `README.md` or ask in the club’s dev channel
