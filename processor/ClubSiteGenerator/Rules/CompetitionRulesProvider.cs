using ClubCore.Models;
using System.Text.Json;

namespace ClubSiteGenerator.Rules
{
    public class CompetitionRulesProvider : ICompetitionRulesProvider
    {
        private readonly Dictionary<int, YearRules> _configs;

        public CompetitionRulesProvider(string jsonPath)
        {
            var json = File.ReadAllText(jsonPath);
            _configs = JsonSerializer.Deserialize<Dictionary<int, YearRules>>(json)
                       ?? throw new InvalidOperationException("Failed to load competition rules config.");
        }

        public ICompetitionRules GetRules(int competitionYear, IEnumerable<CalendarEvent> calendar)
        {
            Console.WriteLine($"[INFO] Getting rules for {competitionYear}");

            var candidateYears = _configs.Keys.Where(y => y <= competitionYear).ToList();
            if (!candidateYears.Any())
                throw new InvalidOperationException($"No competition rules configured for {competitionYear} or earlier.");

            var effectiveYear = candidateYears.Max();
            var config = _configs[effectiveYear];

            var rules = BuildRules(config, calendar);

            Console.WriteLine(
                $"[INFO] Rules: effectiveYear={effectiveYear}, " +
                $"TenMileCount={rules.TenMileCount}, " +
                $"MixedEventCount={rules.MixedEventCount}, " +
                $"NonTenMinimum={rules.NonTenMinimum}, " +
                $"RoundRobinCount={rules.RoundRobin.Count}");

            return rules;
        }

        private CompetitionRules BuildRules(YearRules config, IEnumerable<CalendarEvent> calendar)
        {
            int relevantEvents = calendar.Count();

            //
            // WVCC: Ten‑mile rules
            //
            int tenMileCount = Resolve(config.TenMile, relevantEvents);

            //
            // WVCC: Mixed‑distance rules
            //
            int mixedCount = Resolve(config.MixedDistance, relevantEvents);
            int nonTenMinimum = config.MixedDistance.NonTenMinimum ?? 0;

            //
            // Round Robin rules (optional)
            //
            RoundRobinRules? rrRules = null;

            if (config.RoundRobin != null)
            {
                // RR count (fixed or formula)
                int rrCount = Resolve(config.RoundRobin, relevantEvents);

                // RR minimum rides
                int rrMinimum = config.RoundRobin.Minimum ?? 0;

                // RR team scoring rules
                var team = new RoundRobinTeamRules
                {
                    OpenCount = config.RoundRobin.Team?.OpenCount ?? 4,
                    WomenCount = config.RoundRobin.Team?.WomenCount ?? 1
                };

                rrRules = new RoundRobinRules
                {
                    Count = rrCount,
                    Minimum = rrMinimum,
                    Team = team
                };
            }

            //
            // Construct final rules object
            // (rrRules may be null — CompetitionRules will apply defaults)
            //
            return new CompetitionRules(
                tenMileCount,
                nonTenMinimum,
                mixedCount,
                config.LeagueSponsor,
                rrRules);
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