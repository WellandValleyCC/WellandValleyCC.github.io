# Event Processor Guide
   
This document explains how to prepare event data, run the processor locally, and debug the pipeline.

---

## Step 1: Extract Event CSVs from XLSX

To split `ClubEvents_2026.xlsx` into per-sheet CSVs:

```bash
python scripts/extract_club_events.py data/ClubEvents_2026.xlsx data/extracted/events/
```

This will produce:
- `calendar_2026.csv` — event metadata
- `competitors_2026.csv` — reference sheet
- `Event_01.csv` to `Event_26.csv` — per-event results

---

## Step 2: Run the Processor via CLI

To ingest the extracted CSVs and populate the event database:

```bash
dotnet run --project processor/ClubProcessor/ClubProcessor.csproj --mode events --file data\extracted\2025
```

This will:
- Parse each `Event_*.csv`
- Normalize and validate data
- Write to `club_events_2026.db` (or configured target)
- Emit diagnostics and metrics to console

---

## Step 3: Debug in Visual Studio

To run the processor in debug mode:

1. **Set the Startup Project**
   - In Solution Explorer, right-click `ClubProcessor.csproj`
   - Select **Set as Startup Project**

2. **Configure Debug Arguments**
   - Right-click `ClubProcessor` → **Properties**
   - Go to the **Debug** tab
   - Set **Application arguments** to:
     ```
     --mode events --folder ../../data/extracted/events/
     ```
   - Adjust the path if your `.csproj` is located elsewhere

3. **Set Breakpoints**
   - Open the method that handles `--mode events`
   - Add breakpoints in:
     - CSV parsing logic
     - Event object creation
     - SQLite write operations

4. **Run the Processor**
   - Press **F5** to launch in debug mode
   - Inspect how each CSV is processed and written to the event database

# Notes on ClubEvents_2026.XLSX

## Conditional formatting on Event_nn sheets

Configured using this macro.  Not saved as part of the sheet as it's not .xlsm

```
Sub ApplyStandardEventFormattingWithAbsoluteRefs()
    Dim ws As Worksheet
    Dim i As Integer
    Dim fmt As FormatCondition

    For i = 1 To 26
        On Error Resume Next
        Set ws = ThisWorkbook.Sheets("Event_" & Format(i, "00"))
        On Error GoTo 0

        If Not ws Is Nothing Then
            ' Clear all existing conditional formatting
            ws.Cells.FormatConditions.Delete

            ' Rule 1: Format $D$2:$D$101 with number format (2 decimal places)
            With ws.Range("$D$2:$D$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=TEXT($B$104,""@"")=""Y""")
                fmt.NumberFormat = "0.00"
                fmt.StopIfTrue = False
            End With
            
            ' Rule 2: Format $H$2:$H$101 with number format (2 decimal places)
            With ws.Range("$H$2:$H$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=TEXT($B$104,""@"")=""Y""")
                fmt.NumberFormat = "hh:mm:ss.00;@"
                fmt.StopIfTrue = False
            End With

            ' Rule 3: Fill $G$2:$H$101 with cyan when column I = "X"
            With ws.Range("$G$2:$H$101")
                Set fmt = .FormatConditions.Add(Type:=xlExpression, Formula1:="=$I2=""X""")
                fmt.Interior.Color = RGB(179, 235, 255)
                fmt.StopIfTrue = False
            End With

        End If
    Next i

    MsgBox "Formatting applied with absolute references to Event_01 through Event_26.", vbInformation
End Sub
```

## Contributor Notes

- Always run extraction before processing events.

- Keep club_events_YYYY.db under version control for reproducibility.

- Use debug mode for investigating parsing or DB write issues.

- Update this guide when processor arguments or file locations change.
