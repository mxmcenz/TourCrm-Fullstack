using TourCrm.Core.Entities;

namespace TourCrm.Core.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByPhoneAsync(string phone);
    Task<List<Employee>> GetCompanyEmployeesAsync(int companyId, CancellationToken ct = default);
}