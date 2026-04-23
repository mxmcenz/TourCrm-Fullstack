using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Citizenship;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers.Dictionaries;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewDictionaries")]
public class CitizenshipController(ICitizenshipService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct)
    {
        var item = await service.GetByIdAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [HasPermission("CreateDictionaries")]
    public async Task<IActionResult> Create([FromBody] CreateCitizenshipDto dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new UnauthorizedAccessException();
        var created = await service.CreateAsync(dto,userId, ct);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [HttpPut("{id:int}")]
    [HasPermission("EditDictionaries")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCitizenshipDto dto, CancellationToken ct)
    {
        await service.UpdateAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteDictionaries")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}