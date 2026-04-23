using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;

namespace TourCrm.Application.Interfaces;

public interface IUserService
{
    Task<ServiceResult<List<UserDto>>> GetAllAsync(CancellationToken ct);
    Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct);
    Task<ServiceResult<int>> CreateAsync(CreateUserDto dto, CancellationToken ct);
    Task<ServiceResult<string>> UpdateAsync(int id, UpdateUserDto dto, CancellationToken ct);
    Task<ServiceResult<string>> DeleteAsync(int id, CancellationToken ct);
    Task<ServiceResult<UserDto>> GetByEmailAsync(string email, CancellationToken ct);
    Task<ServiceResult<UserDto>> GetByPhoneAsync(string phone, CancellationToken ct);
}