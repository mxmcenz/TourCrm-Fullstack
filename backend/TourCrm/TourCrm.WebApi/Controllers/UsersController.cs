using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class UsersController(IUserService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(CancellationToken ct)
    {
        var res = await service.GetAllAsync(ct);
        if (!res.Success) return BadRequest(res.Message);
        return Ok(res.Data);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id, CancellationToken ct)
    {
        var res = await service.GetByIdAsync(id, ct);
        if (!res.Success) return NotFound(res.Message);
        return Ok(res.Data);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] CreateUserDto dto, CancellationToken ct)
    {
        var res = await service.CreateAsync(dto, ct);
        if (!res.Success) return BadRequest(res.Message);
        return CreatedAtAction(nameof(GetUser), new { id = res.Data }, res.Data);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken ct)
    {
        var res = await service.UpdateAsync(id, dto, ct);
        if (!res.Success) return BadRequest(res.Message);
        return Ok(res.Message);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var res = await service.DeleteAsync(id, ct);
        if (!res.Success) return NotFound(res.Message);
        return Ok(res.Message);
    }
}