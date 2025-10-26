using AutoFixture;
using ClubProcessor.Models.Enums;

public class ClaimStatusCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        var validStatuses = new[]
        {
            ClaimStatus.FirstClaim,
            ClaimStatus.SecondClaim,
            ClaimStatus.Honorary
        };

        fixture.Register(() =>
        {
            var random = new Random();
            return validStatuses[random.Next(validStatuses.Length)];
        });
    }
}
