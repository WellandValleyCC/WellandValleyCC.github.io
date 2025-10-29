using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor.Tests.Helpers
{
    public static class TestCompetitorHistories
    {
        // Alternates claim status starting from initialStatus, increments LastUpdated by interval
        public static IEnumerable<Competitor> CreateHistory(
            int clubNumber,
            string lastName,
            string firstName,
            ClaimStatus initialStatus,
            DateTime firstUpdated,
            int snapshots = 2,
            TimeSpan? interval = null)
        {
            interval ??= TimeSpan.FromDays(30);

            for (int i = 0; i < snapshots; i++)
            {
                yield return CompetitorFactory.Create(
                    clubNumber,
                    lastName,
                    firstName,
                    i % 2 == 0 ? initialStatus : Toggle(initialStatus),
                    lastUpdated: firstUpdated.AddTicks(interval.Value.Ticks * i)
                );
            }

            static ClaimStatus Toggle(ClaimStatus s) =>
                s == ClaimStatus.FirstClaim ? ClaimStatus.SecondClaim : ClaimStatus.FirstClaim;
        }
    }
}
