# Competition Rules

This folder contains the season-aware scoring rules for club competitions.  
It is the single source of truth for both calculators and renderers.

---

## Config File

Rules are defined in ./config/competition-rules.json.

### Schema

Each year entry contains two groups:

- `tenMile` -> how many ten-mile rides to count (fixed or formula).
- `mixedDistance` -> how many rides to count overall, plus the minimum non-ten rides required.

Example:

```json
{
  "2024": {
    "tenMile": { "count": 6 },
    "mixedDistance": { "count": 11, "nonTenMinimum": 2 }
  },
  "2025": {
    "tenMile": { "count": 8 },
    "mixedDistance": { "formula": "calendarEvents/2+1", "cap": 11, "nonTenMinimum": 2 }
  },
  "2026": {
    "tenMile": { "formula": "calendarEvents/2+1", "cap": 11 },
    "mixedDistance": { "formula": "calendarEvents/2+1", "cap": 11, "nonTenMinimum": 2 }
  }
}

```

- `count` -> fixed number of rides.  
- `formula` -> expression evaluated against the number of calendar events.  
- `cap` -> maximum allowed value.  
- `nonTenMinimum` -> minimum required non-ten rides (applies only to mixedDistance).

---

## Classes

- **CompetitionRules**  
  Immutable object representing resolved rules for a given year.  
  Exposes `TenMileCount`, `MixedEventCount`, `NonTenMinimum`, plus helper titles and narrative text.

- **RuleDefinition**  
  Describes one scoring dimension (fixed or formula).  
  Properties: `Count`, `Formula`, `Cap`, `NonTenMinimum`.

- **YearRules**  
  Container for a year's `tenMile` and `mixedDistance` definitions.

- **CompetitionRulesProvider**  
  Loads competition-rules.json, selects the latest year <= competition year,  
  evaluates formulas, and returns a CompetitionRules instance.

---

## Usage

```csharp
var configDir = FolderLocator.GetConfigDirectory();
var configFilePath = Path.Combine(configDir, "competition-rules.json");

var provider = new CompetitionRulesProvider(configFilePath);
var rules = provider.GetRules(competitionYear, calendar);

// Example: render titles
Console.WriteLine(rules.TenMileTitle);         // "Best 6 ten-mile TTs"
Console.WriteLine(rules.FullCompetitionTitle); // "Scoring 11"
```

## Contributor Notes

- Do not hard-code numbers or text in calculators or renderers. Always consume CompetitionRules.  
- Editing rules: update competition-rules.json.  
- Fixed vs formula: use `count` for fixed values, `formula` + `cap` for computed values.  
- Validation: ensure each group has either `count` or `formula`.  
- Future seasons: add a new year entry; older years remain for historical accuracy.

---

This folder ensures that policy (rules) and mechanics (calculators/renderers) stay in sync.
