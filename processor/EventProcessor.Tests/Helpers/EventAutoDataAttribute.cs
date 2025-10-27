using AutoFixture;
using AutoFixture.Xunit2;
using ClubProcessor.Models;
using EventProcessor.Tests;
using EventProcessor.Tests.Helpers;
using System.Collections.Generic;

public class EventAutoDataAttribute : AutoDataAttribute
{
    public EventAutoDataAttribute() : base(CreateFixture) { }

    private static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        // Use deterministic builders: supply TestCompetitors and TestRides as-is
        fixture.Register(() => TestCompetitors.All.ToList());
        fixture.Register(() => TestRides.All.Where(r => true).Select(r => CloneForFixture(r)).ToList());

        // Provide PointsForPosition helper via Func<int,int>
        fixture.Register<Func<int, int>>(() => (pos) => LeaguePointsCalculatorTests.PointsForPosition(pos));

        // Optional: configure any other defaults or customizations here
        return fixture;
    }

    private static Ride CloneForFixture(Ride r) =>
        new Ride
        {
            EventNumber = r.EventNumber,
            ClubNumber = r.ClubNumber,
            Name = r.Name,
            ActualTime = r.ActualTime,
            TotalSeconds = r.TotalSeconds,
            IsRoadBike = r.IsRoadBike,
            Eligibility = r.Eligibility,
            AvgSpeed = r.AvgSpeed,
            EventPosition = r.EventPosition
        };
}
