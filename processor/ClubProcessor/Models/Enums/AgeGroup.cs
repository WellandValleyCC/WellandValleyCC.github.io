using System.ComponentModel.DataAnnotations;

public enum AgeGroup
{
    [Display(Name = "Undefined")]
    Undefined = 0,

    [Display(Name = "Juvenile")]
    IsJuvenile = 1,

    [Display(Name = "Junior")]
    IsJunior = 2,

    [Display(Name = "Senior")]
    IsSenior = 3,

    [Display(Name = "Veteran")]
    IsVeteran = 4
}
