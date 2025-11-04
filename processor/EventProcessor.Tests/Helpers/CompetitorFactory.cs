using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor.Tests.Helpers
{
    public static class CompetitorFactory
    {
        //public static Competitor Create(
        //    int clubNumber,
        //    string surname = "Test",
        //    string givenName = "Rider",
        //    ClaimStatus claimStatus = ClaimStatus.FirstClaim,
        //    bool isFemale = false,
        //    AgeGroup ageGroup = AgeGroup.IsSenior)
        //{
        //    return new Competitor
        //    {
        //        ClubNumber = clubNumber,
        //        Surname = surname,
        //        GivenName = givenName,
        //        ClaimStatus = claimStatus,
        //        IsFemale = isFemale,
        //        IsJuvenile = ageGroup == AgeGroup.IsJuvenile,
        //        IsJunior = ageGroup == AgeGroup.IsJunior,
        //        IsSenior = ageGroup == AgeGroup.IsSenior,
        //        IsVeteran = ageGroup == AgeGroup.IsVeteran,
        //        CreatedUtc = DateTime.UtcNow,
        //        LastUpdatedUtc = DateTime.UtcNow
        //    };
        //}

        public static Competitor Create(
            int clubNumber,
            string surname = "Test",
            string givenName = "Rider",
            ClaimStatus claimStatus = ClaimStatus.FirstClaim,
            bool isFemale = false,
            AgeGroup ageGroup = AgeGroup.IsSenior,
            DateTime? createdUtc = null,
            int? vetsBucket = null,
            int? id = null)
        {
            var created = createdUtc ?? DateTime.UtcNow.Date.AddDays(-100);

            return new Competitor
            {
                Id = id ?? 0,
                ClubNumber = clubNumber,
                Surname = surname,
                GivenName = givenName,
                ClaimStatus = claimStatus,
                IsFemale = isFemale,
                AgeGroup = ageGroup,
                CreatedUtc = created,
                LastUpdatedUtc = created,
                VetsBucket = vetsBucket
            };
        }

        public static IEnumerable<Competitor> CreateFutureVersions(
            Competitor baseCompetitor,
            int snapshots = 2,
            TimeSpan? interval = null,
            bool capToNow = false)
        {
            if (baseCompetitor == null) throw new ArgumentNullException(nameof(baseCompetitor));
            interval ??= TimeSpan.FromDays(30);

            var baseLastUpdatedUtc = DateTime.SpecifyKind(baseCompetitor.LastUpdatedUtc, DateTimeKind.Utc);

            // Produce snapshots where the first yielded version is base.LastUpdatedUtc + interval
            for (int i = 1; i <= snapshots; i++)
            {
                // i = 1 => first yielded snapshot; alternate ClaimStatus each snapshot
                var status = (i % 2 == 1) ? Toggle(baseCompetitor.ClaimStatus) : baseCompetitor.ClaimStatus;
                var lastUpdated = DateTime.SpecifyKind(baseLastUpdatedUtc.AddTicks(interval.Value.Ticks * i), DateTimeKind.Utc);

                if (capToNow && lastUpdated > DateTime.UtcNow) lastUpdated = DateTime.UtcNow;

                yield return new Competitor
                {
                    ClubNumber = baseCompetitor.ClubNumber,
                    Surname = baseCompetitor.Surname,
                    GivenName = baseCompetitor.GivenName,
                    ClaimStatus = status,
                    IsFemale = baseCompetitor.IsFemale,
                    AgeGroup = baseCompetitor.AgeGroup,
                    CreatedUtc = lastUpdated,
                    LastUpdatedUtc = lastUpdated
                    // copy any other properties your Competitor model has here, preserving original values
                };
            }

            static ClaimStatus Toggle(ClaimStatus s) =>
                s == ClaimStatus.FirstClaim ? ClaimStatus.SecondClaim : ClaimStatus.FirstClaim;
        }
    }
}
