using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs.Dictionaries.Labels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.WebApi.Attributes;

namespace TourCrm.WebApi.Controllers.Dictionaries;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[HasPermission("ViewDictionaries")]
public class LabelsController(ILabelService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await service.GetAllAsync(ct));

    [HttpPost]
    [HasPermission("CreateDictionaries")]
    public async Task<IActionResult> Create([FromBody] CreateLabelDto dto, CancellationToken ct)
        => StatusCode(StatusCodes.Status201Created, await service.CreateAsync(dto, ct));

    [HttpPut("{id:int}")]
    [HasPermission("EditDictionaries")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLabelDto dto, CancellationToken ct)
    { await service.UpdateAsync(id, dto, ct); return NoContent(); }

    [HttpDelete("{id:int}")]
    [HasPermission("DeleteDictionaries")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    { await service.DeleteAsync(id, ct); return NoContent(); }
}