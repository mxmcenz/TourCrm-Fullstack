using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Dictionaries.Hotels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers.Dictionaries;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewDictionaries")]
public sealed class HotelsController(IHotelService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpPost]
    [HasPermission("CreateDictionaries")]
    public async Task<IActionResult> Create([FromBody] CreateHotelDto dto, CancellationToken ct)
        => StatusCode(StatusCodes.Status201Created, await service.CreateAsync(dto, ct));

    [HttpPut("{id:int}")]
    [HasPermission("EditDictionaries")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHotelDto dto, CancellationToken ct)
    { await service.UpdateAsync(id, dto, ct); return NoContent(); }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteDictionaries")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    { await service.DeleteAsync(id, ct); return NoContent(); }
}