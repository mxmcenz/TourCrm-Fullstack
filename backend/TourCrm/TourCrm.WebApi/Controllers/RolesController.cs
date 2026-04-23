using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.DTOs.Roles;
using TourCrm.Application.Interfaces;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewRoles")]
public class RolesController(IRoleService roleService) : ControllerBase
{
    private string UserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await roleService.GetAllRolesAsync(UserId, ct);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpGet("{id:int}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await roleService.GetRoleByIdAsync(id, UserId, ct);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result.Data);
    }

    [HttpPost]
    [HasPermission("CreateRoles")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken ct)
    {
        var result = await roleService.CreateRoleAsync(dto, UserId, ct);
        if (!result.Success) return BadRequest(result.Message);

        var createdRole = result.Data!;
        return CreatedAtAction(nameof(GetById), new { id = createdRole.Id }, createdRole);
    }

    [HttpPut]
    [HasPermission("EditRoles")]
    public async Task<IActionResult> Update([FromBody] UpdateRoleDto dto, CancellationToken ct)
    {
        var existing = await roleService.GetRoleByIdAsync(dto.Id, UserId, ct);
        if (!existing.Success || existing.Data == null)
            return NotFound("Роль с таким ID не найдена");

        var result = await roleService.UpdateRoleAsync(dto, UserId, ct);
        if (!result.Success) return BadRequest(result.Message);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteRoles")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await roleService.DeleteRoleAsync(id, UserId, ct);
        if (!result.Success) return NotFound(result.Message);
        return NoContent();
    }

    [HttpGet("{roleId:int}/permissions")]
    public async Task<IActionResult> GetPermissions(int roleId, CancellationToken ct)
    {
        var result = await roleService.GetRolePermissionsAsync(roleId, UserId, ct);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result.Data);
    }

    [HttpPut("{roleId:int}/permissions")]
    [HasPermission("EditRoles")]
    public async Task<IActionResult> UpdatePermissions(int roleId, [FromBody] GrantPermissionsDto permissions,
        CancellationToken ct)
    {
        var result = await roleService.SetRolePermissionsAsync(roleId, UserId, permissions, ct);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Message);
    }

    [HttpPost("assign")]
    [HasPermission("EditRoles")]
    public async Task<IActionResult> AssignRoleToUser([FromQuery] int userId, [FromQuery] int roleId,
        CancellationToken ct)
    {
        var result = await roleService.AssignRoleToUserAsync(userId, roleId, UserId, ct);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Message);
    }

    [HttpDelete("remove")]
    [HasPermission("EditRoles")]
    public async Task<IActionResult> RemoveRoleFromUser([FromQuery] int userId, [FromQuery] int roleId,
        CancellationToken ct)
    {
        var result = await roleService.RemoveRoleFromUserAsync(userId, roleId, UserId, ct);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Message);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserRoles(int userId, CancellationToken ct)
    {
        var result = await roleService.GetUserRolesAsync(userId, UserId, ct);
        if (!result.Success) return NotFound(result.Message);
        return Ok(result.Data);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] RolesQuery query, CancellationToken ct)
    {
        var result = await roleService.GetRolesPagedAsync(query, UserId, ct);
        if (!result.Success) return BadRequest(result.Message);
        return Ok(result.Data);
    }

    [HttpGet("suggest")]
    public async Task<IActionResult> Suggest([FromQuery] string term, [FromQuery] int take = 10,
        CancellationToken ct = default)
    {
        var q = new RolesQuery { Page = 1, PageSize = take, Search = term, SortBy = "name", Desc = false };
        var result = await roleService.GetRolesPagedAsync(q, UserId, ct);
        if (!result.Success || result.Data is null) return BadRequest(result.Message);
        return Ok(result.Data.Items.Select(x => x.Name).ToList());
    }
}