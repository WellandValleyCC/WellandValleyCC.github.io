using ClubProcessor.Calculators;
using ClubProcessor.Interfaces;
using ClubProcessor.Services; // for VetsHandicapLookup
using System.Reflection;

namespace ClubProcessor.Orchestration
{
    public static class RideProcessingCoordinatorFactory
    {
        /// <summary>
        /// Discover and instantiate all concrete IRideProcessor types in the provided assembly.
        /// </summary>
        public static List<IRideProcessor> DiscoverAll(
            Func<int, int> pointsForPosition,
            int competitionYear,
            Assembly? searchAssembly = null,
            IEnumerable<Type>? additionalProcessorTypes = null,
            IEnumerable<Type>? excludeProcessorTypes = null)
        {
            searchAssembly ??= typeof(RideProcessingCoordinator).Assembly;

            var discovered = searchAssembly
                .GetTypes()
                .Where(t => typeof(IRideProcessor).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToList();

            if (additionalProcessorTypes != null)
                discovered.AddRange(additionalProcessorTypes);

            if (excludeProcessorTypes != null)
                discovered = discovered.Except(excludeProcessorTypes).ToList();

            return InstantiateProcessors(discovered, pointsForPosition, competitionYear);
        }

        /// <summary>
        /// Create a ready-to-use RideProcessingCoordinator using discovered processors.
        /// </summary>
        public static RideProcessingCoordinator Create(
            Func<int, int> pointsForPosition,
            int competitionYear,
            IEnumerable<Type>? additionalProcessorTypes = null,
            IEnumerable<Type>? excludeProcessorTypes = null,
            Assembly? searchAssembly = null)
        {
            var processors = DiscoverAll(pointsForPosition, competitionYear, searchAssembly, additionalProcessorTypes, excludeProcessorTypes);
            return new RideProcessingCoordinator(processors, pointsForPosition);
        }

        // Centralised instantiation logic used by both public factories
        private static List<IRideProcessor> InstantiateProcessors(IEnumerable<Type> types, Func<int, int> pointsForPosition, int competitionYear)
        {
            return types
                .Distinct()
                .OrderBy(t => t.FullName, StringComparer.Ordinal)
                .Select(type =>
                {
                    // Special case: VeteransCompetitionCalculator
                    if (type == typeof(VeteransCompetitionCalculator))
                    {
                        var handicapProvider = VetsHandicapLookup.ForSeason(competitionYear);
                        return (IRideProcessor)Activator.CreateInstance(
                            type,
                            pointsForPosition,
                            competitionYear,
                            handicapProvider)!;
                    }

                    // Generic case: ctor with delegate
                    var ctorWithDelegate = type.GetConstructor(new[] { typeof(Func<int, int>) });
                    if (ctorWithDelegate != null)
                        return (IRideProcessor)ctorWithDelegate.Invoke(new object[] { pointsForPosition });

                    // Default ctor
                    var defaultCtor = type.GetConstructor(Type.EmptyTypes);
                    if (defaultCtor != null)
                        return (IRideProcessor)Activator.CreateInstance(type)!;

                    throw new InvalidOperationException($"Cannot instantiate {type.FullName}: no compatible constructor found.");
                })
                .ToList();
        }
    }
}