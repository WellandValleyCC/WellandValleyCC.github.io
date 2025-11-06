using ClubProcessor.Services;
using FluentAssertions;
using Xunit;

namespace EventProcessor.Tests
{
    public class VttaStandardYearResolverTests
    {
        [Theory]
        [InlineData(2023, new[] { 2024, 2026 }, false)] // no match
        [InlineData(2024, new[] { 2024, 2026 }, true)]
        [InlineData(2025, new[] { 2024, 2026 }, true)]
        [InlineData(2026, new[] { 2024, 2026 }, true)]
        [InlineData(2027, new[] { 2024, 2026 }, true)]
        public void GetEffectiveStandardYear_WithInjectedYears_ThrowsOrReturnsExpected(int seasonYear, int[] availableYears, bool shouldSucceed)
        {
            Action act = () => VttaStandardYearResolver.GetEffectiveStandardYear(seasonYear, availableYears);

            if (shouldSucceed)
            {
                var expected = availableYears.Where(y => y <= seasonYear).Max();
                act.Should().NotThrow();
                VttaStandardYearResolver.GetEffectiveStandardYear(seasonYear, availableYears).Should().Be(expected);
            }
            else
            {
                act.Should().Throw<InvalidOperationException>()
                   .WithMessage($"*{seasonYear}*");
            }
        }

        [Fact]
        public void GetEffectiveStandardYear_ThrowsIfAvailableYearsEmpty()
        {
            var empty = Array.Empty<int>();
            Action act = () => VttaStandardYearResolver.GetEffectiveStandardYear(2025, empty);

            act.Should().Throw<InvalidOperationException>()
               .WithMessage("No standards available*");
        }
    }
}
