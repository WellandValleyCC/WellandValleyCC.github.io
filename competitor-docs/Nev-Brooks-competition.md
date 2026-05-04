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
	

### Why 16:35?
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
