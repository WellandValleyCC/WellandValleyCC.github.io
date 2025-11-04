using System.ComponentModel.DataAnnotations;

public enum AgeGroup
{
    [Display(Name = "Undefined")]
    Undefined = 0,

    [Display(Name = "Juvenile")]
    Juvenile = 1,

    [Display(Name = "Junior")]
    Junior = 2,

    [Display(Name = "Senior")]
    Senior = 3,

    [Display(Name = "Veteran")]
    Veteran = 4
}
