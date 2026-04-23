using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewLeads")]
public class LeadController(ILeadService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LeadDto>>> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<LeadDto>> GetById(int id, CancellationToken ct)
        => (await service.GetByIdAsync(id, ct)) is { } dto ? Ok(dto) : NotFound();

    [HttpPost]
    [HasPermission("CreateLeads")]
    public async Task<ActionResult<LeadDto>> Create([FromBody] CreateLeadDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var created = await service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("Лимит", StringComparison.OrdinalIgnoreCase) ||
            ex.Message.Contains("офисы достигли", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Лимит офиса исчерпан",
                detail: ex.Message,
                type: "about:blank"
            );
        }
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditLeads")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLeadDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPut("{id:int}/assign")]
    [HasPermission("EditLeads")]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignUserToLeadDto? dto, CancellationToken ct)
    {
        if (dto is null) return BadRequest("Body is required");
        try
        {
            await service.AssignUserAsync(id, dto.UserId, ct);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (
            ex.Message.Contains("Лимит", StringComparison.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Лимит офиса исчерпан",
                detail: ex.Message,
                type: "about:blank"
            );
        }
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteLeads")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("filter")]
    public async Task<ActionResult<IEnumerable<LeadDto>>> Filter([FromQuery] string status, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(status)) return BadRequest("status is required");
        return Ok(await service.FilterByStatusAsync(status, ct));
    }

    [HttpGet("search")]
    public async Task<ActionResult<LeadPageDto>> Search([FromQuery] LeadFilterDto filter, CancellationToken ct)
        => Ok(await service.SearchAsync(filter, ct));

    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IReadOnlyList<LeadHistoryDto>>> History(int id, CancellationToken ct)
        => Ok(await service.GetHistoryAsync(id, ct));
}