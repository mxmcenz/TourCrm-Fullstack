using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs.Tariffs;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Enums;

namespace TourCrm.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TariffsController(
    ITariffService tariffService,
    ITariffPricingService pricingService
) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResult<List<TariffDto>>>> GetAll(CancellationToken ct)
        => Ok(await tariffService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResult<TariffDto?>>> Get(int id, CancellationToken ct)
        => Ok(await tariffService.GetAsync(id, ct));

    [HttpGet("suggest")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResult<TariffDto?>>> Suggest([FromQuery] int employees, CancellationToken ct)
        => Ok(await tariffService.SuggestForEmployeesAsync(employees, ct));

    [HttpGet("{id:int}/price")]
    [AllowAnonymous]
    public async Task<ActionResult<decimal?>> GetPrice(int id, [FromQuery] BillingPeriod period, CancellationToken ct)
        => Ok(await pricingService.GetPriceAsync(id, period, ct));
    
    [HttpGet("{id:int}/price-per-month")]
    [AllowAnonymous]
    public async Task<ActionResult<decimal?>> GetPricePerMonth(int id, [FromQuery] BillingPeriod period, CancellationToken ct)
        => Ok(await pricingService.GetMonthlyEquivalentAsync(id, period, ct));

    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<TariffDto>>> Create([FromBody] CreateTariffDto dto,
        CancellationToken ct)
        => Ok(await tariffService.CreateAsync(dto, ct));

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<TariffDto>>> Update(int id, [FromBody] UpdateTariffDto dto, CancellationToken ct)
        => Ok(await tariffService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<object>>> Delete(int id, CancellationToken ct)
        => Ok(await tariffService.DeleteAsync(id, ct));

    [HttpPost("{tariffId:int}/assign")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<object>>> Assign([FromRoute] int tariffId, [FromQuery] int companyId,
        CancellationToken ct)
        => Ok(await tariffService.AssignToCompanyAsync(companyId, tariffId, ct));

    [HttpPost("{tariffId:int}/change")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<object>>> Change([FromRoute] int tariffId, [FromQuery] int companyId,
        CancellationToken ct)
        => Ok(await tariffService.ChangeCompanyTariffAsync(companyId, tariffId, ct));

    [HttpDelete("companies/{companyId:int}/assign")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ServiceResult<object>>> Unassign([FromRoute] int companyId, CancellationToken ct)
        => Ok(await tariffService.RemoveCompanyTariffAsync(companyId, ct));
}