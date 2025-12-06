using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.Interfaces;
using ClubSiteGenerator.Services;
using ClubSiteGenerator.Tests.Helpers;

namespace ClubSiteGenerator.Tests
{
    public class CompetitionResultsCalculatorTests
    {
        private readonly List<CalendarEvent> _calendar = new()
        {
            new CalendarEvent { EventNumber = 1, EventName = "Evening 10", IsEvening10 = true },
            new CalendarEvent { EventNumber = 2, EventName = "Evening 10", IsEvening10 = true },
            new CalendarEvent { EventNumber = 3, EventName = "25 Mile", IsEvening10 = false },
            new CalendarEvent { EventNumber = 4, EventName = "9.5 Mile - 1", IsEvening10 = false },
            new CalendarEvent { EventNumber = 5, EventName = "9.5 Mile - 2", IsEvening10 = false },
            new CalendarEvent { EventNumber = 6, EventName = "9.5 Mile - 3", IsEvening10 = false },
            new CalendarEvent { EventNumber = 7, EventName = "9.5 Mile - 4", IsEvening10 = false },
            new CalendarEvent { EventNumber = 8, EventName = "9.5 Mile - 5", IsEvening10 = false }
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

            var rulesProvider = new FakeRulesProvider();
            var tenMileRule = rulesProvider.GetRule(2025, CompetitionRuleScope.TenMile);
            var fullCompetitionRule = (IMixedCompetitionRule)rulesProvider.GetRule(2025, CompetitionRuleScope.Full);

            var result = CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), _calendar, r => r.JuvenilesPoints, tenMileRule, fullCompetitionRule);

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

            var rulesProvider = new FakeRulesProvider();
            var tenMileRule = rulesProvider.GetRule(2025, CompetitionRuleScope.TenMile);
            var fullCompetitionRule = (IMixedCompetitionRule)rulesProvider.GetRule(2025, CompetitionRuleScope.Full);
            
            var result = CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), _calendar, r => r.JuvenilesPoints, tenMileRule, fullCompetitionRule);

            Assert.True(result.FullCompetition.Points > 0, $"{label} scoring should be positive");
            Assert.NotEmpty(result.FullCompetition.Rides);
        }

        [Fact]
        public void BuildCompetitorResult_SplitsEventsCompletedIntoTensAndOther()
        {
            // Arrange: competitor with 2 valid ten‑mile rides, 1 valid non‑ten, and 1 DNS
            var competitor = new Competitor
            {
                ClubNumber = 1,
                Surname = "Smith",
                GivenName = "Alice",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = false,
                AgeGroup = AgeGroup.Juvenile
            };

            var calendar = new List<CalendarEvent>
            {
                new CalendarEvent { EventNumber = 1, IsEvening10 = true },
                new CalendarEvent { EventNumber = 2, IsEvening10 = true },
                new CalendarEvent { EventNumber = 3, IsEvening10 = false },
                new CalendarEvent { EventNumber = 4, IsEvening10 = false }
            };

            var rides = new List<Ride>
            {
                new Ride { Competitor = competitor, EventNumber = 1, Status = RideStatus.Valid, JuvenilesPoints = 10 },
                new Ride { Competitor = competitor, EventNumber = 2, Status = RideStatus.Valid, JuvenilesPoints = 12 },
                new Ride { Competitor = competitor, EventNumber = 3, Status = RideStatus.Valid, JuvenilesPoints = 8 },
                new Ride { Competitor = competitor, EventNumber = 4, Status = RideStatus.DNS, JuvenilesPoints = 0 }
            };

            var group = rides
                .Where(r => r.Competitor != null)
                .GroupBy(r => r.Competitor!)
                .First();

            var rulesProvider = new FakeRulesProvider();
            var tenMileRule = rulesProvider.GetRule(2025, CompetitionRuleScope.TenMile);
            var fullCompetitionRule = (IMixedCompetitionRule)rulesProvider.GetRule(2025, CompetitionRuleScope.Full);

            // Act
            var result = CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.JuvenilesPoints, tenMileRule, fullCompetitionRule);

            // Assert
            Assert.Equal(2, result.EventsCompletedTens);   // two valid ten‑mile rides
            Assert.Equal(1, result.EventsCompletedOther);  // one valid non‑ten ride
            Assert.Equal(3, result.EventsCompleted);       // total = 2 + 1
        }

        [Fact]
        public void BuildCompetitorResult_AllNonTenRides_CountsOnlyOther()
        {
            // Arrange: competitor with 3 valid non‑ten rides and 1 DNF
            var competitor = new Competitor
            {
                ClubNumber = 2,
                Surname = "Jones",
                GivenName = "Ben",
                ClaimStatus = ClaimStatus.FirstClaim,
                IsFemale = true,
                AgeGroup = AgeGroup.Juvenile
            };

            var calendar = new List<CalendarEvent>
            {
                new CalendarEvent { EventNumber = 10, IsEvening10 = false },
                new CalendarEvent { EventNumber = 11, IsEvening10 = false },
                new CalendarEvent { EventNumber = 12, IsEvening10 = false },
                new CalendarEvent { EventNumber = 13, IsEvening10 = false }
            };

            var rides = new List<Ride>
            {
                new Ride { Competitor = competitor, EventNumber = 10, Status = RideStatus.Valid, JuvenilesPoints = 15 },
                new Ride { Competitor = competitor, EventNumber = 11, Status = RideStatus.Valid, JuvenilesPoints = 14 },
                new Ride { Competitor = competitor, EventNumber = 12, Status = RideStatus.Valid, JuvenilesPoints = 13 },
                new Ride { Competitor = competitor, EventNumber = 13, Status = RideStatus.DNF, JuvenilesPoints = 0 }
            };

            var group = rides
                .Where(r => r.Competitor != null)
                .GroupBy(r => r.Competitor!)
                .First();

            var rulesProvider = new FakeRulesProvider();
            var tenMileRule = rulesProvider.GetRule(2025, CompetitionRuleScope.TenMile);
            var fullCompetitionRule = (IMixedCompetitionRule)rulesProvider.GetRule(2025, CompetitionRuleScope.Full);

            // Act
            var result = CompetitionResultsCalculator.BuildCompetitorResult(group.ToList(), calendar, r => r.JuvenilesPoints, tenMileRule, fullCompetitionRule);

            // Assert
            Assert.Equal(0, result.EventsCompletedTens);   // no ten‑mile rides
            Assert.Equal(3, result.EventsCompletedOther);  // three valid non‑ten rides
            Assert.Equal(3, result.EventsCompleted);       // total = 0 + 3
        }
    }
}
