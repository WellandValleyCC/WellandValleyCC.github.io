# Nev Brooks Competition

This is a handicap competition based on the 10-mile TTs in the season.
All FirstClaim club members are eligible - any age group.
The intent is to reward improvement using a handicapping system

## How it works.
- A Nev Brooks handicap time is established for each 10-mile TT that you ride.  
	- This handicap time is the difference between the ride time and the world record time (16:35 = 995 seconds).
	- E.g. if you ride a 24:35 (1475 seconds), then your calculated handicap time will be 480 seconds
	- E.g. if you ride a 26:35 (1595 seconds), then your calculated handicap time will be 600 seconds
- The handicap time that you set at your first 10-mile event is used as your handicap in the next 10-mile TT which you ride
- At each subsequent 10-mile TT, a new handicap time is calculated, always calculated as *your-time minus 995 seconds*.
- The scoring system maintains the following information for each 10-mile event
  - Nev Brooks handicap seconds generated (i.e. total seconds - 995)
  - Nev Brooks handicap applied 
    - this is the smallest Nev Brooks handicap that you have achieved so far this season
    - not applicable for the first 10-mile event that you ride since you do not yet have any handicap
  - Nev Brooks adjusted time (i.e. total seconds - your lowest Nev Brooks handicap so far this season)

These Nev Brooks adjusted times are what is used to rank and score the riders at each 10-mile TT event.
- Scoring is the same as in other competitionts: 60, 55, 51, etc.
	

### Why 16:35?
16:35 was chosen since it is the world record for a 10-mile TT.  When the Nev Brooks competition was introduced in 2022, the old Excel workbook which calculated the results was set to use 21:00 as the Nev Brooks Standard Time, but a club member beat that time by 13 seconds and the old Excel workbook could not handle a negative handicap.

It really makes no difference what value is used since if all riders repeated their same times every race, the handicap system would make all events a draw, with every rider achieving an adjusted time of 16:35.

TotalSecondsFirstEvent - NevBrooksStandardTimeSeconds = HandicapForSecondEvent
TotalSecondsSecondEvent - HandicapForSecondEvent = NevBrooksAdjustedTime

1475 - 995 = 480
1475 - 480 = 995

1595 - 995 = 600
1595 - 600 = 995