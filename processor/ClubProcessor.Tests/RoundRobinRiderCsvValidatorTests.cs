using ClubCore.Models.Csv;
using ClubProcessor.Services.Validation;
using FluentAssertions;

namespace ClubProcessor.Tests
{
    public class RoundRobinRiderCsvValidatorTests
    {
        [Fact]
        public void Validate_WhenAllFieldsValid_ShouldReturnNoIssues()
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow
            {
                Name = "Alice",
                Club = "Velo",
                DecoratedName = "Alice (Velo)",
                IsFemale = "Y"
            }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows);

            issues.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "Velo", " (Velo)", "Line 2: Name is missing.")]
        [InlineData("Alice", "Velo", "", "Line 2: DecoratedName is missing.")]
        public void Validate_WhenRequiredFieldMissing_ShouldReturnExpectedIssue(
            string name, string club, string decoratedName, string expectedIssue)
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow
            {
                Name = name,
                Club = club,
                DecoratedName = decoratedName,
                IsFemale = ""
            }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows);

            issues.Should().ContainSingle().Which.Should().Be(expectedIssue);
        }

        [Fact]
        public void Validate_WhenDecoratedNameIncorrect_ShouldReturnIssue()
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow
            {
                Name = "Alice",
                Club = "Velo",
                DecoratedName = "Alice-Velo",
                IsFemale = ""
            }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows);

            issues.Should().Contain("Line 2: DecoratedName should be \"Name (Club)\".");
        }

        [Theory]
        [InlineData("")]
        [InlineData("Y")]
        [InlineData("y")]
        public void Validate_WhenIsFemaleValid_ShouldReturnNoIssues(string isFemale)
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow
            {
                Name = "Alice",
                Club = "Velo",
                DecoratedName = "Alice (Velo)",
                IsFemale = isFemale
            }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows);

            issues.Should().BeEmpty();
        }

        [Theory]
        [InlineData("X")]
        [InlineData("N")]
        [InlineData("Female")]
        public void Validate_WhenIsFemaleInvalid_ShouldReturnIssue(string isFemale)
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow
            {
                Name = "Alice",
                Club = "Velo",
                DecoratedName = "Alice (Velo)",
                IsFemale = isFemale
            }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows);

            issues.Should().Contain("Line 2: IsFemale should be blank or \"Y\".");
        }

        [Fact]
        public void Validate_MultipleRows_ShouldIncrementLineNumbersCorrectly()
        {
            var rows = new[]
            {
            new RoundRobinRiderCsvRow { Name = "", Club = "A", DecoratedName = " (A)", IsFemale = "" },
            new RoundRobinRiderCsvRow { Name = "Bob", Club = "", DecoratedName = "Bob ()", IsFemale = "" }
        };

            var issues = RoundRobinRiderCsvValidator.Validate(rows).ToList();

            issues.Should().Contain("Line 2: Name is missing.");
        }
    }
}
