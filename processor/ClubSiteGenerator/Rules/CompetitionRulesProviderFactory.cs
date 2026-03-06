using ClubCore.Utilities;

namespace ClubSiteGenerator.Rules
{
    public static class CompetitionRulesFactory
    {
        public static ICompetitionRulesProvider CreateRulesProvider()
        {
            var folderLocator = new DefaultFolderLocator(
                new DefaultDirectoryProvider(),
                new DefaultLog());

            var configDir = folderLocator.GetConfigDirectory();

            var configFilePath = Path.Combine(configDir, "competition-rules.json");
            return new CompetitionRulesProvider(configFilePath);
        }
    }
}
