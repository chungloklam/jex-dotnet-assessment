using System.ComponentModel.DataAnnotations;

namespace JexAssessment.DTOs;

public class UpdateCompanyDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;
}