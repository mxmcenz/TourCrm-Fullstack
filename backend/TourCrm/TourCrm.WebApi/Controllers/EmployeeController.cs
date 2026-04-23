using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Employees;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewEmployees")]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(UserId, ct));

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 5,
        [FromQuery] int? officeId = null, [FromQuery] bool? isDeleted = null, CancellationToken ct = default)
    {
        if (page < 1 || pageSize < 1) return BadRequest("page и pageSize должны быть положительными числами");
        return Ok(await service.GetPagedAsync(page, pageSize, officeId, isDeleted, UserId, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var dto = await service.GetByIdAsync(id, UserId, ct);
        return dto is null ? NotFound(new { message = $"Сотрудник с ID {id} не найден" }) : Ok(dto);
    }

    [HttpGet("office/{officeId:int}")]
    public async Task<IActionResult> GetByOffice(int officeId, CancellationToken ct)
    {
        var items = await service.GetByOfficeAsync(officeId, UserId, ct);
        if (!items.Any())
            return NotFound(new { message = $"В офисе с ID {officeId} нет сотрудников или офис не существует" });
        return Ok(items);
    }
    
    [HttpGet("generate-password")]
    [HasPermission("CreateEmployees")]
    public async Task<IActionResult> GeneratePassword([FromQuery] int length = 12)
    {
        var password = await service.GeneratePasswordAsync(length);
        return Ok(new { password });
    }


    [HttpPost]
    [HasPermission("CreateEmployees")]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await service.CreateAsync(dto, UserId, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditEmployees")]
    public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var ok = await service.UpdateAsync(id, dto, UserId, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteEmployees")]
    public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
        => await service.MarkAsDeletedAsync(id, UserId, ct) ? NoContent() : NotFound();

    [HttpPatch("{id:int}/restore")]
    [HasPermission("EditEmployees")]
    public async Task<IActionResult> Restore(int id, CancellationToken ct)
        => await service.RestoreAsync(id, UserId, ct) ? NoContent() : NotFound();
}