import sqlite3
import pandas as pd
import argparse

def summarise_metrics(df):
    print(f"✅ Total competitors processed: {len(df)}")

    latest = df.sort_values('LastUpdatedUtc').drop_duplicates('ClubNumber', keep='last')
    print(f"📦 Unique competitors (latest per ClubNumber): {len(latest)}")

    age_groups = ['Juvenile', 'Junior', 'Senior', 'Veteran']
    for group in age_groups:
        f = latest[(latest['AgeGroup'] == group) & (latest['IsFemale'] == 1)]
        m = latest[(latest['AgeGroup'] == group) & (latest['IsFemale'] == 0)]
        print(f"📊 {group}: {len(f)}F / {len(m)}M / {len(f)+len(m)}T")

    missing = latest[~latest['AgeGroup'].isin(age_groups)]
    print(f"⚠️ Competitors missing age group: {len(missing)}")
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
