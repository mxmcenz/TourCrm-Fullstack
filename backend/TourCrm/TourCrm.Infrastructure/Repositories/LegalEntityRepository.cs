using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class LegalEntityRepository(TourCrmDbContext context)
    : GenericRepository<LegalEntity>(context), ILegalEntityRepository
{
    public Task<bool> ExistsByNameInCompanyAsync(int companyId, string displayName, CancellationToken ct = default) =>
        _context.LegalEntities.AnyAsync(x =>
            !x.IsDeleted &&
            x.CompanyId == companyId &&
            x.Name.ToLower() == displayName.ToLower(), ct);

    public async Task<IReadOnlyList<LegalEntity>> GetByCompanyAsync(int companyId, CancellationToken ct = default) =>
        await _context.LegalEntities
            .Where(x => x.CompanyId == companyId && !x.IsDeleted)
            .Include(x => x.CountryRef)
            .Include(x => x.CityRef)
            .Include(x => x.Offices).ThenInclude(o => o.Employees)
            .OrderBy(x => x.Name)
            .ToListAsync(ct);
}