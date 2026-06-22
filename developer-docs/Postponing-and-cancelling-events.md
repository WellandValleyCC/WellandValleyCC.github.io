# TT Event Status Behaviour Rules

## Overview
Time Trial (TT) events can exist in three logical states: **Normal**, **Postponed**, or **Cancelled**. These states determine how events appear in the calendar, how they contribute to competition scoring, and how replacement events are handled.

## Event States

### Normal Event
- The event is scheduled and expected to run.
- Included in all scoring calculations.
- Appears normally in the calendar and competition tables.

### Postponed Event
- The event will not run on its original date but is still intended to take place.
- **Included** in the total number of events used for the *N/2 + 1* scoring formula.
- Appears in the calendar with postponed styling.
- Does not yet have any rides or results.
- Remains part of the calendar until a replacement event is created.

### Cancelled Event
- The event will not take place and will not be replaced.
- **Excluded** from the total number of events used for the *N/2 + 1* scoring formula.
- Should not appear in competition scoring logic.
- Still appears visually in the calendar with cancelled styling.

## Replacement Events

When a replacement event is added to the calendar:

- The original postponed event is automatically converted to **Cancelled**.
- The replacement event becomes the active event.
- Scoring logic uses the replacement event instead of the original.

## Scoring Logic

### Counting Events for N/2 + 1
- Only events that are **not cancelled** contribute to the total event count.
- Formula:  
  **EventsToScore = (ActiveEvents / 2) + 1**

Where **ActiveEvents** includes:
- Normal events  
- Postponed events  

And excludes:
- Cancelled events

## Rendering Rules

### Calendar
- Normal events: standard appearance.
- Postponed events: distinct postponed styling.
- Cancelled events: cancelled styling (e.g., strike‑through).

### Competition Tables
- Normal and postponed events appear as event columns.
- Cancelled events do **not** appear in the table.
- Scoring uses only active events.

## Ride Creation
- Normal events: rides created as usual.
- Postponed events: no rides created until the replacement event exists.
- Cancelled events: no rides created.
