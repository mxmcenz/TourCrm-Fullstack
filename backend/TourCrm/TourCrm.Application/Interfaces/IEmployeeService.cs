using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Employees;

namespace TourCrm.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(string userId, CancellationToken ct = default);
    Task<EmployeeDto?> GetByIdAsync(int id, string userId, CancellationToken ct = default);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto, string userId, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, EmployeeUpdateDto dto, string userId, CancellationToken ct = default);
    Task<bool> MarkAsDeletedAsync(int id, string userId, CancellationToken ct = default);
    Task<bool> RestoreAsync(int id, string userId, CancellationToken ct = default);
    Task<IEnumerable<EmployeeDto>> GetByOfficeAsync(int officeId, string userId, CancellationToken ct = default);

    Task<PagedResult<EmployeeDto>> GetPagedAsync(int page, int pageSize, int? officeId, bool? isDeleted, string userId,
        CancellationToken ct = default);
    Task<string> GeneratePasswordAsync(int length = 12);
}