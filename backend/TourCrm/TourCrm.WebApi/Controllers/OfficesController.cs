using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Offices;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewOffices")]
public class OfficesController(IOfficeService service) : ControllerBase
{
    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OfficeDto>> Get(int id, CancellationToken ct)
    {
        var dto = await service.GetAsync(id, UserId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("by-legal/{legalEntityId:int}")]
    public async Task<ActionResult<IEnumerable<OfficeListItemDto>>> GetByLegal(
        int legalEntityId,
        [FromQuery] string? query,
        [FromQuery] string? q,
        CancellationToken ct)
    {
        var term = string.IsNullOrWhiteSpace(query) ? q : query;
        var items = await service.GetByLegalAsync(UserId, legalEntityId, term, ct);
        return Ok(items);
    }

    [HttpPost]
    [HasPermission("CreateOffices")]
    public async Task<ActionResult<OfficeDto>> Create([FromBody] OfficeUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await service.CreateAsync(dto, UserId, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditOffices")]
    public async Task<ActionResult<OfficeDto>> Update(int id, [FromBody] OfficeUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var updated = await service.UpdateAsync(id, dto, UserId, ct);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteOffices")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.SoftDeleteAsync(id, UserId, ct);
        return NoContent();
    }
}