using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.LegalEntity;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewLegalEntities")]
public class LegalEntitiesController(ILegalEntityService service) : ControllerBase
{
    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<IActionResult> GetMine([FromQuery] string? query, [FromQuery] string? q, CancellationToken ct)
    {
        var term = string.IsNullOrWhiteSpace(query) ? q : query;
        var items = await service.GetMineAsync(UserId, term, ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => (await service.GetAsync(id, UserId, ct)) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    [HasPermission("CreateLegalEntities")]
    public async Task<IActionResult> Create([FromBody] LegalEntityUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await service.CreateAsync(dto, UserId, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditLegalEntities")]
    public async Task<IActionResult> Update(int id, [FromBody] LegalEntityUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await service.UpdateAsync(id, dto, UserId, ct));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteLegalEntities")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.SoftDeleteAsync(id, UserId, ct);
        return NoContent();
    }
}