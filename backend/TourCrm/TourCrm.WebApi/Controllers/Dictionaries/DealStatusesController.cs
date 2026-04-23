using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Dictionaries.DealStatus;
using TourCrm.Application.Interfaces.Dictionaries;

namespace TourCrm.WebApi.Controllers.Dictionaries;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class DealStatusesController(IDealStatusService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<DealStatusDto>>> GetAll(CancellationToken ct)
    {
        var items = await service.GetAllAsync(ct);
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DealStatusDto>> GetById(int id, CancellationToken ct)
    {
        var item = await service.GetByIdAsync(id, ct);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<DealStatusDto>> Create([FromBody] CreateDealStatusDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Не удалось определить текущего пользователя.");

        var created = await service.CreateAsync(dto, userId, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDealStatusDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("seed-defaults/{companyId:int}")]
    public async Task<ActionResult<object>> SeedDefaults(int companyId, CancellationToken ct)
    {
        var inserted = await service.SeedDefaultsForCompanyAsync(companyId, ct);
        return Ok(new { inserted });
    }
}
