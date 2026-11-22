using System.ComponentModel.DataAnnotations;

namespace JexAssessment.DTOs;

public class PublicCreateVacancyDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }
}