using ClubCore.Utilities;
using FluentAssertions;

public class NamePartsTests
{
    [Theory]
    [InlineData("John Smith", "Smith", "John")]
    [InlineData("Alice Mary Johnson", "Johnson", "Alice Mary")]
    [InlineData("Jean-Claude Van Damme", "Damme", "Jean-Claude Van")]
    [InlineData("Madonna", "Madonna", "")]
    [InlineData("  Sarah Connor  ", "Connor", "Sarah")]
    [InlineData("Juan Carlos Ortega", "Ortega", "Juan Carlos")]
    [InlineData("Anne-Marie O'Neill", "O'Neill", "Anne-Marie")]
    [InlineData("", "", "")]
    [InlineData("   ", "", "")]
    public void Split_ReturnsExpectedParts(string fullName, string expectedSurname, string expectedGivenNames)
    {
        var (surname, givenNames) = NameParts.Split(fullName);

        surname.Should().Be(expectedSurname);
        givenNames.Should().Be(expectedGivenNames);
    }
}