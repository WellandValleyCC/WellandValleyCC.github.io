using ClubCore.Models;
using ClubSiteGenerator.Models;
using System.Text.Json;

namespace ClubSiteGenerator.Rules
{
    public class CompetitionRulesProvider
    {
        private readonly Dictionary<int, YearRules> _configs;

        public CompetitionRulesProvider(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);
            _configs = JsonSerializer.Deserialize<Dictionary<int, YearRules>>(json)
                       ?? throw new InvalidOperationException("Failed to load competition rules config.");
        }

        public CompetitionRules GetRules(int competitionYear, IEnumerable<CalendarEvent> calendar)
        {
            var candidateYears = _configs.Keys.Where(y => y <= competitionYear).ToList();
            if (!candidateYears.Any())
                throw new InvalidOperationException($"No competition rules configured for {competitionYear} or earlier.");

            var effectiveYear = candidateYears.Max();
            var config = _configs[effectiveYear];

            return BuildRules(config, calendar);
        }

        private CompetitionRules BuildRules(YearRules config, IEnumerable<CalendarEvent> calendar)
        {
            int relevantEvents = calendar.Count();

            // Ten‑mile
            int tenMileCount = Resolve(config.TenMile, relevantEvents);

            // Mixed distance
            int mixedCount = Resolve(config.MixedDistance, relevantEvents);
            int nonTenMinimum = config.MixedDistance.NonTenMinimum ?? 0;

            return new CompetitionRules(tenMileCount, nonTenMinimum, mixedCount);
        }

        private int Resolve(RuleDefinition rule, int calendarEvents)
        {
            if (rule.Count.HasValue)
                return rule.Count.Value;

            if (!string.IsNullOrWhiteSpace(rule.Formula))
            {
                int value = EvaluateFormula(rule.Formula, calendarEvents);
                if (rule.Cap.HasValue)
                    value = Math.Min(value, rule.Cap.Value);
                return value;
            }

            throw new InvalidOperationException("Rule definition must have either Count or Formula.");
        }

        private int EvaluateFormula(string formula, int calendarEvents)
        {
            return formula switch
            {
                "calendarEvents/2+1" => (calendarEvents / 2) + 1,
                _ => throw new NotSupportedException($"Unsupported formula: {formula}")
            };
        }
    }
}
