# Round Robin README

The `ClubEvents_YYYY.xlsx` files include details of round robin riders from clubs other than WVCC.

![ClubEvents_YYYY.xlsx RoundRobinRiders tab.png](../images/ClubEvents_YYYY-RoundRobinRiders-tab.png)

The Round Robin competitions are processed alongside the WVCC competitions by `ClubProcessor`.

## ClubProcessor enhancements

The initial implementation of ClubProcessor, in readiness for 2026 season, operated in two modes:

--mode competitors
--mode events

### Club Processor mode: competitors
In `--mode competitors`, the processor reads `competitors_YYYY.csv` and populates the `club_competitors_YYYY.db` SQLite database.