# Nev Brooks Competition

This is a handicap competition based on the 10‑mile time trials in the season.
All First Claim club members are eligible, regardless of age. The aim is to reward improvement by using a simple handicapping system.

## How it works

- Every time you ride a 10‑mile TT, a **Nev Brooks handicap time** is calculated.  
  - This is your ride time minus the Nev Brooks Standard Time (16:35 = 995 seconds).  
  - Example: 24:35 (1475 seconds) → handicap = 1475 − 995 = 480 seconds  
  - Example: 26:35 (1595 seconds) → handicap = 1595 − 995 = 600 seconds

- Your **first** 10‑mile TT of the season sets your initial handicap.  
  This becomes the handicap applied to your **next** 10‑mile TT.

- At each subsequent 10‑mile TT:
  - A new handicap time is calculated (again: your time − 995).  
  - Your **lowest handicap so far this season** becomes your active handicap.  
  - Your **Nev Brooks adjusted time** is then:
    - total seconds − your lowest handicap so far.

These adjusted times are used to rank riders at each 10‑mile TT.  
Scoring follows the standard club points system (60, 55, 51, etc.).
	

## Worked example – Milly Pinnock's 2025 Nev Brooks season

To show how the Nev Brooks handicap evolves through a season, here is a real
example showing Milly's 2025 10‑mile TT results. This illustrates:

- how the first ride sets the initial handicap,
- how the lowest handicap so far becomes the one applied at each event,
- how the adjusted time is calculated at every race,
- and how improvements (or slower days) affect the standings.

The table below shows each of Milly’s 2025 10‑mile TTs, along with the
handicap generated, the lowest handicap so far, and the adjusted time used
for Nev Brooks scoring.

| Event | Time (mm:ss) | Total Seconds | Handicap Generated | Lowest Handicap So Far | Adjusted Time | Points | Notes |
|-------|--------------|---------------|--------------------|------------------------|---------------|--------|-------|
| 5  | 28:42 | 1722 | 727 | n/a | n/a | n/a | First 10TT ridden — sets initial handicap |
| 7  | -     | n/a  | n/a | n/a | n/a | n/a | Event cancelled |
| 8  | -     | 0    | n/a | n/a | n/a | n/a | Milly did not ride |
| 10 | 26:39 | 1599 | 604 | 727 | 872 | 48 | First event where handicap is applied (727) |
| 12 | 25:59 | 1559 | 564 | 604 | 955 | 48 | New lowest handicap (604 → 564) |
| 13 | 26:15 | 1575 | 580 | 564 | 1011 | 38 | Slower ride — lowest handicap remains 564 |
| 14 | 25:57 | 1557 | 562 | 564 | 993 | 37.5 | New lowest handicap (564 → 562) |
| 15 | 28:18 | 1698 | 703 | 562 | 1136 | 49.5 | Slower ride — lowest handicap remains 562 |
| 16 | 27:09 | 1629 | 634 | 562 | 1067 | 44 | Slower ride — lowest handicap remains 562 |
| 17 | 28:23 | 1703 | 708 | 562 | 1141 | 39 | Slower ride — lowest handicap remains 562 |
| 18 | 27:38 | 1658 | 663 | 562 | 1096 | 37 | Slower ride — lowest handicap remains 562 |
| 19 | 26:37 | 1597 | 602 | 562 | 1035 | 32.5 | Slower ride — lowest handicap remains 562 |
| 20 | -     | n/a  | n/a | n/a | n/a | n/a | Event not ridden |
| 21 | 26:26 | 1586 | 591 | 562 | 1024 | 39.5 | Slower ride — lowest handicap remains 562 |
| 22 | -     | n/a  | n/a | n/a | n/a | n/a | Event not ridden |
| 24 | 26:43 | 1603 | 608 | 562 | 1041 | 55 | Slower ride — lowest handicap remains 562 |
| 25 | -     | n/a  | n/a | n/a | n/a | n/a | Event cancelled |
| 26 | -     | n/a  | n/a | n/a | n/a | n/a | Event cancelled |

## Why is the Nev Brooks Standard Time set at 16:35?
16:35 was chosen since it is the world record for a 10-mile TT and is therefore unlikely to be beaten on any Welland Valley course.  When the Nev Brooks competition was introduced in 2022, the Excel workbook which calculated the results was set to use 21:00 as the Nev Brooks Standard Time, but a club member beat that time by 13 seconds and the old Excel workbook could not handle a negative handicap.

It really makes no difference what value is used since if all riders repeated their same times every race, the handicap system would make all events a draw, with every rider achieving an adjusted time of 16:35 (or whatever the Nev Brooks Standard Time is).

```
    TotalSecondsFirstEvent  - NevBrooksStandardTimeSeconds 
    = HandicapForNextEvent

    TotalSecondsNextEvent   - HandicapForNextEvent         
    = NevBrooksAdjustedTime
```

#### Example 1 - Rider consistently achieving 24:35

```
Handicap for next event:
  1475 (24:35) - 995 = 480

Adjusted time for next event:
  1475 - 480 = 995
```

#### Example 2 - Rider consistently achieving 26:35

```
Handicap for next event:
  1595 (26:35) - 995 = 600

Adjusted time for next event:
  1595 - 600 = 995
```
