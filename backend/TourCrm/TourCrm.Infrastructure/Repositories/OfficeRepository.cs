using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class OfficeRepository(TourCrmDbContext context) : GenericRepository<Office>(context), IOfficeRepository
{
    public Task<bool> ExistsNameInLegalAsync(int legalEntityId, string name, CancellationToken ct = default) =>
        _context.Offices.AnyAsync(o =>
                !o.IsDeleted
                &&  o.LegalEntityId == legalEntityId
                &&  EF.Functions.ILike(o.Name, name), // регистронезависимое равенство
            ct);

    public async Task<Office?> GetByIdAsync(int id, bool includeLegal = false, bool includeEmployees = false, CancellationToken ct = default)
    {
        var q = _context.Offices.AsQueryable();
        if (includeLegal)     q = q.Include(o => o.LegalEntity);
        if (includeEmployees) q = q.Include(o => o.Employees);
        return await q.FirstOrDefaultAsync(o => o.Id == id, ct);
    }

    public async Task<IReadOnlyList<Office>> GetByLegalAsync(int legalEntityId, bool includeLegal = false, bool includeEmployees = false, 
        CancellationToken ct = default)
    {
        var q = _context.Offices
            .AsNoTracking()
            .Where(o => o.LegalEntityId == legalEntityId
                        && !o.IsDeleted
                        && !o.LegalEntity.IsDeleted);
        if (includeLegal) q = q.Include(o => o.LegalEntity);
        if (includeEmployees) q = q.Include(o => o.Employees);
        return await q.OrderBy(o => o.Name).ToListAsync(ct);
    }
}