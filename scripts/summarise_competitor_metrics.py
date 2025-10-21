import sqlite3
import pandas as pd
import argparse

def summarise_metrics(df):
    print(f"✅ Total competitors processed: {len(df)}")

    latest = df.sort_values('LastUpdatedUtc').drop_duplicates('ClubNumber', keep='last')
    print(f"📦 Unique competitors (latest per ClubNumber): {len(latest)}")

    for band in ['IsJuvenile', 'IsJunior', 'IsSenior', 'IsVeteran']:
        f = latest[(latest[band] == 1) & (latest['IsFemale'] == 1)]
        m = latest[(latest[band] == 1) & (latest['IsFemale'] == 0)]
        print(f"📊 {band}: {len(f)}F / {len(m)}M / {len(f)+len(m)}T")

    missing = latest[
        (latest['IsJuvenile'] == 0) &
        (latest['IsJunior'] == 0) &
        (latest['IsSenior'] == 0) &
        (latest['IsVeteran'] == 0)
    ]
    print(f"⚠️ Competitors missing age band: {len(missing)}")
    if not missing.empty:
        print(missing[['ClubNumber', 'GivenName', 'Surname']].to_string(index=False))

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--db', required=True, help='Path to competitor SQLite DB')
    args = parser.parse_args()

    conn = sqlite3.connect(args.db)
    df = pd.read_sql_query("SELECT * FROM Competitors", conn)
    summarise_metrics(df)

if __name__ == "__main__":
    main()
