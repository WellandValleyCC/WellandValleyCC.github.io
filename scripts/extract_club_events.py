import pandas as pd
import sys
import os

def extract_sheets(xlsx_path, output_dir):
    expected_sheets = {
        "Events": "events_2025.csv",
        "Calendar": "calendar_2025.csv",
        "Metadata": "metadata.csv"
    }

    print(f"📘 Reading workbook: {xlsx_path}")
    try:
        xl = pd.ExcelFile(xlsx_path)
    except Exception as e:
        print(f"❌ Failed to open workbook: {e}")
        sys.exit(1)

    os.makedirs(output_dir, exist_ok=True)

    for sheet_name, output_file in expected_sheets.items():
        if sheet_name in xl.sheet_names:
            print(f"✅ Extracting sheet: {sheet_name}")
            df = xl.parse(sheet_name)
            output_path = os.path.join(output_dir, output_file)
            df.to_csv(output_path, index=False)
            print(f"📁 Saved to: {output_path} ({len(df)} rows)")
        else:
            print(f"⚠️ Sheet missing: {sheet_name}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python extract_club_events.py <input_xlsx> <output_dir>")
        sys.exit(1)

    xlsx_path = sys.argv[1]
    output_dir = sys.argv[2]
    extract_sheets(xlsx_path, output_dir)
