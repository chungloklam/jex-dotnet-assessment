using JexAssessment.Data;
using JexAssessment.DTOs;
using JexAssessment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JexAssessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VacanciesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public VacanciesController(AppDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VacancyDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var vacancies = await _dbContext.Vacancies
            .AsNoTracking()
            .Select(v => new VacancyDto
            {
                Id = v.Id,
                CompanyId = v.CompanyId,
                Title = v.Title,
                Description = v.Description,
                IsActive = v.IsActive
            })
            .ToListAsync(cancellationToken);

        return Ok(vacancies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VacancyDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var vacancy = await _dbContext.Vacancies
            .AsNoTracking()
            .Where(v => v.Id == id)
            .Select(v => new VacancyDto
            {
                Id = v.Id,
                CompanyId = v.CompanyId,
                Title = v.Title,
                Description = v.Description,
                IsActive = v.IsActive
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (vacancy is null)
            return NotFound();

        return Ok(vacancy);
    }

    [HttpPost]
    public async Task<ActionResult<VacancyDto>> Create(
        CreateVacancyDto request,
        CancellationToken cancellationToken)
    {
        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (!companyExists)
            return NotFound("Company not found.");

        var vacancy = new Vacancy
        {
            CompanyId = request.CompanyId,
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            IsActive = request.IsActive
        };

        _dbContext.Vacancies.Add(vacancy);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new VacancyDto
        {
            Id = vacancy.Id,
            CompanyId = vacancy.CompanyId,
            Title = vacancy.Title,
            Description = vacancy.Description,
            IsActive = vacancy.IsActive
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = vacancy.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateVacancyDto request,
        CancellationToken cancellationToken)
    {
        var vacancy = await _dbContext.Vacancies
            .FindAsync([id], cancellationToken);

        if (vacancy is null)
            return NotFound();

        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

        if (!companyExists)
            return NotFound("Company not found.");

        vacancy.CompanyId = request.CompanyId;
        vacancy.Title = request.Title.Trim();
        vacancy.Description = request.Description.Trim();
        vacancy.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var vacancy = await _dbContext.Vacancies
            .FindAsync([id], cancellationToken);

        if (vacancy is null)
            return NotFound();

        _dbContext.Vacancies.Remove(vacancy);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }
}