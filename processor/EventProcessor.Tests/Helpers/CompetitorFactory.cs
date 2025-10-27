using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor.Tests.Helpers
{
    public enum AgeGroup { IsJuvenile, IsJunior, IsSenior, IsVeteran }

    public static class CompetitorFactory
    {
        public static Competitor Create(
            int clubNumber,
            string surname = "Test",
            string givenName = "Rider",
            ClaimStatus claimStatus = ClaimStatus.FirstClaim,
            bool isFemale = false,
            AgeGroup ageGroup = AgeGroup.IsSenior)
        {
            return new Competitor
            {
                ClubNumber = clubNumber,
                Surname = surname,
                GivenName = givenName,
                ClaimStatus = claimStatus,
                IsFemale = isFemale,
                IsJuvenile = ageGroup == AgeGroup.IsJuvenile,
                IsJunior = ageGroup == AgeGroup.IsJunior,
                IsSenior = ageGroup == AgeGroup.IsSenior,
                IsVeteran = ageGroup == AgeGroup.IsVeteran,
                CreatedUtc = DateTime.UtcNow,
                LastUpdatedUtc = DateTime.UtcNow
            };
        }
    }
}
