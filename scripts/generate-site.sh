#!/usr/bin/env bash
set -euo pipefail

# Usage: ./scripts/generate-site.sh [YEAR]
# If YEAR is supplied, use it. Otherwise detect latest year from club_events_*.db

YEAR=${1:-}

if [ -z "$YEAR" ]; then
  echo "[INFO] No year supplied, detecting from club_events DBs..."
  FILE=$(ls data/club_events_*.db | sort | tail -n1)
  YEAR=$(echo "$FILE" | sed -E 's/.*club_events_([0-9]{4})\.db/\1/')
fi

echo "[INFO] Generating site for year $YEAR"

dotnet run --project src/ClubSiteGenerator \
  --events data/club_events_${YEAR}.db \
  --competitors data/club_competitors_${YEAR}.db \
  --output docs/
