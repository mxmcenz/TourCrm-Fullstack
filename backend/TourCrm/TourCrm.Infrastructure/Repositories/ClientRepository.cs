using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Client;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class ClientRepository(TourCrmDbContext context) : GenericRepository<Client>(context), IClientRepository
{
    public async Task<Client?> GetByIdAsync(int companyId, int id, CancellationToken ct = default)
        => await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Id == id, ct);

    public Task<Client?> GetWithDetailsAsync(int companyId, int id, bool includeDeleted, CancellationToken ct)
    {
        var q = _dbSet.AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.Id == id)
            .Include(x => x.Passport)
            .Include(x => x.IdentityDocument)
            .Include(x => x.BirthCertificate)
            .Include(x => x.Insurances)
            .Include(x => x.Visas)
            .AsSplitQuery();

        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q.SingleOrDefaultAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(int companyId, string email, int? excludeClientId = null,
        CancellationToken ct = default)
        => await _dbSet.AnyAsync(
            x => x.CompanyId == companyId && x.Email == email &&
                 (excludeClientId == null || x.Id != excludeClientId), ct);

    public async Task<bool> PhoneExistsAsync(int companyId, string phoneE164, int? excludeClientId = null,
        CancellationToken ct = default)
        => await _dbSet.AnyAsync(
            x => x.CompanyId == companyId && x.PhoneE164 == phoneE164 &&
                 (excludeClientId == null || x.Id != excludeClientId), ct);

    public async Task<IReadOnlyList<Client>> SearchAsync(int companyId, string? query, int page, int pageSize,
        CancellationToken ct = default)
    {
        var q = _dbSet.AsNoTracking().Where(x => x.CompanyId == companyId);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var like = $"%{query.Trim()}%";
            q = q.Where(x =>
                EF.Functions.ILike(x.FirstName, like) ||
                EF.Functions.ILike(x.LastName, like) ||
                EF.Functions.ILike(x.MiddleName, like) ||
                (x.Email != null && EF.Functions.ILike(x.Email, like)) ||
                (x.PhoneE164 != null && EF.Functions.ILike(x.PhoneE164, like)));
        }

        return await q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
    }

    public async Task<int> CountAsync(int companyId, string? query, CancellationToken ct = default)
    {
        var q = _dbSet.Where(x => x.CompanyId == companyId);
        if (!string.IsNullOrWhiteSpace(query))
        {
            var like = $"%{query.Trim()}%";
            q = q.Where(x =>
                EF.Functions.ILike(x.FirstName, like) ||
                EF.Functions.ILike(x.LastName, like) ||
                EF.Functions.ILike(x.MiddleName, like) ||
                (x.Email != null && EF.Functions.ILike(x.Email, like)) ||
                (x.PhoneE164 != null && EF.Functions.ILike(x.PhoneE164, like)));
        }

        return await q.CountAsync(ct);
    }

    public Task<Client?> GetForUpdateAsync(int companyId, int id, CancellationToken ct = default)
        => GetForUpdateAsync(companyId, id, includeDeleted: false, ct);

    public Task<Client?> GetForUpdateAsync(int companyId, int id, bool includeDeleted, CancellationToken ct)
    {
        var q = _dbSet
            .Where(x => x.CompanyId == companyId && x.Id == id)
            .Include(x => x.Passport)
            .Include(x => x.IdentityDocument)
            .Include(x => x.BirthCertificate)
            .Include(x => x.Insurances)
            .Include(x => x.Visas)
            .AsSplitQuery();

        if (includeDeleted) q = q.IgnoreQueryFilters();
        return q.SingleOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<Client> items, int total)> SearchWithDetailsAsync(
        int companyId, string? q, int page, int pageSize, bool includeDeleted, CancellationToken ct)
    {
        var queryable = _dbSet
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.Passport)
            .Include(x => x.IdentityDocument)
            .Include(x => x.BirthCertificate)
            .Include(x => x.Insurances)
            .Include(x => x.Visas)
            .AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q.Trim()}%";
            queryable = queryable.Where(x =>
                EF.Functions.ILike(x.FirstName + " " + x.LastName, like) ||
                (x.Email != null && EF.Functions.ILike(x.Email, like)) ||
                (x.PhoneE164 != null && EF.Functions.ILike(x.PhoneE164, like)));
        }

        if (includeDeleted)
            queryable = queryable.IgnoreQueryFilters();

        var total = await queryable.CountAsync(ct);

        var items = await queryable
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<(IReadOnlyList<Client> items, int total)> SearchDeletedAsync(
        int companyId, string? q, int page, int pageSize, CancellationToken ct)
    {
        var queryable = _dbSet
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(x => x.CompanyId == companyId && x.IsDeleted)
            .Include(x => x.Passport)
            .Include(x => x.IdentityDocument)
            .Include(x => x.BirthCertificate)
            .Include(x => x.Insurances)
            .Include(x => x.Visas)
            .AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var like = $"%{q.Trim()}%";
            queryable = queryable.Where(x =>
                EF.Functions.ILike(x.FirstName + " " + x.LastName, like) ||
                (x.Email != null && EF.Functions.ILike(x.Email, like)) ||
                (x.PhoneE164 != null && EF.Functions.ILike(x.PhoneE164, like)));
        }

        var total = await queryable.CountAsync(ct);

        var items = await queryable
            .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }
}