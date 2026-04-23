using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewDeals")]
public class DealsController(IDealService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? statusId, [FromQuery] int? officeId, [FromQuery] int? managerId, CancellationToken ct)
        => Ok(await service.GetAllAsync(statusId, officeId, managerId, ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
        => (await service.GetAsync(id, ct)) is { } d ? Ok(d) : NotFound();

    [HttpPost]
    [HasPermission("CreateDeals")]
    public async Task<IActionResult> Create([FromBody] CreateDealDto dto, CancellationToken ct)
        => StatusCode(201, await service.CreateAsync(dto, ct));

    [HttpPut("{id:int}")]
    [HasPermission("EditDeals")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDealDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/status/{statusId:int}")]
    [HasPermission("EditDeals")]
    public async Task<IActionResult> ChangeStatus(int id, int statusId, CancellationToken ct)
    {
        await service.ChangeStatusAsync(id, statusId, ct);
        return NoContent();
    }

    [HttpPost("from-lead/{leadId:int}")]
    [HasPermission("CreateDeals")]
    public async Task<IActionResult> CreateFromLead(int leadId, [FromQuery] int managerId, [FromQuery] int touristId, CancellationToken ct)
        => StatusCode(StatusCodes.Status201Created, await service.CreateFromLeadAsync(leadId, managerId, touristId, ct));
    
    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> GetHistory(int id, CancellationToken ct)
        => Ok(await service.GetHistoryAsync(id, ct));
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] DealSearchRequestDto req, CancellationToken ct)
        => Ok(await service.SearchAsync(req, ct));
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        await service.ArchiveAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/restore")]
    public async Task<IActionResult> Restore(int id, CancellationToken ct)
    {
        await service.RestoreAsync(id, ct);
        return NoContent();
    }
}