using ClubProcessor.Context;

namespace ClubProcessor.Configuration;

public static class CompetitionConfig
{
    public static Func<int, int> LoadPointsForPosition(EventDbContext context)
    {
        var map = context.PointsAllocations
            .ToDictionary(p => p.Position, p => p.Points);

        return position => map.TryGetValue(position, out var points) ? points : 0;
    }
}
