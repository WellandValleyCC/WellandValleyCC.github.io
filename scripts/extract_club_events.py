import pandas as pd
import sys
import os
import re

def extract_club_events(xlsx_path, output_dir):
    # Extract year from filename
    match = re.search(r'ClubEvents_(\d{4})\.xlsx$', os.path.basename(xlsx_path))
    if not match:
        print("[ERROR] Filename must match pattern: ClubEvents_YYYY.xlsx")
        sys.exit(1)
    year = match.group(1)

    print(f"[INFO] Reading workbook: {xlsx_path}")
    try:
        xl = pd.ExcelFile(xlsx_path)
    except Exception as e:
        print(f"[ERROR] Failed to open workbook: {e}")
        sys.exit(1)

    # Create output folders
    os.makedirs(output_dir, exist_ok=True)
    events_dir = os.path.join(output_dir, "events")
    os.makedirs(events_dir, exist_ok=True)

    # Extract calendar sheet
    if "calendar" in xl.sheet_names:
        print("[OK] Extracting calendar sheet")
        calendar_df = xl.parse("calendar")
        calendar_out = os.path.join(output_dir, f"calendar_{year}.csv")
        calendar_df.to_csv(calendar_out, index=False)
        print(f"[INFO] Saved to: {calendar_out} ({len(calendar_df)} rows)")
    else:
        print("[WARN] Sheet missing: calendar")

    # Extract competitors sheet (reference only)
    if "competitors" in xl.sheet_names:
        print("[INFO] Extracting competitors sheet (reference only)")
        competitors_df = xl.parse("competitors")
        competitors_out = os.path.join(output_dir, f"competitors_{year}.csv")
        competitors_df.to_csv(competitors_out, index=False)
        print(f"[INFO] Saved to: {competitors_out} ({len(competitors_df)} rows)")
    else:
        print("[WARN] Sheet missing: competitors")

    # Extract all Event (n) sheets
    event_sheets = [s for s in xl.sheet_names if re.match(r'Event \(\d+\)', s)]
    if not event_sheets:
        print("[WARN] No event sheets found")
    else:
        for sheet in event_sheets:
            event_num = re.search(r'\d+', sheet).group()
            print(f"[OK] Extracting event sheet: {sheet}")
            df = xl.parse(sheet)
            event_out = os.path.join(events_dir, f"Event_{event_num}.csv")
            df.to_csv(event_out, index=False)
            print(f"[INFO] Saved to: {event_out} ({len(df)} rows)")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python extract_club_events.py <input_xlsx> <output_dir>")
        sys.exit(1)

    xlsx_path = sys.argv[1]
    output_dir = sys.argv[2]
    extract_club_events(xlsx_path, output_dir)
