using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController(IPermissionProvider permissionProvider) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await permissionProvider.GetPermissionsAsync());

    [HttpGet("tree")]
    public async Task<ActionResult<IReadOnlyList<PermissionCategoryDto>>> GetPermissionTree(CancellationToken ct)
        => Ok(await permissionProvider.GetPermissionTreeAsync());
}