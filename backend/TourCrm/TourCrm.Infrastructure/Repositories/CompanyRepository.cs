using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class CompanyRepository(TourCrmDbContext context)
    : GenericRepository<Company>(context), ICompanyRepository
{
    public Task<Company?> GetByOwnerAsync(string ownerUserId, CancellationToken ct = default) =>
        _context.Companies
            .AsNoTracking()
            .Include(c => c.LegalEntity)
            .Include(c => c.LegalEntities)
            .FirstOrDefaultAsync(c => c.OwnerUserId == ownerUserId, ct);

    public Task<bool> ExistsForOwnerAsync(string ownerUserId, CancellationToken ct = default) =>
        _context.Companies.AnyAsync(c => c.OwnerUserId == ownerUserId, ct);

    public Task<Company?> GetByLegalEntityIdAsync(int legalEntityId, CancellationToken ct = default) =>
        _context.Companies
            .AsNoTracking()
            .Include(c => c.LegalEntities)
            .FirstOrDefaultAsync(c => c.LegalEntities.Any(le => le.Id == legalEntityId), ct);
}