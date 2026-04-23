using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/leads/{leadId:int}/selections")]
[Authorize]
public class LeadSelectionsController(ILeadSelectionService service) : ControllerBase
{
    [HttpGet("single")]
    public async Task<ActionResult<LeadSelectionDto>> GetSingle(
        int leadId, CancellationToken ct)
    {
        var userId = User?.Identity?.Name ?? "system";
        var dto = await service.GetSingleByLeadAsync(leadId, userId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeadSelectionDto>> GetById(
        int leadId, int id, CancellationToken ct)
    {
        var userId = User?.Identity?.Name ?? "system";
        var dto = await service.GetAsync(leadId, id, userId, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<LeadSelectionDto>> Create(
        int leadId, [FromBody] CreateLeadSelectionDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User?.Identity?.Name ?? "system";
        var createdOrUpdated = await service.CreateAsync(leadId, dto, userId, ct);
        return CreatedAtAction(nameof(GetById), new { leadId, id = createdOrUpdated.Id }, createdOrUpdated);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<LeadSelectionDto>> Update(
        int leadId, int id, [FromBody] UpdateLeadSelectionDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var userId = User?.Identity?.Name ?? "system";
        var updated = await service.UpdateAsync(leadId, id, dto, userId, ct);
        return Ok(updated);
    }
}