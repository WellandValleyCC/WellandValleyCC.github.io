using ClubProcessor.Calculators;
using ClubProcessor.Models;
using ClubProcessor.Models.Enums;
using ClubProcessor.Orchestration;
using ClubProcessor.Services;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor.Tests
{
    public class VeteransCompetitionScoringCalculatorTests
    {
        [Theory]
        [InlineData(9.5,  1, false, 10000, 35)]  
        [InlineData(10.0, 1, false, 10000, 36)]
        [InlineData(25.0, 1, false, 10000, 94)]
                                
        [InlineData(9.5,  5, false, 10000, 57)]  
        [InlineData(10.0, 5, false, 10000, 59)]
        [InlineData(25.0, 5, false, 10000, 154)]
                          
        [InlineData(9.5,  10, false, 10000, 91)] 
        [InlineData(10.0, 10, false, 10000, 96)]
        [InlineData(25.0, 10, false, 10000, 248)]
                              
        [InlineData(9.5,  15, false, 10000, 133)] 
        [InlineData(10.0, 15, false, 10000, 140)]
        [InlineData(25.0, 15, false, 10000, 364)]
                              
        [InlineData(9.5,  20, false, 10000, 184)] 
        [InlineData(10.0, 20, false, 10000, 194)]
        [InlineData(25.0, 20, false, 10000, 506)]

        [InlineData(9.5, 1,   true, 10000, 159)]
        [InlineData(10.0, 1,  true, 10000, 167)]
        [InlineData(25.0, 1,  true, 10000, 424)]
                        
        [InlineData(9.5, 5,   true, 10000, 172)]
        [InlineData(10.0, 5,  true, 10000, 180)]
        [InlineData(25.0, 5,  true, 10000, 461)]
                    
        [InlineData(9.5, 10,  true, 10000, 202)]
        [InlineData(10.0, 10, true, 10000, 213)]
        [InlineData(25.0, 10, true, 10000, 546)]
                            
        [InlineData(9.5, 15,  true, 10000, 257)]
        [InlineData(10.0, 15, true, 10000, 271)]
        [InlineData(25.0, 15, true, 10000, 696)]
                    
        [InlineData(9.5, 20,  true, 10000, 347)]
        [InlineData(10.0, 20, true, 10000, 365)]
        [InlineData(25.0, 20, true, 10000, 941)]
        public void CalculatesCorrectAAT_ForVariousDistancesAndBuckets(
            double distanceMiles,
            int vetsBucket,
            bool isFemale,
            int totalSeconds,
            int expectedAatSeconds)
        {
            var competitor = new Competitor
            {
                ClubNumber = 12345,
                Surname = "Rider",
                GivenName = "Test",
                IsFemale = isFemale,
                AgeGroup = AgeGroup.Veteran,
                ClaimStatus = ClaimStatus.FirstClaim,
                VetsBucket = vetsBucket
            };

            var calendarEvent = new CalendarEvent
            {
                EventNumber = 1,
                Miles = distanceMiles,
                EventDate = new DateTime(2025, 8, 11)
            };

            var ride = new Ride
            {
                EventNumber = 1,
                ClubNumber = competitor.ClubNumber,
                Name = competitor.FullName,
                TotalSeconds = totalSeconds,
                Competitor = competitor,
                CalendarEvent = calendarEvent,
                Eligibility = RideEligibility.Valid
            };

            var rides = new List<Ride> { ride };
            var competitors = new List<Competitor> { competitor };
            var competitorVersions = TestHelpers.CreateCompetitorVersionsLookup(competitors);
            var calendarEvents = new List<CalendarEvent> { calendarEvent };

            var calculator = new VeteransCompetitionCalculator(
                 PointsProvider.AsDelegate(),
                 2025,
                 VetsHandicapLookup.ForSeason(2025));

            var scorer = RideProcessingCoordinatorFactory.Create(PointsProvider.AsDelegate(), 2025);

            // Act
            scorer.ProcessAll(rides, competitors, calendarEvents);

            // Assert
            var debug = TestHelpers.RenderVeteransDebugOutput(rides, competitorVersions, new[] { 1 });
            _ = debug; // breakpoint-friendly

            ride.HandicapSeconds.Should().Be(
                expectedAatSeconds,
                because: $"VetsBucket {vetsBucket}, {(isFemale?"Female":"Male")}, Distance {distanceMiles} miles");

            ride.HandicapTotalSeconds.Should().Be(
                totalSeconds - ride.HandicapSeconds,
                because: $"VetsBucket {vetsBucket}, {(isFemale ? "Female" : "Male")}, Distance {distanceMiles} miles");
        }
    }
}
