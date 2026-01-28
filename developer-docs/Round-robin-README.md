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
![Competitors table](/images/Competitors-Table.png)
<img src="../images/Competitors-Table.png" alt="Competitors table" style="width:50%;">
-->

<img src="../images/Competitors-Table.png" alt="Competitors table" width="300">

### Mode: events
In `--mode events`, the processor reads `ClubEvents_YYYY.xlsx` and populates the `club_events_YYYY.db` SQLite database - specifically the `CalendarEvents` and `Rides` tables.

<table>
  <tr>
    <td valign="top">
      <img src="../images/CalendarEvents-Table.png" width="300">
    </td>
    <td valign="top">
      <img src="../images/Rides-Table.png" width="400">
    </td>
  </tr>
</table>

## Round Robin enhancements 

To support round robin competitors, the following enhancements were made:

- [x] ClubEvents_YYYY.xlsx enhancements
- [x] Github pipeline script enhanced
- [ ] 

### ClubEvents_YYYY.xlsx enhancements

- [x] RoundRobinRiders sheet added
- [x] Calendar sheet enhanced with RoundRobinEvent and RoundRobinClub columns

#### RoundRobinRiders sheet - isFemale column

The `ClubEvents_YYYY.xlsx` files already included details of round robin riders from clubs other than WVCC.  This was used only within the .xlsx workbook to improve the names presented for members of other clubs appearing as guest riders in the event pages.  This has been extended to include an `isFemale` boolean column since the RR competitions include a female-only competition and the interclub competition also takes the score of the fastest female from each club at an event.

![ClubEvents_YYYY.xlsx RoundRobinRiders sheet.png](/images/ClubEvents_YYYY-RoundRobinRiders-sheet.png)

#### Calendar sheet - RoundRobinEvent and RoundRobinClub columns

![ClubEvents_YYYY.xlsx Calendar sheet.png](/images/ClubEvents_YYYY-Calendar-sheet.png)

This sheet has been extended with two new columns:

- `Round Robin Event` - string (`Y` or blank) - indicates whether this event is a round robin event.
- `Round Robin Club` - string - indicates the club organising the event (e.g. `Ratae`, `LFCC`, `WVCC`, etc.)

![ClubEvents_YYYY.xlsx Calendar sheet updated.png](/images/ClubEvents_YYYY-Calendar-sheet-updated.png)

#### Event sheets - new Raw Name column to simplify results entry

These sheets have an additional column `Raw Name`:

![ClubEvents_YYYY.xlsx Event sheets.png](/images/ClubEvents_YYYY-Event-sheets.png)

And column A (`Number/Name`) has been updated to make use of a formula:
```
=IF(J2="","",XLOOKUP(J2,ClubNames,ClubNumbers,XLOOKUP(J2,RoundRobinRiderNames,RoundRobinRiderDecoratedNames,J2)))
```

This formula follows the following algorithm:

1.  Is the named rider a member of WVCC (`Competitors` sheet - populated with data from `Competitors_YYYY.csv`)?  Yes - use their member number. [ `Mike Ives` --> `581`]
2. Is the named rider present in the `RoundRobinRiders` sheet?  Yes - use their `Decorated Name` which includes their club name in parenthesis. [`John Doe` --> `John Doe (Ratae)`]
3. Otherwise - use the name 'as is'.  [`Geraint Thomas` --> `Geraint Thomas`]

> [!TIP]
>
> When entering event results, enter names in column J initially to make use of this lookup algorithm.  
> 
> However, you should overwrite the formula in column `A` with the rider's name if, at the time of the event, the rider is *not* a member of a club (WVCC or other Round Robin club).  This prevents issues if the rider later joins WVCC or one of the other clubs, where the formula would make it appear as though their membership was back-dated.


### Github pipeline script enhanced 

- [x] Python script extracts RoundRobinRiders sheet to CSV

The Python script `scripts\extract_club_events.py` now extracts the RoundRobinRiders sheet to a CSV file named `RoundRobinRiders_{YYYY}.csv`

``` csv
Name,Club,Decorated Name,isFemale
John Doe,Ratae,John Doe (Ratae),False
Jane Doe,LFCC,Jane Doe (LFCC),True
```

### `--mode events` enhanced

- [ ] Additional columns in CalendarEvents table (populated from Calendar sheet)
- [ ] RoundRobinRiders table added to club_events_YYYY.db - new migration
- [ ] RoundRobinRiders table populated from RoundRobinRiders_{YYYY}.csv and Competitors table

#### Enhanced CalendarEvents table

This needs a new migration to add the following columns to the CalendarEvents table:

- IsRoundRobinEvent - boolean
- RoundRobinClub - string

#### New RoundRobinRiders table

- [ ] to do

This table will be populated from the `RoundRobinRiders_{YYYY}.csv` file.  These riders are from clubs other than WVCC, so they will not appear in the `Competitors` table.

> [!NOTE]
> When scoring the RoundRobin competitions, the `RoundRobinRiders` table will be used in conjunction with the `Competitors` table. 

What to do about WVCC riders concept of dates when they sign up?  Do we need this concept for other clubs?  For now, we will assume that all non-WVCC riders are in their club for all RR events.  We could always use a different name in the event pages if we need to.


