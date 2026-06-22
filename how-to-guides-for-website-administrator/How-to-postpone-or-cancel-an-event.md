# How to Postpone or Cancel an Event

This guide explains how to postpone or cancel a Time Trial (TT) event in the Welland Valley CC website administration system.  
It also explains how postponed and cancelled events appear on the website and how they affect competition scoring.

---

## When to Postpone vs Cancel

### Postpone an event when:
- The event will not run on its original date.
- A replacement event **will** be scheduled later.
- The event should **remain visible** in the calendar.
- The event should **still appear as a column** in competition tables.
- The event should **still count** towards the N/2 + 1 scoring calculation.

### Cancel an event when:
- The event will not take place.
- There will be **no replacement event**.
- The event should **remain visible** in the calendar (for transparency).
- The event should **still appear as a column** in competition tables.
- The event should **not** count towards the N/2 + 1 scoring calculation.

---

## How to Postpone an Event

1. Open the event in the website admin panel.
2. Set the **IsPostponed** flag to `true`.
3. Ensure **IsCancelled** remains `false`.
4. Save the event.

### What happens next
- The event remains visible in the calendar with a **Postponed** banner.
- It still appears as a column in competition tables.
- It still counts towards the N/2 + 1 scoring formula.
- No rides will be created for the postponed event.
- The event remains active until a replacement event is created.

---

## How to Add a Replacement Event

When you know the new date:

1. Create a **new event** in the calendar for the replacement date.
2. Return to the original postponed event.
3. Change:
   - **IsPostponed** → `false`
   - **IsCancelled** → `true`
4. Save the event.

### What happens next
- The original event is now marked as **Cancelled**.
- It remains visible in the calendar and competition tables.
- It no longer counts towards N/2 + 1.
- The replacement event becomes the active event for scoring and results.

---

## How to Cancel an Event

1. Open the event in the admin panel.
2. Set **IsCancelled** to `true`.
3. Ensure **IsPostponed** is `false`.
4. Save the event.

### What happens next
- The event appears in the calendar with a **Cancelled** banner.
- It still appears as a column in competition tables.
- It is **excluded** from the N/2 + 1 scoring calculation.
- No rides will be created for the event.

---

## How Events Appear on the Website

### Calendar
- **Normal events**: shown normally.
- **Postponed events**: shown with a “Postponed” banner.
- **Cancelled events**: shown with a “Cancelled” banner.

### Competition Tables
All events appear as columns:
- Normal events: show rides and points.
- Postponed events: show an empty column until the replacement event is run.
- Cancelled events: show an empty column permanently.

This ensures the table structure remains consistent for all riders.

---

## Summary Table

| State       | Shown in Calendar | Shown in Competition Tables | Counts for N/2+1 | Has Rides | Notes |
|-------------|-------------------|------------------------------|------------------|-----------|-------|
| Normal      | Yes               | Yes                          | Yes              | Yes       | Standard event |
| Postponed   | Yes               | Yes                          | Yes              | No        | Will be replaced |
| Cancelled   | Yes               | Yes                          | No               | No        | Never run |

---

## Best Practices

- **Postpone first** if you expect a replacement event.
- **Cancel only** when the event will not run at all.
- After adding a replacement event, **remember to cancel the original postponed event**.
- Avoid setting both *IsPostponed* and *IsCancelled* at the same time — this is an invalid state.

