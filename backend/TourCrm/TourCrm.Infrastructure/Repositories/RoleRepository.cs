using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Roles;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class RoleRepository(TourCrmDbContext context) : GenericRepository<Role>(context), IRoleRepository
{
    public async Task<(List<Role> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string? sortBy,
        bool desc,
        string superAdminName,
        int? companyId)
    {
        var q = _context.Roles.AsNoTracking()
            .Where(r => r.CompanyId == companyId);

        if (!string.IsNullOrWhiteSpace(superAdminName))
            q = q.Where(r => !EF.Functions.ILike(r.Name, superAdminName));

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = $"%{search.Trim()}%";
            q = q.Where(r => EF.Functions.ILike(r.Name, term));
        }

        q = (sortBy?.ToLowerInvariant()) switch
        {
            "id" => desc ? q.OrderByDescending(r => r.Id) : q.OrderBy(r => r.Id),
            "name" => desc ? q.OrderByDescending(r => r.Name) : q.OrderBy(r => r.Name),
            _ => desc ? q.OrderByDescending(r => r.Name) : q.OrderBy(r => r.Name),
        };

        var total = await q.CountAsync();

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, total);
    }
}