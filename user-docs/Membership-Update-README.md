# How to incorporate a new membership list
This guide explains the steps required to bring a newly received membership list into the system and ensure it flows correctly through the automated pipelines.

## Prerequisites
- Access to the private membership repository  
  Clone location (example):  
  `C:\repos\wvcc\WellandValleyCC.club-membership-private`
- Access to the public website repository  
  Clone location (example):  
  `C:\repos\wvcc\WellandValleyCC.github.io`
- The latest membership list from the Membership Secretary (usually via email)  
  Filename example:  
  `26.01.08  2026 membership list.xlsx`  
  *Note the double space — use the filename exactly as supplied.*

---

## Steps to incorporate a new membership list

### 1: Save the new membership file
1. Save the received `.xlsx` file into the private membership repo:

   `C:\repos\wvcc\WellandValleyCC.club-membership-private\data\26.01.08  2026 membership list.xlsx`

2. Keep the filename exactly as supplied.

---

### 2: Update `competitors.meta.json`
1. Open:

   `C:\repos\wvcc\WellandValleyCC.club-membership-private\data\competitors.meta.json`

2. Update it to reference the new file. Example:

```json
{
  "source": "26.01.08  2026 membership list.xlsx",
  "use_simulated_import_date": false,
  "simulated_import_date": "2026-03-21"
}
```

3. Choosing `use_simulated_import_date`:
   - `false` — normal case.
   - `true` — only if the updated list arrives *after* an event and you need to “pretend” the import happened earlier so that membership is treated as valid for that event.

4. Ensure the `source` filename matches exactly, including spaces.

---

### 3: Commit the updated files (private repo)
1. Stage the updated files (the new `.xlsx` and the `.json`):

```bash
git add "data/26.01.08  2026 membership list.xlsx" data/competitors.meta.json
```

2. Commit with a clear message:

```bash
git commit -m "Updated membership list from JW"
```

3. Push to `main`:

```bash
git push
```

Example commit:

`f127196 (HEAD -> main, origin/main, origin/HEAD) Updated membership list from JW`

---

### 4: Watch the pipeline run (private repo)
1. Go to GitHub Actions for the private repo:

   https://github.com/WellandValleyCC/WellandValleyCC.club-membership-private/actions

2. Confirm the pipeline runs successfully.

Example run:

   https://github.com/WellandValleyCC/WellandValleyCC.club-membership-private/actions/runs/23375274598

This pipeline publishes the new `competitors_2026.csv` into the public repo.

---

### 5: Confirm the new commit in the public repository
1. In your local clone of the public repo:

   `C:\repos\wvcc\WellandValleyCC.github.io`

2. Pull the latest changes:

```bash
git pull
```

3. Check the commit log on `master`:

```bash
git log master -n 12 --oneline
```

4. You should see a commit similar to:

```text
d6961217 (tag: clubdata-competitors-2026-20260321080039) Publish competitors_2026.csv with 207 rows
```

This commit is automatically created and tagged by the pipeline.

---

## Updating the events workbook

### 6: Open the events workbook
Open the events workbook in Excel:

`C:\repos\wvcc\WellandValleyCC.github.io\data\ClubEvents_2026.xlsx`

---

### 7: Open the newly generated CSV
Open the CSV file in Excel:

`C:\repos\wvcc\WellandValleyCC.github.io\data\competitors_2026.csv`

---

### 8: Rename the CSV sheet
Rename the sheet tab in the CSV workbook to:

`0321_competitors_2026`

where `0321` is today’s date (e.g. `MMDD`, or whatever convention you’re using consistently).

This preserves a historical snapshot of each membership list update over the season.

---

### 9: Copy the data into the events workbook
1. In the CSV workbook, on the `0321_competitors_2026` sheet:
   - Select the full data range, starting from row 2 (to skip the header).  
     Example:

     `A2:H208`

     where `208` is one more than the number of members in the CSV. Adjust this row number as needed.

2. Copy the selected range.

3. Switch to `ClubEvents_2026.xlsx` and go to the `Competitors` sheet.

4. Select cell `A2`.

5. Paste, overwriting the existing competitor data.

This approach keeps all references in the `Event_nn` sheets working correctly. Do **not** move or rename the `Competitors` sheet itself; only replace its data.

---

### 10: Save and commit the updated events workbook (public repo)
1. Save the updated `ClubEvents_2026.xlsx` workbook.

2. Commit with a `[ci skip]` suffix on the message to avoid triggering the events processing pipeline unnecessarily:

```bash
git add data/ClubEvents_2026.xlsx
git commit -m "Competitors updated as per JW 21/03/2026 [ci skip]"
git push
```

3. Why `[ci skip]`?
   - If you omit `[ci skip]`, the events will be processed.  
   - This is harmless, but not necessary at this stage if you are only updating membership data and not intending to process events.

---

## Summary
After completing these steps:

- The new membership list is stored in the private repo with the correct metadata.
- The private repo pipeline publishes an updated `competitors_2026.csv` to the public repo.
- The events workbook (`ClubEvents_2026.xlsx`) is updated with the latest competitor data.
- A dated snapshot of the membership list is preserved in a separate sheet (e.g. `0321_competitors_2026`).
- Everything is ready for the next event processing run when needed, without triggering unnecessary pipelines during the membership update itself.
