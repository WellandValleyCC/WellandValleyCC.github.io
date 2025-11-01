using System.ComponentModel.DataAnnotations;

namespace ClubProcessor.Models.Enums
{
    public enum RideEligibility
    {
        [Display(Name = "Undefined")]
        Undefined = 0,

        [Display(Name = "Valid")]
        Valid = 1,

        [Display(Name = "DNS")]
        DNS = 2,

        [Display(Name = "DQ")]
        DQ = 3,

        [Display(Name = "DNF")]
        DNF = 4
    }
}
