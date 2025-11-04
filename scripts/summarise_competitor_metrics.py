import sqlite3
import pandas as pd
import argparse

def summarise_metrics(df):
    print(f"✅ Total competitors processed: {len(df)}")

    latest = df.sort_values('LastUpdatedUtc').drop_duplicates('ClubNumber', keep='last')
    print(f"📦 Unique competitors (latest per ClubNumber): {len(latest)}")

    # Map AgeGroup enum integers to string labels
    age_group_map = {
        1: 'Juvenile',
        2: 'Junior',
        3: 'Senior',
        4: 'Veteran'
    }
    latest['AgeGroupName'] = latest['AgeGroup'].map(age_group_map)

    for group in ['Juvenile', 'Junior', 'Senior', 'Veteran']:
        f = latest[(latest['AgeGroupName'] == group) & (latest['IsFemale'] == 1)]
        m = latest[(latest['AgeGroupName'] == group) & (latest['IsFemale'] == 0)]
        print(f"📊 {group}: {len(f)}F / {len(m)}M / {len(f)+len(m)}T")

    missing = latest[~latest['AgeGroup'].isin(age_group_map.keys())]
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
