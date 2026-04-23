using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class UserRepository(TourCrmDbContext context) : IUserRepository
{
    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        => await context.Users.AsNoTracking().ToListAsync(ct);

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Users.FindAsync([id], ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await context.Users.AddAsync(user, ct);

    public void Update(User user) => context.Users.Update(user);
    public void Delete(User user) => context.Users.Remove(user);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    
    public async Task<User?> GetByPhoneAsync(string phone)
        => await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phone);
    
    public async Task<List<Employee>> GetCompanyEmployeesAsync(int companyId, CancellationToken ct = default)
    {
        return await context.Employees
            .AsNoTracking()
            .Where(e => e.LegalEntity.CompanyId == companyId
                        && !e.Office.IsDeleted
                        && !e.LegalEntity.IsDeleted)
            .ToListAsync(ct);
    }
}