using ClubCore.Utilities;

namespace ClubSiteGenerator.Rules
{
    public static class CompetitionRulesFactory
    {
        public static ICompetitionRulesProvider CreateRulesProvider()
        {
            var configDir = FolderLocator.GetConfigDirectory();
            var configFilePath = Path.Combine(configDir, "competition-rules.json");
            return new CompetitionRulesProvider(configFilePath);
        }
    }
}
