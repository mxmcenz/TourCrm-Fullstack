using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Companies;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController(ICompanyService service, IReferenceDataSeeder referenceDataSeeder) : ControllerBase
{
    private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)
                              ?? User.FindFirstValue("sub");

    [HttpGet("mine")]
    public async Task<ActionResult<CompanyDto?>> GetMine(CancellationToken ct)
    {
        if (UserId is null) return Unauthorized();
        var entity = await service.GetMineAsync(UserId, ct);
        if (entity is null) return Ok(null);

        return Ok(new CompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            LegalEntityId = entity.LegalEntityId,
            LegalEntityName = entity.LegalEntity?.NameRu,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] CompanyUpsertDto dto, CancellationToken ct)
    {
        if (UserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");

        var entity = await service.CreateAsync(UserId, dto.Name.Trim(), ct);

        return StatusCode(StatusCodes.Status201Created, new CompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut("mine/name")]
    public async Task<ActionResult<CompanyDto>> Rename([FromBody] CompanyUpsertDto dto, CancellationToken ct)
    {
        if (UserId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest("Name is required.");

        var entity = await service.RenameAsync(UserId, dto.Name.Trim(), ct);
        return Ok(new CompanyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            LegalEntityId = entity.LegalEntityId,
            LegalEntityName = entity.LegalEntity?.NameRu,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        });
    }

    [HttpPut("mine/main-legal/{legalEntityId:int}")]
    public async Task<IActionResult> SetMainLegal(int legalEntityId, CancellationToken ct)
    {
        if (UserId is null) return Unauthorized();
        await service.SetMainLegalAsync(UserId, legalEntityId, ct);
        return NoContent();
    }

    [HttpPost("seed/mine")]
    public async Task<IActionResult> SeedMine(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException();

        var company = await service.GetMineAsync(userId, ct);
        var companyId = company!.Id;
        await referenceDataSeeder.SeedAllAsync(companyId, ct);
        return Ok("Defaults seeded");
    }
}