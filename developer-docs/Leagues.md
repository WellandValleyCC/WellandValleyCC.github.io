# League Competitions Guide

## Overview
- 5 divisions: Premier, 1, 2, 3, 4
- Membership assigned mid‑season via `Leagues` sheet in ClubEvents_YYYY.xlsx`
- python extractor extracts this sheet to`Leagues_YYYY.csv`
- All events produce league scores, even before allocation

## Data Model
- Competitors table: holds League assignment (single source of truth)
- Ride table: holds LeaguePosition + LeaguePoints outputs

## Workflow
1. Extract `Leagues_YYYY.csv` alongside other sheets
2. Refresh competitor league assignments during `--mode events`
3. Run `LeaguesScoreCalculator` to populate Ride outputs

###  Workflow details

Inputs
 ├─ Competitors_YYYY.csv   (baseline competitor metadata: ID, Name, AgeGroup, Gender, etc.)
 ├─ Leagues_YYYY.csv       (authoritative league membership: CompetitorID → League)
 └─ Event_X.csv            (rides from each event, extracted from ClubEvents_YYYY.xlsx)

Processing (--mode events)
 ├─ Step 1: Load Competitors
 │    • Read Competitors_YYYY.csv
 │    • Build competitor registry
 │
 ├─ Step 2: Refresh League Membership
 │    • Read Leagues_YYYY.csv
 │    • Update Competitors table with League assignment
 │    • Remove League assignment for competitors not present in Leagues file
 │    • Ensures Competitors table is always current
 │
 ├─ Step 3: Process Events
 │    • For each Event_X.csv:
 │        – Join rides with competitor registry (now includes League)
 │        – Pass rides into calculators
 │
 ├─ Step 4: LeaguesScoreCalculator
 │    • Iterate over all 5 leagues (Prem, 1, 2, 3, 4)
 │    • For each event:
 │        – Filter rides by league membership
 │        – Rank competitors within league
 │        – Assign LeaguePosition and LeaguePoints
 │        – Persist into Ride table
 │
 └─ Step 5: Persist Outputs
      • Ride table now contains:
          – Existing scoring outputs (Juveniles, Juniors, Seniors, Veterans, RoadBike, Women, etc.)
          – LeaguePosition, LeaguePoints
      • Competitors table contains:
          – Current League assignment (single source of truth)

### Key Principles
- Competitors table holds membership (League column).

- Ride table holds outputs (LeaguePosition, LeaguePoints).

- Leagues_YYYY.csv is the authoritative source, processed each time you run --mode events.

- If TT committee reshuffles divisions mid‑season, you just re‑process events with the updated Leagues file — Competitors table is refreshed, Ride outputs are recalculated.

## Calculator
- Iterates over all events
- For each league:
  - Filter rides by membership
  - Rank competitors
  - Assign LeaguePosition + LeaguePoints

## Notes for Contributors
- Membership changes mid‑season → reprocess events
- Removed competitors → cleared from Competitors table
- League outputs always recalculated fresh
