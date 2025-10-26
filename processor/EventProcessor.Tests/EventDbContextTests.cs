using ClubProcessor.Models;
using EventProcessor.Tests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventProcessor.Tests
{
    public class EventDbContextTests
    {
        [Theory]
        [InlineData(1, 60)]
        [InlineData(46, 2)]
        [InlineData(100, 1)]
        [InlineData(101, 1)] // clamped
        [InlineData(0, 60)]  // clamped
        public void GetPointsForPosition_ReturnsExpectedPoints(int position, int expected)
        {
            // Assemble
            using var context = DbContextFactory.CreateEventContext();

            context.PointsAllocations.AddRange(
                Enumerable.Range(1, 100).Select(pos => new PointsAllocation
                {
                    Position = pos,
                    Points = pos switch
                    {
                        1 => 60,
                        2 => 55,
                        3 => 51,
                        4 => 48,
                        5 => 46,
                        6 => 44,
                        7 => 42,
                        8 => 40,
                        9 => 39,
                        10 => 38,
                        11 => 37,
                        12 => 36,
                        13 => 35,
                        14 => 34,
                        15 => 33,
                        16 => 32,
                        17 => 31,
                        18 => 30,
                        19 => 29,
                        20 => 28,
                        21 => 27,
                        22 => 26,
                        23 => 25,
                        24 => 24,
                        25 => 23,
                        26 => 22,
                        27 => 21,
                        28 => 20,
                        29 => 19,
                        30 => 18,
                        31 => 17,
                        32 => 16,
                        33 => 15,
                        34 => 14,
                        35 => 13,
                        36 => 12,
                        37 => 11,
                        38 => 10,
                        39 => 9,
                        40 => 8,
                        41 => 7,
                        42 => 6,
                        43 => 5,
                        44 => 4,
                        45 => 3,
                        46 => 2,
                        >= 47 and <= 100 => 1,
                        _ => 0
                    }
                })
            );
            context.SaveChanges();

            // Act
            var result = context.GetPointsForPosition(position);

            // Assert
            result.Should().Be(expected);
        }

    }
}
