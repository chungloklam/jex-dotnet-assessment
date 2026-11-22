namespace JexAssessment.DTOs;

public class VacancyDto
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}