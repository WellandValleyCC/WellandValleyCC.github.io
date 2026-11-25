import pandas as pd
import sys
import os
import re
import hashlib
import shutil
from openpyxl import load_workbook
import warnings

def sha256(path):
    h = hashlib.sha256()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(8192), b""):
            h.update(chunk)
    return h.hexdigest()

def normalize_xlsx(src_path):
    normalized = src_path + ".normalized.xlsx"
    wb = load_workbook(src_path, data_only=True)
    wb.save(normalized)
    return normalized

def suppress_openpyxl_header_footer_warnings():
    warnings.filterwarnings(
        "ignore",
        category=UserWarning,
        module="openpyxl.worksheet.header_footer"
    )
    print("[INFO] Suppressed openpyxl header/footer warnings (safe to ignore, no impact on CSV extraction)")
    
def normalize_boolean_column(df, colname="isFemale"):
    if colname not in df.columns:
        return

    # Convert everything to string for consistent handling
    s = df[colname].astype(str).str.strip().str.upper()

    # Try numeric conversion (handles 0, 1, 0.0, 1.0)
    numeric = pd.to_numeric(s, errors="coerce")

    # Case A: numeric values → boolean
    numeric_mask = numeric.notna()
    numeric_bool = (numeric != 0)

    # Case B: TRUE/FALSE strings
    string_bool = s.map({"TRUE": True, "FALSE": False})

    # Combine both into a single boolean array
    combined = numeric_bool.where(numeric_mask, string_bool)

    # Final output as "True"/"False"
    df[colname] = combined.map({True: "True", False: "False"})
    
def extract_club_events(xlsx_path, output_dir):
    match = re.search(r'ClubEvents_(\d{4})\.xlsx$', os.path.basename(xlsx_path))
    if not match:
        print("[ERROR] Filename must match pattern: ClubEvents_YYYY.xlsx")
        sys.exit(1)
    year = match.group(1)

    print(f"[INFO] Input XLSX path: {xlsx_path}")
    print(f"[INFO] Input XLSX sha256: {sha256(xlsx_path)}")

    suppress_openpyxl_header_footer_warnings()
    
    try:
        normalized_path = normalize_xlsx(xlsx_path)
        print(f"[INFO] Normalized XLSX saved: {normalized_path}")
    except Exception as e:
        print(f"[WARN] Normalization failed, continuing with original file: {e}")
        normalized_path = xlsx_path

    try:
        xl = pd.ExcelFile(normalized_path)
    except Exception as e:
        print(f"[ERROR] Failed to open workbook: {e}")
        sys.exit(1)

    year_dir = os.path.join(output_dir, year)
    os.makedirs(year_dir, exist_ok=True)

    events_dir = os.path.join(year_dir, "events")
    os.makedirs(events_dir, exist_ok=True)

    artifact_dir = os.path.join("artifacts", "extracted", year)
    os.makedirs(artifact_dir, exist_ok=True)

    calendar_df = None

    if "Calendar" in xl.sheet_names:
        print("[OK] Extracting Calendar sheet")
        calendar_df = xl.parse("Calendar", dtype={"Event Number": str})
        calendar_out = os.path.join(year_dir, f"Calendar_{year}.csv")
        calendar_df.to_csv(calendar_out, index=False)
        shutil.copy(calendar_out, os.path.join(artifact_dir, os.path.basename(calendar_out)))
        print(f"[INFO] Saved to: {calendar_out} ({len(calendar_df)} rows)")
    else:
        print("[WARN] Sheet missing: Calendar")

    if "Competitors" in xl.sheet_names:
        print("[INFO] Extracting Competitors sheet (reference only)")
        competitors_df = xl.parse("Competitors")
        normalize_boolean_column(competitors_df, "isFemale")
        competitors_out = os.path.join(year_dir, f"Competitors_{year}.csv")
        competitors_df.to_csv(competitors_out, index=False)
        shutil.copy(competitors_out, os.path.join(artifact_dir, os.path.basename(competitors_out)))
        print(f"[INFO] Saved to: {competitors_out} ({len(competitors_df)} rows)")
    else:
        print("[WARN] Sheet missing: Competitors")
        
    if "RoundRobinRiders" in xl.sheet_names:
        print("[OK] Extracting RoundRobinRiders sheet")
        round_robin_riders_df = xl.parse("RoundRobinRiders")
        round_robin_riders_out = os.path.join(year_dir, f"RoundRobinRiders_{year}.csv")
        round_robin_riders_df.to_csv(round_robin_riders_out, index=False)
        shutil.copy(round_robin_riders_out, os.path.join(artifact_dir, os.path.basename(round_robin_riders_out)))
        print(f"[INFO] Saved to: {round_robin_riders_out} ({len(round_robin_riders_df)} rows)")
    else:
        print("[WARN] Sheet missing: RoundRobinRiders")
        
    if "Leagues" in xl.sheet_names:
        print("[OK] Extracting Leagues sheet")
        leagues_df = xl.parse("Leagues")
        leagues_out = os.path.join(year_dir, f"Leagues_{year}.csv")
        leagues_df.to_csv(leagues_out, index=False)
        shutil.copy(leagues_out, os.path.join(artifact_dir, os.path.basename(leagues_out)))
        print(f"[INFO] Saved to: {leagues_out} ({len(leagues_df)} rows)")
    else:
        print("[WARN] Sheet missing: Leagues")

    # Event extraction driven by Calendar sheet
    if calendar_df is None:
        print("[WARN] Cannot extract events because Calendar sheet is missing")
    else:
        required_columns = ["Event Number", "Sheet Name"]

        missing_columns = [
            col for col in required_columns
            if col not in calendar_df.columns
        ]

        if missing_columns:
            print(
                f"[ERROR] Calendar sheet missing required columns: {missing_columns}"
            )
        else:
            manifest_entries = []
            seen_event_numbers = set()

            for _, row in calendar_df.iterrows():

                event_number_value = row["Event Number"]

                sheet_name = str(row["Sheet Name"]).strip()

                if pd.isna(event_number_value):
                    continue

                try:
                    event_num = int(event_number_value)
                except (TypeError, ValueError):
                    print(
                        f"[WARN] Invalid Event Number '{event_number_value}'"
                    )
                    continue

                if event_num in seen_event_numbers:
                    print(
                        f"[ERROR] Duplicate EventNumber detected in Calendar: {event_num}"
                    )
                    continue

                seen_event_numbers.add(event_num)

                if not sheet_name:
                    print(
                        f"[WARN] Event {event_num:02} has no Sheet Name"
                    )
                    continue

                if sheet_name not in xl.sheet_names:
                    print(
                        f"[ERROR] Event {event_num:02} references missing worksheet '{sheet_name}'"
                    )
                    continue

                print(
                    f"[OK] Extracting Event {event_num:02} from worksheet '{sheet_name}'"
                )

                df = xl.parse(sheet_name)

                # Trim at first blank row in column A
                first_blank_index = (
                    df[
                        df.iloc[:, 0].isnull()
                        | (df.iloc[:, 0].astype(str).str.strip() == "")
                    ]
                    .index
                    .min()
                )

                if pd.notnull(first_blank_index):
                    df = df.iloc[:first_blank_index]

                # Normalize string columns
                for c in df.select_dtypes(include=["object"]).columns:
                    df[c] = (
                        df[c]
                        .astype(str)
                        .str.strip()
                        .replace({"nan": ""})
                    )

                headers = list(df.columns)

                missing_name_count = (
                    df["Name"].isnull().sum()
                    if "Name" in df.columns else "N/A"
                )

                print(
                    f"[INFO] Worksheet {sheet_name} headers: {headers}; "
                    f"rows: {len(df)}; "
                    f"missing Names: {missing_name_count}"
                )

                print(
                    f"[INFO] Preview of Event_{event_num:02}.csv:"
                )

                print(df.head(4).to_string(index=False))

                event_filename = f"{sheet_name}.csv"

                event_out = os.path.join(
                    events_dir,
                    event_filename
                )

                df.to_csv(event_out, index=False)

                shutil.copy(
                    event_out,
                    os.path.join(
                        artifact_dir,
                        os.path.basename(event_out)
                    )
                )

                print(
                    f"[INFO] Saved to: {event_out} ({len(df)} rows)"
                )

                manifest_entries.append(
                    (
                        event_num,
                        sheet_name,
                        f"events/{event_filename}"
                    )
                )

            manifest_path = os.path.join(
                year_dir,
                f"EventManifest_{year}.csv"
            )

            with open(manifest_path, "w") as f:
                f.write("EventNumber,SheetName,Filename\n")

                for event_num, sheet_name, filename in sorted(
                    manifest_entries,
                    key=lambda x: x[0]
                ):
                    f.write(
                        f"{event_num:02},{sheet_name},{filename}\n"
                    )

            print(f"[INFO] Manifest saved: {manifest_path}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python extract_club_events.py <input_xlsx> <output_dir>")
        sys.exit(1)

    xlsx_path = sys.argv[1]
    output_dir = sys.argv[2]
    extract_club_events(xlsx_path, output_dir)
