using ClubCore.Models;
using ClubCore.Models.Enums;
using ClubSiteGenerator.ResultsGenerator;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubSiteGenerator.Tests
{
    public class ResultsGeneratorTests
    {
        [Fact]
        public void OrderedIneligibleRides_SortsMembersThenSecondClaimThenGuests()
        {
            // Arrange
            var competitor = new Competitor
            {
                ClubNumber = 123,
                ClaimStatus = ClaimStatus.FirstClaim, // whatever enum/value your domain expects
                IsFemale = false,
                AgeGroup = AgeGroup.Senior,           // again, pick a valid enum/value
                Surname = "Smith",
                GivenName = "Alice",
                VetsBucket = null
            };

            var rides = new[]
            {
                new Ride
                {
                    Eligibility = RideEligibility.DNF,
                    EventEligibleRidersRank = 1,
                    Competitor = competitor
                },
                new Ride
                {
                    Eligibility = RideEligibility.DNF,
                    ClubNumber = 123,
                    Name = "Bob SecondClaim"
                },
                new Ride
                {
                    Eligibility = RideEligibility.DNF,
                    ClubNumber = null,
                    Name = "Charlie Guest"
                }
            };

            // Act
            var ordered = BaseResults.OrderedIneligibleRides(rides, RideEligibility.DNF).ToList();

            // Assert
            ordered[0].Competitor?.Surname.Should().Be("Smith");   // member first
            ordered[1].Name.Should().Contain("SecondClaim");      // then 2nd claim
            ordered[2].Name.Should().Contain("Guest");            // then guest
        }
    }
}
