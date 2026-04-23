using TourCrm.Core.Entities;

namespace TourCrm.Core.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<Employee?> GetByIdAsync(int id);
    Task<IEnumerable<Employee>> GetAllAsync();
    
    Task<(IReadOnlyList<Employee> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        int? officeId = null,
        bool? isDeleted = null
    );

    Task<IReadOnlyList<Employee>> GetByOfficeAsync(int officeId);
}