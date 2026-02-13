namespace ClubSiteGenerator.Interfaces
{
    public interface IDbPathResolver
    {
        string GetCompetitorDbPath(string year);
        string GetEventDbPath(string year);
    }
}