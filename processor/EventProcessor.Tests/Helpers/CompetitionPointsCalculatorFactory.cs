using ClubProcessor.Interfaces;
using ClubProcessor.Orchestration;
using System.Reflection;

namespace EventProcessor.Tests.Helpers
{
    public static class CompetitionPointsCalculatorFactory
    {
        /// <summary>
        /// Create a CompetitionPointsCalculator populated with all concrete ICompetitionScoreCalculator
        /// types discovered in the same assembly as the provided marker type (default: typeof(CompetitionPointsCalculator)).
        /// </summary>
        public static CompetitionPointsCalculator Create(
            IEnumerable<Type>? additionalCalculatorTypes = null,
            IEnumerable<Type>? excludeCalculatorTypes = null,
            Assembly? searchAssembly = null)
        {
            searchAssembly ??= typeof(CompetitionPointsCalculator).Assembly;

            // discover concrete public types implementing the interface
            var discovered = searchAssembly
                .GetTypes()
                .Where(t => typeof(ICompetitionScoreCalculator).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsInterface && t.GetConstructor(Type.EmptyTypes) != null)
                .ToList();

            if (additionalCalculatorTypes != null)
                discovered.AddRange(additionalCalculatorTypes.Where(t => t != null));

            if (excludeCalculatorTypes != null)
                discovered = discovered.Except(excludeCalculatorTypes).ToList();

            // instantiate in deterministic order (by type name) to avoid test flakiness
            var calculators = discovered
                .Distinct()
                .OrderBy(t => t.FullName, StringComparer.Ordinal)
                .Select(Activator.CreateInstance)
                .Cast<ICompetitionScoreCalculator>()
                .ToList();

            return new CompetitionPointsCalculator(calculators);
        }

        /// <summary>
        /// Convenience overload to create with a specific set of calculators (useful for narrow tests).
        /// </summary>
        public static CompetitionPointsCalculator CreateFrom(params ICompetitionScoreCalculator[] calculators)
        {
            return new CompetitionPointsCalculator(calculators.ToList());
        }
    }
}
