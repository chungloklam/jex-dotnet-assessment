using System.ComponentModel.DataAnnotations;

namespace JexAssessment.DTOs;

public class UpdateVacancyDto
{
    [Range(1, int.MaxValue)]
    public int CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}