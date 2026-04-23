using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Clients;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewClients")]
public class ClientsController(IClientService clients, IAuditQueryService audit) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string? q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeDeleted = false,
        CancellationToken ct = default)
    {
        var companyId = GetCompanyId();
        var (items, total) = await clients.SearchAsync(companyId, q, page, pageSize, includeDeleted, ct);
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("deleted")]
    public async Task<IActionResult> SearchDeleted([FromQuery] string? q, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var companyId = GetCompanyId();
        var (items, total) = await clients.SearchDeletedAsync(companyId, q, page, pageSize, ct);
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, [FromQuery] bool includeDeleted = false,
        CancellationToken ct = default)
    {
        var companyId = GetCompanyId();
        var item = await clients.GetAsync(id, companyId, includeDeleted, ct);
        return item is null ? NotFound() : Ok(item);
    }


    [HttpGet("{id:int}/history")]
    public async Task<IActionResult> GetHistory(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50,
        CancellationToken ct = default)
    {
        var companyId = GetCompanyId();
        var (items, total) = await audit.GetByEntityAsync(companyId, "Client", id.ToString(), page, pageSize, ct);
        Response.Headers["X-Total-Count"] = total.ToString();
        return Ok(items);
    }

    [HttpPost]
    [HasPermission("CreateClients")]
    public async Task<IActionResult> Create([FromBody] CreateClientDto dto, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        var created = await clients.CreateAsync(companyId, userId, dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditClients")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClientDto dto, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        await clients.UpdateAsync(id, companyId, userId, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteClients")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        await clients.SoftDeleteAsync(id, companyId, userId, ct);
        return NoContent();
    }
    
    [HttpPost("{id:int}/restore")]
    [HasPermission("EditClients")]
    public async Task<IActionResult> Restore(int id, CancellationToken ct)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();
        await clients.RestoreAsync(id, companyId, userId, ct);
        return NoContent();
    }

    private int? GetUserId()
    {
        var v = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(v, out var id) ? id : null;
    }

    private int GetCompanyId()
    {
        var claim = User.FindFirst("companyId")?.Value ?? User.FindFirst("CompanyId")?.Value;
        if (int.TryParse(claim, out var id)) return id;
        throw new UnauthorizedAccessException("CompanyId is missing");
    }
}