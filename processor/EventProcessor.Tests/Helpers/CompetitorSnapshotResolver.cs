using ClubCore.Models;

namespace EventProcessor.Tests.Helpers
{
    public static class CompetitorSnapshotResolver
    {
        // Picks the latest version of the numbered competitor whose CreatedUtc is <= eventDateUtc
        public static Competitor? ResolveForEvent(
            IReadOnlyDictionary<int, List<Competitor>> snapshotsByClubNumber,
            int clubNumber,
            DateTime eventDateUtc)
        {
            if (!snapshotsByClubNumber.TryGetValue(clubNumber, out var snaps)) return null;

            return snaps
                .Where(s => s.CreatedUtc <= eventDateUtc)
                .OrderByDescending(s => s.CreatedUtc)
                .FirstOrDefault();
        }
    }
}
