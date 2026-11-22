namespace JexAssessment.Models;

public class Vacancy
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; } = false;

    public Company Company { get; set; } = null!;
}