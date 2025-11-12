using System.ComponentModel.DataAnnotations;

namespace ClubCore.Models.Enums
{
    public enum ClaimStatus
    {
        [Display(Name = "Unknown")]
        Unknown = 0,

        [Display(Name = "First Claim")]
        FirstClaim = 1,

        [Display(Name = "Second Claim")]
        SecondClaim = 2,

        [Display(Name = "Honorary")]
        Honorary = 3
    }
}
