# WVCC — Membership List Update (Ops‑Only)

## 1. Save the new file
- Drop the received `.xlsx` into:
  C:\repos\wvcc\WellandValleyCC.club-membership-private\data\
- Keep filename exactly as supplied.

## 2. Update competitors.meta.json
- Edit:
  C:\repos\wvcc\WellandValleyCC.club-membership-private\data\competitors.meta.json

- Set:
  "source": "<the new filename>"
  "use_simulated_import_date": false   (true only if list arrives after an event)

```json
{
  "source": "26.01.08  2026 membership list.xlsx",
  "use_simulated_import_date": false,
  "simulated_import_date": "2026-03-21"
}
```

## 3. Commit + push (private repo)
```bash
git add "data/<filename>" data/competitors.meta.json
git commit -m "Updated membership list from JW"
git push
```

## 4. Confirm pipeline run
- Check GitHub Actions for the private repo.
- Ensure the run completes and publishes competitors_YYYY.csv to the public repo.

## 5. Confirm public repo pipeline run
- The commit that publishes competitors_YYYY.csv automatically triggers the Club Processor workflow (mode: competitor).
- This rebuilds the competitor SQLite database and commits the updated DB back into the public repo.

## 6. Pull updated CSV (public repo)
- In:
  C:\repos\wvcc\WellandValleyCC.github.io

```bash
git pull
```

- Confirm a new commit tagged like:
  clubdata-competitors-YYYY-<timestamp>

## 7. Open workbooks
- Open:
  ClubEvents_YYYY.xlsx
- Open:
  competitors_YYYY.csv

## 8. Snapshot the CSV
- Rename CSV sheet to:
  MMDD_competitors_YYYY

## 9. Copy into events workbook
- From CSV sheet: select A2:H<lastrow>
- Paste into Competitors sheet at A2 in ClubEvents_YYYY.xlsx

## 10. Commit updated events workbook
```bash
git add data/ClubEvents_YYYY.xlsx
git commit -m "Competitors updated as per JW <date> [ci skip]"
git push
```

## Done
- Membership list updated
- CSV published
- Events workbook refreshed
- Snapshot preserved
