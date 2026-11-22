using JexAssessment.Data;
using JexAssessment.DTOs;
using JexAssessment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JexAssessment.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public CompaniesController(AppDbContext dbContext) => _dbContext = dbContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var companies = await _dbContext.Companies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address
            })
            .ToListAsync(cancellationToken);

        return Ok(companies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CompanyDto>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (company is null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create(
        CreateCompanyDto request,
        CancellationToken cancellationToken)
    {
        var company = new Company
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim()
        };

        _dbContext.Companies.Add(company);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var response = new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Address = company.Address
        };

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCompanyDto request,
        CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .FindAsync([id], cancellationToken);

        if (company is null)
            return NotFound();

        company.Name = request.Name.Trim();
        company.Address = request.Address.Trim();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var company = await _dbContext.Companies
            .FindAsync([id], cancellationToken);

        if (company is null)
            return NotFound();

        var hasVacancies = await _dbContext.Vacancies
            .AnyAsync(v => v.CompanyId == id, cancellationToken);

        if (hasVacancies)
            return Conflict(
                "Cannot delete a company that has vacancies.");

        _dbContext.Companies.Remove(company);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet("with-active-vacancies")]
    public async Task<ActionResult<IEnumerable<CompanyWithVacanciesDto>>>
        GetCompaniesWithActiveVacancies(
            CancellationToken cancellationToken)
    {
        var companies = await _dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Vacancies.Any(v => v.IsActive))
            .Select(c => new CompanyWithVacanciesDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                Vacancies = c.Vacancies
                    .Where(v => v.IsActive)
                    .Select(v => new VacancyDto
                    {
                        Id = v.Id,
                        CompanyId = v.CompanyId,
                        Title = v.Title,
                        Description = v.Description,
                        IsActive = v.IsActive
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return Ok(companies);
    }

    [HttpPost("{companyId:int}/vacancies")]
    public async Task<ActionResult<VacancyDto>> CreatePublicVacancy(
        int companyId,
        PublicCreateVacancyDto request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest("Title is required.");

        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == companyId, cancellationToken);

        if (!companyExists)
            return NotFound("Company not found.");

        var vacancy = new Vacancy
        {
            CompanyId = companyId,
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            IsActive = false
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
            nameof(VacanciesController.GetById),
            "Vacancies",
            new { id = vacancy.Id },
            response);
    }
}