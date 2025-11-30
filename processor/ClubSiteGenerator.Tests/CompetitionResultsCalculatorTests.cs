using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Services;

namespace ClubSiteGenerator.Tests
{
    public class CompetitionResultsCalculatorTests
    {
        private readonly List<CalendarEvent> _calendar = new()
        {
            new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
            new CalendarEvent { EventNumber = 2, EventName = "Evening 10", IsEvening10 = true },
            new CalendarEvent { EventNumber = 3, EventName = "25 Mile", IsEvening10 = false },
            new CalendarEvent { EventNumber = 4, EventName = "50 Mile", IsEvening10 = false }
        };

        [Fact]
        public void JuvenileCompetitor_ScoresAreCalculatedCorrectly()
        {
            var competitor = new Competitor
            {
                ClubNumber = 1,
                Surname = "Smith",
                GivenName = "Alice",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,
                AgeGroup = AgeGroup.Juvenile
            };

            var rides = new List<Ride>
            {
                new Ride { Competitor = competitor, EventNumber = 1, JuvenilesPoints = 60, Status = RideStatus.Valid },
                new Ride { Competitor = competitor, EventNumber = 2, JuvenilesPoints = 55, Status = RideStatus.Valid },
                new Ride { Competitor = competitor, EventNumber = 3, JuvenilesPoints = 50, Status = RideStatus.Valid },
                new Ride { Competitor = competitor, EventNumber = 4, JuvenilesPoints = 45, Status = RideStatus.Valid }
            };

            var group = rides.GroupBy(r => r.Competitor!).Single();

            var result = CompetitionResultsCalculator.BuildCompetitorResult(group, _calendar);

            // Assert totals
            Assert.Equal(115, result.TenMileCompetition.Points); // 60 + 55
            Assert.Equal(210, result.FullCompetition.Points);    // 60 + 55 + 50 + 45

            // Assert contributing rides
            Assert.Contains(result.TenMileCompetition.Rides, r => r.EventNumber == 1);
            Assert.Contains(result.TenMileCompetition.Rides, r => r.EventNumber == 2);

            Assert.Equal(4, result.FullCompetition.Rides.Count);
            Assert.Contains(result.FullCompetition.Rides, r => r.EventNumber == 3);
            Assert.Contains(result.FullCompetition.Rides, r => r.EventNumber == 4);
        }

        [Theory]
        [InlineData(true, "Juvenile")]
        [InlineData(false, "Senior")]
        public void Calculator_WorksAcrossAgeGroups(bool isJuvenile, string label)
        {
            var competitor = new Competitor
            {
                ClubNumber = 2,
                Surname = $"Doe-{label}",
                GivenName = $"Bob-{label}",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,
                AgeGroup = isJuvenile ? AgeGroup.Juvenile : AgeGroup.Senior
            };

            var rides = new List<Ride>
        {
            new Ride { Competitor = competitor, EventNumber = 1, JuvenilesPoints = 40, Status = RideStatus.Valid },
            new Ride { Competitor = competitor, EventNumber = 2, JuvenilesPoints = 35, Status = RideStatus.Valid },
            new Ride { Competitor = competitor, EventNumber = 3, JuvenilesPoints = 30, Status = RideStatus.Valid },
            new Ride { Competitor = competitor, EventNumber = 4, JuvenilesPoints = 25, Status = RideStatus.Valid }
        };

            var group = rides.GroupBy(r => r.Competitor!).Single();

            var result = CompetitionResultsCalculator.BuildCompetitorResult(group, _calendar);

            Assert.True(result.FullCompetition.Points > 0, $"{label} scoring should be positive");
            Assert.NotEmpty(result.FullCompetition.Rides);
        }
    }
}
