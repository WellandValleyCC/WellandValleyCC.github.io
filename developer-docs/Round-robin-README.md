# Round Robin README

The Round Robin competitions are not WVCC specific, but for reasons of practicality, it makes sense to process them alongside the WVCC competitions.
A key design goal, however, is to not pollute the WVCC specific implementation with `ClubProcessor`, so that other clubs wishing to adopt this cloud-based competition system can do so.

## ClubProcessor before Round Robin support

The initial implementation of ClubProcessor, as developed in readiness for 2026 season, operates in two modes:

- --mode competitors
- --mode events

### Mode: competitors
In `--mode competitors`, the processor reads `competitors_YYYY.csv` and populates the `club_competitors_YYYY.db` SQLite database - specifically the `Competitors` table.

<!--
![Competitors table](../images/Competitors-table.png)
<img src="../images/Competitors-table.png" alt="Competitors table" style="width:50%;">
-->

<img src="../images/Competitors-table.png" alt="Competitors table" width="300">

### Mode: events
In `--mode events`, the processor reads `ClubEvents_YYYY.xlsx` and populates the `club_events_YYYY.db` SQLite database - specifically the `CalendarEvents` and `Rides` tables.


<img src="../images/CalendarEvents-table.png" alt="Calendar Events table" width="300" style="vertical-align: top;">
<img src="../images/Rides-table.png" alt="Rides table" width="400" style="vertical-align: top;">

## Round Robin enhancements

To support round robin competitors, the following enhancements were made:

### ClubEvents_YYYY.xlsx enhancements

#### RoundRobinRiders sheet - isFemale column 

The `ClubEvents_YYYY.xlsx` files already included details of round robin riders from clubs other than WVCC.  This was used only within the .xlsx workbook to improve the names presented for members of other clubs appearing as guest riders in the event pages.  This has been extended to include an `isFemale` boolean column since the RR competitions include a female-only competition and the interclub competition also takes the score of the fastest female from each club at an event.

![ClubEvents_YYYY.xlsx RoundRobinRiders sheet.png](../images/ClubEvents_YYYY-RoundRobinRiders-sheet.png)

#### Calendar sheet - RoundRobinEvent and RoundRobinClub columns

![ClubEvents_YYYY.xlsx Calendar sheet.png](../images/ClubEvents_YYYY-Calendar-sheet.png)

This sheet has been extended with two new columns:

- `Round Robin Event` - string (`Y` or blank) - indicates whether this event is a round robin event.
- `Round Robin Club` - string - indicates the club organising the event (e.g. `Ratae`, `LFCC`, `WVCC`, etc.)

![ClubEvents_YYYY.xlsx Calendar sheet updated.png](../images/ClubEvents_YYYY-Calendar-sheet-updated.png)

### Github pipeline script enhanced 

The Python script `scripts\extract_club_events.py` now extracts the RoundRobinRiders sheet to a CSV file named `RoundRobinRiders_{YYYY}.csv`

``` csv
Name,Club,Decorated Name,isFemale
John Doe,Ratae,John Doe (Ratae),False
Jane Doe,LFCC,Jane Doe (LFCC),True
```

### `--mode events` enhanced

#### New RoundRobinRiders table

This table will be populated from the `RoundRobinRiders_{YYYY}.csv` file together with *all* WVCC competitors from Competitors table, with `Club` set to `WVCC` and `IsFemale` set according to the `IsFemale` column in the Competitors table. No other `Competitor` metadata (is required )such as ClaimStatus, AgeGroup, ClubNumber, etc.) is required.

