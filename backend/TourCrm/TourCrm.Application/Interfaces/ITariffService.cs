using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs.Tariffs;

namespace TourCrm.Application.Interfaces;

public interface ITariffService
{
    Task<ServiceResult<TariffDto>> CreateAsync(CreateTariffDto dto, CancellationToken ct = default);
    Task<ServiceResult<TariffDto>> UpdateAsync(int id, UpdateTariffDto dto, CancellationToken ct);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);

    Task<ServiceResult<TariffDto?>> GetAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<List<TariffDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceResult<TariffDto?>> SuggestForEmployeesAsync(int employees, CancellationToken ct = default);

    Task<ServiceResult<object>> AssignToCompanyAsync(int companyId, int tariffId, CancellationToken ct = default);
    Task<ServiceResult<object>> ChangeCompanyTariffAsync(int companyId, int tariffId, CancellationToken ct = default);
    Task<ServiceResult<object>> RemoveCompanyTariffAsync(int companyId, CancellationToken ct = default);
}